using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Data.Ef
{
    [Table("Word")]
    public partial class Word : IWord
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Word()
        {
            Scores = new HashSet<Score>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string OriginalWord { get; set; }

        [StringLength(50)]
        public string Pronunciation { get; set; }

        public DateTime LastModified { get; set; }

        [StringLength(250)]
        public string Translation { get; set; }

        public string Usage { get; set; }

        public byte[] CardAll { get; set; }

        public byte[] CardOriginalWord { get; set; }

        public byte[] CardTranslation { get; set; }

        public byte[] CardPronunciation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Score> Scores { get; set; }
        public long IdOwner { get; set; }


        public User UserOwner { get; set; }

        public int SyllablesCount { get; set; }
    }
}
