using System;

namespace YellowDuck.LearnChinese.Data.ObjectModels
{
    public class WordSearchResultItem
    {
        public string OriginalWord { get; set; }
        public string Pronunciation { get; set; }
        public string Translation { get; set; }
        public Guid FileId { get; set; }
    }
}
