using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Data.Ef
{
    [Table("Score")]
    public class Score : IScore
    {
        public long Id { get; set; }

        public virtual User User { get; set; }

        public virtual Word Word { get; set; }

        public long IdUser { get; set; }

        public long IdWord { get; set; }
        public bool IsInLearnMode { get; set; }
        public DateTime? LastLearned { get; set; }

        [StringLength(50)]
        public string LastLearnMode { get; set; }

        public DateTime LastView { get; set; }

        public int? OriginalWordCount { get; set; }

        public int? OriginalWordSuccessCount { get; set; }
        public int? PronunciationCount { get; set; }

        public int? PronunciationSuccessCount { get; set; }
        public short? RightAnswerNumber { get; set; }
        public int? TranslationCount { get; set; }

        public int? TranslationSuccessCount { get; set; }
        public int? ViewCount { get; set; }
    }
}