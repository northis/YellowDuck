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
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.LayoutRoot;
using User = YellowDuck.LearnChinese.Data.Ef.User;

namespace YellowDuck.LearnChineseBotService.MainExecution
{
    public class QueryHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IWordRepository _repository;
        private readonly string _flashCardUrl;

        public QueryHandler(TelegramBotClient client, IWordRepository repository, string flashCardUrl)
        {
            _client = client;
            _repository = repository;
            _flashCardUrl = flashCardUrl;
        }

        public async Task InlineQuery(InlineQuery inlineQuery)
        {
            var userId = inlineQuery.From.Id;
            var q = inlineQuery.Query;

            var results =_repository.FindFlashCard(q, userId).ToArray();

            if (!results.Any())
            {
                return;
            }

            IEnumerable<InlineQueryResult> inlineQueryResults;

            if (MainFactory.UseWebhooks)
            {
                inlineQueryResults = results.Select(
                    a =>
                        new InlineQueryResultPhoto
                        {
                            Title = $"{a.OriginalWord}-{a.Pronunciation}-{a.Translation}",
                            Url = _flashCardUrl + a.FileId.ToString(),
                            ThumbUrl = _flashCardUrl + a.FileId.ToString(),Height = 200,Width = 200,
                            Id = inlineQuery.Id,
                        });
            }
            else
            {
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

                                },
                            Url = MainFactory.UseWebhooks ? _flashCardUrl + a.FileId.ToString() : null
                        });
            }

            await _client.AnswerInlineQueryAsync(inlineQuery.Id, inlineQueryResults.ToArray());

        }

        public async Task CallbackQuery(CallbackQuery callbackQuery)
        {
            try
            {
                await OnMessage(callbackQuery.Message, callbackQuery.Data, callbackQuery.From);
            }
            catch (Exception ex)
            {
                MainFactory.Log.Write("CallbackQuery", ex, null);
            }
        }

        public async Task OnMessage(Message msg)
        {
            try
            {
                await OnMessage(msg, msg.Text, msg.From);
            }
            catch (Exception ex)
            {
                MainFactory.Log.Write("Message", ex, null);
            }
        }

        public void OnReceiveGeneralError(Exception e)
        {
            MainFactory.Log.Write(nameof(OnReceiveGeneralError), e, null);
        }

        public void OnReceiveError(ApiRequestException e)
        {
            MainFactory.Log.Write(nameof(OnReceiveError), e, null);
        }

        async Task OnMessage(Message msg, string argumentCommand, Telegram.Bot.Types.User user)
        {
            if (!_repository.IsUserExist(user.Id))
                _repository.AddUser(new User
                {
                    IdUser = user.Id,
                    Name = $"{user.FirstName} {user.LastName}"
                });

            var firstEntity = msg.Entities.FirstOrDefault();
            if (firstEntity?.Type == MessageEntityType.BotCommand)
            {
                var commandOnly = msg.Text.Substring(firstEntity.Offset, firstEntity.Length);

                var noEmoji = GetNoEmojiString(msg.Text);

                var textOnly = noEmoji.Replace(commandOnly,string.Empty);
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

        async Task HandleArgumentCommand(Message msg, string argumentCommand, long userId)
        {
            var possiblePreviousCommand = _repository.GetUserCommand(userId);

            if (!string.IsNullOrWhiteSpace(possiblePreviousCommand))
            {
                var noEmojiCmd = GetNoEmojiString(argumentCommand);

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

        async Task HandleCommand(MessageItem mItem)
        { 
            var handler = MainFactory.GetCommandHandler(mItem.Command);
            var reply = handler.Reply(mItem);

            if (reply.Picture == null)
            {
                await _client.SendTextMessageAsync(mItem.ChatId, reply.Message, true, false, 0, reply.Markup);
            }
            else
            {
                using (var ms = new MemoryStream(reply.Picture))
                {
                   await _client.SendPhotoAsync(mItem.ChatId, new FileToSend(Guid.NewGuid() + ".png", ms), reply.Message, false,
                        0, reply.Markup);
                }

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

                if(cat != UnicodeCategory.OtherSymbol && cat != UnicodeCategory.Surrogate && cat != UnicodeCategory.NonSpacingMark)
                    noEmojiStr.Append(chr);
            }

            return noEmojiStr.ToString();
        }
    }

}
