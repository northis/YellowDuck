using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YellowDuck.LearnChinese.Data
{
    [Table("Word")]
    public partial class Word
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Word()
        {
            Scores = new HashSet<Score>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ChineseWord { get; set; }

        [StringLength(50)]
        public string PinyinWord { get; set; }

        public DateTime DateAdded { get; set; }

        [StringLength(250)]
        public string TranslationNative { get; set; }

        [StringLength(250)]
        public string TranslationEng { get; set; }

        public string Usage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Score> Scores { get; set; }
    }
}
