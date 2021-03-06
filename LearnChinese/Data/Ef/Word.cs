using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Data.Ef
{
    [Table("Word")]
    public class Word : IWord
    {
        [NotMapped] private GenerateImageResult _cardAll;

        [NotMapped] private GenerateImageResult _cardOriginalWord;

        [NotMapped] private GenerateImageResult _cardPronunciation;

        [NotMapped] private GenerateImageResult _cardTranslation;

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Word()
        {
            Scores = new HashSet<Score>();
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Score> Scores { get; set; }

        public long IdOwner { get; set; }


        public User UserOwner { get; set; }

        public virtual WordFileA WordFileA { get; set; }

        public virtual WordFileO WordFileO { get; set; }

        public virtual WordFileP WordFileP { get; set; }

        public virtual WordFileT WordFileT { get; set; }

        [NotMapped]
        public GenerateImageResult CardAll
        {
            get => _cardAll ?? (_cardAll = new GenerateImageResult
            {
                ImageBody = WordFileA?.Bytes,
                Height = WordFileA?.Height,
                Width = WordFileA?.Width
            });
            set
            {
                if (WordFileA == null)
                {
                    WordFileA = new WordFileA {Bytes = value.ImageBody, Height = value.Height, Width = value.Width};
                }
                else
                {
                    WordFileA.Bytes = value.ImageBody;
                    WordFileA.Height = value.Height;
                    WordFileA.Width = value.Width;
                }

                _cardAll = value;
            }
        }


        [NotMapped]
        public GenerateImageResult CardOriginalWord
        {
            get => _cardOriginalWord ?? (_cardOriginalWord = new GenerateImageResult
            {
                ImageBody = WordFileO?.Bytes,
                Height = WordFileO?.Height,
                Width = WordFileO?.Width
            });
            set
            {
                if (WordFileO == null)
                {
                    WordFileO = new WordFileO {Bytes = value.ImageBody, Height = value.Height, Width = value.Width};
                }
                else
                {
                    WordFileO.Bytes = value.ImageBody;
                    WordFileO.Height = value.Height;
                    WordFileO.Width = value.Width;
                }
                _cardOriginalWord = value;
            }
        }


        [NotMapped]
        public GenerateImageResult CardPronunciation
        {
            get => _cardPronunciation ?? (_cardPronunciation = new GenerateImageResult
            {
                ImageBody = WordFileP?.Bytes,
                Height = WordFileP?.Height,
                Width = WordFileP?.Width
            });
            set
            {
                if (WordFileP == null)
                {
                    WordFileP = new WordFileP {Bytes = value.ImageBody, Height = value.Height, Width = value.Width};
                }
                else
                {
                    WordFileP.Bytes = value.ImageBody;
                    WordFileP.Height = value.Height;
                    WordFileP.Width = value.Width;
                }
                _cardPronunciation = value;
            }
        }


        [NotMapped]
        public GenerateImageResult CardTranslation
        {
            get => _cardTranslation ?? (_cardTranslation = new GenerateImageResult
            {
                ImageBody = WordFileT?.Bytes,
                Height = WordFileT?.Height,
                Width = WordFileT?.Width
            });
            set
            {
                if (WordFileT == null)
                {
                    WordFileT = new WordFileT {Bytes = value.ImageBody, Height = value.Height, Width = value.Width};
                }
                else
                {
                    WordFileT.Bytes = value.ImageBody;
                    WordFileT.Height = value.Height;
                    WordFileT.Width = value.Width;
                }
                _cardTranslation = value;
            }
        }

        public long Id { get; set; }

        public DateTime LastModified { get; set; }

        [Required]
        [StringLength(50)]
        public string OriginalWord { get; set; }

        [StringLength(50)]
        public string Pronunciation { get; set; }

        public int SyllablesCount { get; set; }

        [StringLength(250)]
        public string Translation { get; set; }

        public string Usage { get; set; }

        public void CleanCards()
        {
            _cardAll = null;
            _cardOriginalWord = null;
            _cardTranslation = null;
            _cardPronunciation = null;
        }
    }
}