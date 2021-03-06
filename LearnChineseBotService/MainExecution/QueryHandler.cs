﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.LayoutRoot;

namespace YellowDuck.LearnChineseBotService.MainExecution
{
    public class QueryHandler
    {
        private readonly AntiDdosChecker _checker;
        private readonly TelegramBotClient _client;
        private readonly string _flashCardUrl;
        private readonly IWordRepository _repository;

        public int MaxInlineQueryLength = 5;
        public int MaxInlineSearchResult = 7;

        public QueryHandler(TelegramBotClient client, IWordRepository repository, AntiDdosChecker checker,
            string flashCardUrl)
        {
            _client = client;
            _repository = repository;
            _checker = checker;
            _flashCardUrl = flashCardUrl;
        }

        public async Task CallbackQuery(CallbackQuery callbackQuery)
        {
            try
            {
                var userId = callbackQuery.From.Id;
                if (!_checker.AllowUser(userId))
                    return;

                await OnMessage(callbackQuery.Message, callbackQuery.Data, callbackQuery.From);

                _checker.UserQueryProcessed(userId);
            }
            catch (Exception ex)
            {
                MainFactory.Log.Write("CallbackQuery", ex, null);
            }
        }

        public static string GetNoEmojiString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var noEmojiStr = new StringBuilder();

            foreach (var chr in str)
            {
                var cat = char.GetUnicodeCategory(chr);

                if (cat != UnicodeCategory.OtherSymbol && cat != UnicodeCategory.Surrogate &&
                    cat != UnicodeCategory.NonSpacingMark)
                    noEmojiStr.Append(chr);
            }

            return noEmojiStr.ToString();
        }

        public async Task InlineQuery(InlineQuery inlineQuery)
        {
            var userId = inlineQuery.From.Id;
            var q = inlineQuery.Query;

            if (!_checker.AllowUser(userId))
                return;

            if (q.Length > MaxInlineQueryLength)
                return;

            if (!_repository.IsUserExist(userId))
                _repository.AddUser(new LearnChinese.Data.Ef.User
                {
                    IdUser = userId,
                    Name = $"{inlineQuery.From.FirstName} {inlineQuery.From.LastName}",
                    Mode = EGettingWordsStrategy.Random.ToString()
                });

            IEnumerable<InlineQueryResult> inlineQueryResults;
            WordSearchResult[] results;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (q.Any())
                results = _repository.FindFlashCard(q, userId).ToArray();
            else
                results = _repository.GetLastWords(userId, MaxInlineSearchResult).ToArray();

            if (!results.Any())
            {
                await _client.AnswerInlineQueryAsync(inlineQuery.Id, new InlineQueryResult[]
                {
                    new InlineQueryResultArticle
                    {
                        Title = "Please, type a chinese word to view it's flashcard",
                        Id = inlineQuery.Id,
                        InputMessageContent =
                            new InputTextMessageContent
                            {
                                DisableWebPagePreview = true,
                                MessageText = "Still can't show you a flashcard",
                                ParseMode = ParseMode.Default
                            }
                    }
                });

                _checker.UserQueryProcessed(userId);
                return;
            }


            if (MainFactory.UseWebhooks)
                inlineQueryResults = results.Select(
                    a =>
                        new InlineQueryResultPhoto
                        {
                            Caption = a.OriginalWord,
                            Url = _flashCardUrl + a.FileId,
                            ThumbUrl = _flashCardUrl + a.FileId,
                            Height = a.HeightFlashCard.GetValueOrDefault(),
                            Width = a.WidthFlashCard.GetValueOrDefault(),
                            Id = Guid.NewGuid().ToString()
                        });
            else
                inlineQueryResults = results.Select(
                    a =>
                        new InlineQueryResultArticle
                        {
                            Title = $"{a.OriginalWord}-{a.Pronunciation}",
                            Id = inlineQuery.Id,
                            InputMessageContent =
                                new InputTextMessageContent
                                {
                                    DisableWebPagePreview = true,
                                    MessageText =
                                        $"<b>{a.OriginalWord}</>{Environment.NewLine}<i>{a.Pronunciation}</i>{Environment.NewLine}{a.Translation}",
                                    ParseMode = ParseMode.Html
                                }
                        });

            await _client.AnswerInlineQueryAsync(inlineQuery.Id, inlineQueryResults.ToArray(), 0);

            _checker.UserQueryProcessed(userId);
        }

        public async Task OnMessage(Message msg)
        {
            try
            {
                var userId = msg.From.Id;
                if (!_checker.AllowUser(userId))
                    return;

                await OnMessage(msg, msg.Text, msg.From);

                _checker.UserQueryProcessed(userId);
            }
            catch (Exception ex)
            {
                MainFactory.Log.Write("Message", ex, null);
            }
        }

        public void OnReceiveError(ApiRequestException e)
        {
            MainFactory.Log.Write(nameof(OnReceiveError), e, null);
        }

        public void OnReceiveGeneralError(Exception e)
        {
            MainFactory.Log.Write(nameof(OnReceiveGeneralError), e, null);
        }

        private async Task HandleArgumentCommand(Message msg, string argumentCommand, long userId)
        {
            var possiblePreviousCommand = _repository.GetUserCommand(userId);

            if (!string.IsNullOrWhiteSpace(possiblePreviousCommand))
            {
                var noEmojiCmd = GetNoEmojiString(argumentCommand) ?? string.Empty;

                var mItem = new MessageItem
                {
                    Command = possiblePreviousCommand,
                    ChatId = msg.Chat.Id,
                    UserId = msg.From.Id,
                    Text = msg.Text,
                    TextOnly = noEmojiCmd.Replace(possiblePreviousCommand, string.Empty)
                };

                if (msg.Document != null)
                {
                    var file = await _client.GetFileAsync(msg.Document.FileId);
                    mItem.FileStream = file.FileStream;
                }

                await HandleCommand(mItem);
            }
        }

        private async Task HandleCommand(MessageItem mItem)
        {
            var handler = MainFactory.GetCommandHandler(mItem.Command);
            var reply = handler.Reply(mItem);

            if (reply.Markup == null)
                reply.Markup = new ReplyKeyboardRemove();

            if (reply.Picture == null)
                await _client.SendTextMessageAsync(mItem.ChatId, reply.Message, ParseMode.Default, true, false, 0,
                    reply.Markup);
            else
                using (var ms = new MemoryStream(reply.Picture.ImageBody))
                {
                    await _client.SendPhotoAsync(mItem.ChatId, new FileToSend("file.jpg", ms), reply.Message, false,
                        0, reply.Markup);
                }
        }

        private async Task OnMessage(Message msg, string argumentCommand, User user)
        {
            if (!_repository.IsUserExist(user.Id))
                _repository.AddUser(new LearnChinese.Data.Ef.User
                {
                    IdUser = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    Mode = EGettingWordsStrategy.Random.ToString()
                });

            var firstEntity = msg.Entities.FirstOrDefault();
            if (firstEntity?.Type == MessageEntityType.BotCommand)
            {
                var commandOnly = msg.Text.Substring(firstEntity.Offset, firstEntity.Length);

                var noEmoji = GetNoEmojiString(msg.Text);

                var textOnly = noEmoji.Replace(commandOnly, string.Empty);
                await HandleCommand(new MessageItem
                {
                    Command = commandOnly,
                    ChatId = msg.Chat.Id,
                    UserId = user.Id,
                    Text = msg.Text,
                    TextOnly = textOnly
                });

                _repository.SetUserCommand(user.Id, commandOnly);
            }
            else
            {
                await HandleArgumentCommand(msg, argumentCommand, user.Id);
            }
        }
    }
}