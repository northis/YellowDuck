using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Interfaces.Data;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;
using File = System.IO.File;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class ImportCommand : CommandBase
    {
        private readonly IChineseWordParseProvider _parseProvider;
        private readonly IWordRepository _repository;
        private readonly IFlashCardGenerator _flashCardGenerator;

        public ImportCommand(IChineseWordParseProvider parseProvider, IWordRepository repository, IFlashCardGenerator flashCardGenerator)
        {
            _parseProvider = parseProvider;
            _repository = repository;
            _flashCardGenerator = flashCardGenerator;
        }

        public const uint MaxImportFileSize = 1048576;//1 Мб


        public override AnswerItem Reply(MessageItem mItem)
        {
            var buttons = GetDefaultButtons();

            var loadFileMessage =
                "Загрузите список слов в виде файла .csv в формате <слово/фраза иероглифами>;<перевод>";

            var fileStream = mItem.FileStream;

            if (fileStream == null)
            {
                return new AnswerItem
                {
                    Message = loadFileMessage
                };
            }
            if (fileStream.Length > MaxImportFileSize)
            {
                return new AnswerItem
                {
                    Message =
                        $"Файл не должен быть больше {MaxImportFileSize} байт.{Environment.NewLine}{loadFileMessage}"
                };
            }

            var fileStrings = ReadLines(fileStream, Encoding.UTF8).ToArray();
            var result = _parseProvider.ImportWords(fileStrings, false);

            if (result == null)
                return new AnswerItem
                {
                    Message = $"Файл плохой.{Environment.NewLine}{loadFileMessage}"
                };


            var badWords = new List<string>();
            var goodWords = new List<IWord>();
            foreach (var word in result.SuccessfulWords)
            {
                try
                {
                    word.CardAll = _flashCardGenerator.Generate(word, ELearnMode.FullView);
                    word.CardOriginalWord = _flashCardGenerator.Generate(word, ELearnMode.OriginalWord);
                    word.CardPronunciation = _flashCardGenerator.Generate(word, ELearnMode.Pronunciation);
                    word.CardTranslation = _flashCardGenerator.Generate(word, ELearnMode.Translation);

                    _repository.AddWord(word, mItem.UserId);
                    goodWords.Add(word);
                }
                catch (Exception ex)
                {
                    badWords.Add(word.OriginalWord + ": " + ex.Message);
                }
            }

            badWords.AddRange(result.FailedWords);

            var answer = new AnswerItem
            {
                Message = string.Empty,
                Markup =
                    new ReplyKeyboardMarkup {Keyboard = new[] {buttons}, ResizeKeyboard = true, OneTimeKeyboard = true}
            };

            if (result.SuccessfulWords.Any())
                answer.Message +=
                    $"Успешно добавлены ({goodWords.Count}): {Environment.NewLine} {string.Join(Environment.NewLine, goodWords.Select(a => a.OriginalWord))}{Environment.NewLine}";

            if (badWords.Any())
                answer.Message +=
                    $"есть проблемы  ({badWords.Count}): {Environment.NewLine} {string.Join(Environment.NewLine, badWords)}";

            return answer;
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Import;
        }

        public IEnumerable<string> ReadLines(Stream streamProvider,Encoding encoding)
        {
            using (var stream = streamProvider)
            using (var reader = new StreamReader(stream, encoding))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
