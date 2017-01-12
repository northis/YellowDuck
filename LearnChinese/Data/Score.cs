using System.ComponentModel.DataAnnotations.Schema;

namespace YellowDuck.LearnChinese.Data
{
    [Table("Score")]
    public  partial class Score
    {
        public long Id { get; set; }

        public long IdUser { get; set; }

        public long IdWord { get; set; }

        public int CheckCount { get; set; }

        public int SuccessCount { get; set; }

        public virtual User User { get; set; }

        public virtual Word Word { get; set; }
    }
}
