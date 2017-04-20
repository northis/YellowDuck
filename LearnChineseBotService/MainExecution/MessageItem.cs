using System.IO;

namespace YellowDuck.LearnChineseBotService.MainExecution
{
    public class MessageItem
    {
        public long ChatId { get; set; }

        public long UserId { get; set; }

        public string Command { get; set; }

        public Stream FileStream { get; set; }

        public string Text { get; set; }

        public string TextOnly { get; set; }
        public byte[] Picture { get; set; }
    }
}
