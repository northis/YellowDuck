using System;
using System.IO;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using File = System.IO.File;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class ImportCommand : CommandBase
    {
        private readonly IChineseWordParseProvider _parseProvider;

        public ImportCommand(IChineseWordParseProvider parseProvider)
        {
            _parseProvider = parseProvider;
        }

        public const uint MaxImportFileSize = 1048576;//1 Мб


        public override AnswerItem Reply(Message msg)
        {
            var buttons = GetDefaultButtons();

            var loadFileMessage =
                "Загрузите список слов в виде файла .csv в формате <слово/фраза иероглифами>;<перевод>";

            if (msg.Document == null)
            {
                return new AnswerItem
                {
                    Message = loadFileMessage
                };
            }
            if(msg.Document.FileSize > MaxImportFileSize)
            {
                return new AnswerItem
                {
                    Message = $"Файл не должен быть больше {MaxImportFileSize} байт.{Environment.NewLine}{loadFileMessage}"
                };
            }
            var fileStrings = File.ReadAllLines(msg.Document.FilePath, Encoding.UTF8);
            var result = _parseProvider.ImportWords(fileStrings, false);

            if (result == null)
                return new AnswerItem
                {
                    Message = $"Файл плохой.{Environment.NewLine}{loadFileMessage}"
                };

            return new AnswerItem
            {
                Message =
                    $"Успешно добавлены ({result.SuccessfulWords.Length}): {Environment.NewLine} {string.Join(Environment.NewLine, result.SuccessfulWords.Select(a => a.OriginalWord))}{Environment.NewLine}" +
                    $"не удалось распознать  ({result.FailedWords.Length}): {Environment.NewLine} {string.Join(Environment.NewLine, result.FailedWords)}",
                Markup =
                    new ReplyKeyboardMarkup {Keyboard = new[] {buttons}, ResizeKeyboard = true, OneTimeKeyboard = true}
            };
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Import;
        }
    }
}
