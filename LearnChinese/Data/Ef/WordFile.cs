using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YellowDuck.LearnChinese.Data.Ef
{
    public abstract class WordFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long IdWord { get; set; }

        public Guid Id { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        public byte[] Bytes { get; set; }

        public virtual Word Word { get; set; }


        public int? Height { get; set; }
        public int? Width { get; set; }
    }
}