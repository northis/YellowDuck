using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YellowDuck.LearnChinese.Data.ObjectModels;
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

        [NotMapped]
        private GenerateImageResult _cardAll;
        [NotMapped]
        private GenerateImageResult _cardOriginalWord;
        [NotMapped]
        private GenerateImageResult _cardTranslation;
        [NotMapped]
        private GenerateImageResult _cardPronunciation;

        [NotMapped]
        public GenerateImageResult CardAll
        {
            get
            {
                return _cardAll ?? (_cardAll = new GenerateImageResult
                {
                    ImageBody = WordFileA?.Bytes,
                    Height = WordFileA?.Height,
                    Width = WordFileA?.Width
                });
            }
            set
            {
                WordFileA = new WordFileA {Bytes = value.ImageBody, Height = value.Height, Width = value.Width};
                _cardAll = value;
            }
        }


        [NotMapped]
        public GenerateImageResult CardOriginalWord
        {
            get
            {
                return _cardOriginalWord ?? (_cardOriginalWord = new GenerateImageResult
                {
                    ImageBody = WordFileO?.Bytes,
                    Height = WordFileO?.Height,
                    Width = WordFileO?.Width
                });
            }
            set
            {
                WordFileO = new WordFileO { Bytes = value.ImageBody, Height = value.Height, Width = value.Width };
                _cardOriginalWord = value;
            }
        }


        [NotMapped]
        public GenerateImageResult CardTranslation
        {
            get
            {
                return _cardTranslation ?? (_cardTranslation = new GenerateImageResult
                {
                    ImageBody = WordFileT?.Bytes,
                    Height = WordFileT?.Height,
                    Width = WordFileT?.Width
                });
            }
            set
            {
                WordFileT = new WordFileT { Bytes = value.ImageBody, Height = value.Height, Width = value.Width };
                _cardTranslation = value;
            }
        }


        [NotMapped]
        public GenerateImageResult CardPronunciation
        {
            get
            {
                return _cardPronunciation ?? (_cardPronunciation = new GenerateImageResult
                {
                    ImageBody = WordFileP?.Bytes,
                    Height = WordFileP?.Height,
                    Width = WordFileP?.Width
                });
            }
            set
            {
                WordFileP = new WordFileP { Bytes = value.ImageBody, Height = value.Height, Width = value.Width };
                _cardPronunciation = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Score> Scores { get; set; }
        public long IdOwner { get; set; }


        public User UserOwner { get; set; }

        public int SyllablesCount { get; set; }

        public virtual WordFileA WordFileA { get; set; }

        public virtual WordFileO WordFileO { get; set; }

        public virtual WordFileP WordFileP { get; set; }

        public virtual WordFileT WordFileT { get; set; }
    }
}
