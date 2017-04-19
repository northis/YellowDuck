using System.IO;

namespace YellowDuck.LearnChineseBotService.MainExecution
{
    public class MessageItem
    {
        public long ChatId { get; set; }

        public long UserId { get; set; }

        public string Command { get; set; }

        public Stream FileStream { get; set; }
    }
}
