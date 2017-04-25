using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class LearnViewCommand : CommandBase
    {
        private readonly IStudyProvider _studyProvider;

        public LearnViewCommand(IStudyProvider studyProvider)
        {
            _studyProvider = studyProvider;
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            var buttons = GetLearnButtons();

            var newWord = _studyProvider.LearnWord(mItem.UserId, ELearnMode.FullView,
                EGettingWordsStrategy.NewMostDifficult);
            
            return new AnswerItem
            {
                Message = "🎓👀",
                Markup = new ReplyKeyboardMarkup {Keyboard = buttons, ResizeKeyboard = true, OneTimeKeyboard = false},
                Picture = newWord.Picture
            };
        }


        public override ECommands GetCommandType()
        {
            return ECommands.LearnView;
        }
        public override string GetCommandDescription()
        {
            return "🎓👀Просматривать слова";
        }
    }
}
