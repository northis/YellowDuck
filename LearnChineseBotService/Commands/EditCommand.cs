using System;
using System.Diagnostics;
using System.Linq;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Extentions;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class EditCommand : CommandBase
    {
        private readonly IWordRepository _repository;
        private readonly IChineseWordParseProvider _parseProvider;
        private readonly ImportCommand _importCommand;
        private readonly IFlashCardGenerator _flashCardGenerator;
        public const string EditCmd = "edit";
        public const string EditCmdSeparator = "=";

        public EditCommand(IWordRepository repository, IChineseWordParseProvider parseProvider, ImportCommand importCommand, IFlashCardGenerator flashCardGenerator)
        {
            _repository = repository;
            _parseProvider = parseProvider;
            _importCommand = importCommand;
            _flashCardGenerator = flashCardGenerator;
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            var message = new AnswerItem
            {
                Message = GetCommandIconUnicode()
            };

            try
            {
                var idUser = mItem.ChatId;

                if (string.IsNullOrEmpty(mItem.TextOnly))
                {
                    message.Message = "Type a word to edit. Use chinese characters only!";
                }
                else if (mItem.TextOnly.StartsWith(EditCmd))
                {
                    if (!long.TryParse(
                        mItem.TextOnly.Split(new[] {EditCmdSeparator}, StringSplitOptions.RemoveEmptyEntries)
                            .LastOrDefault(),
                        out long idWord))
                    {
                        message.Message += "I can not edit your word now, sorry.";
                        return message;
                    }

                    var lastWord = _repository.GetUserWordStatistic(idUser, idWord);
                        //_repository.GetCurrentUserWordStatistic(idUser);

                    if (lastWord == null)
                    {
                        message.Message += "No words to edit";
                        return message;
                    }
                    message.Message = lastWord.Word.ToEditString();
                }
                else if (mItem.TextOnly.Length > ImportCommand.MaxImportRowLength)
                {
                    message.Message =
                        $"Too much characters. Maximum is {ImportCommand.MaxImportRowLength}";
                }
                else 
                {
                    var editedText = new[] {mItem.TextOnly};
                    var usePinYin = _importCommand.GetUsePinyin(editedText);

                    if (usePinYin == null)
                    {
                        var wordToFind = _repository.GetWord(mItem.TextOnly);

                        if (wordToFind == null)
                        {

                            message.Message += "Bad text";
                            return message;
                        }

                        message.Message = wordToFind.ToEditString();
                        return message;
                    }

                    var word = _parseProvider.ImportWords(editedText, usePinYin.Value);
                    var parsedWord = word.SuccessfulWords.FirstOrDefault();

                    if (parsedWord == null)
                    {
                        message.Message += word.FailedWords.FirstOrDefault();
                        return message;
                    }

                    parsedWord.CardAll = _flashCardGenerator.Generate(parsedWord, ELearnMode.FullView);
                    parsedWord.CardOriginalWord = _flashCardGenerator.Generate(parsedWord, ELearnMode.OriginalWord);
                    parsedWord.CardPronunciation = _flashCardGenerator.Generate(parsedWord, ELearnMode.Pronunciation);
                    parsedWord.CardTranslation = _flashCardGenerator.Generate(parsedWord, ELearnMode.Translation);

                    _repository.EditWord(parsedWord);
                    message.Message += "The word has been updated";
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                message.Message += ex.Message;
            }
            return message;
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Edit;
        }

        public override string GetCommandIconUnicode()
        {
            return "🖌";
        }

        public override string GetCommandTextDescription()
        {
            return "Edit an existing chinese word";
        }
    }
}
