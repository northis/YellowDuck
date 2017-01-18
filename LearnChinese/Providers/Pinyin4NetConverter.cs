using System;
using System.Linq;
using System.Text.RegularExpressions;
using Pinyin4net;
using Pinyin4net.Format;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;

namespace YellowDuck.LearnChinese.Providers
{
    public class Pinyin4NetConverter : IChinesePinyinConverter
    {
        #region Fields

        public const string PinyinExcludeRegexPattern = "[^a-zāáǎàēéěèīíǐìōóǒòūúǔùǖǘǚǜ]";
        public const string PinyinVowelOnlyRegexPattern = "[āáǎàēéěèīíǐìōóǒòūúǔùǖǘǚǜaeiouü]";

        public const string FirstTonePattern = "[āēīōūǖ]";
        public const string SecondTonePattern = "[áéíóúǘ]";
        public const string ThirdTonePattern = "[ǎěǐǒǔǚ]";
        public const string ForthTonePattern = "[àèìòùǜ]";

        public const ushort TonesTotalCount = 5;

        public const string PatternA = "[āáǎàa]";
        public const string PatternE = "[ēéěèe]";
        public const string PatternI = "[īíǐìi]";
        public const string PatternO = "[ōóǒòo]";
        public const string PatternU = "[ūúǔùu]";
        public const string PatternÜ = "[ǖǘǚǜü]";

        #endregion

        #region Methods

        public string[] Convert(char chineseCharacter, EToneType toneType)
        {
            var format = new HanyuPinyinOutputFormat
            {
                CaseType = HanyuPinyinCaseType.LOWERCASE,
                ToneType = HanyuPinyinToneType.WITH_TONE_MARK,
                VCharType = HanyuPinyinVCharType.WITH_U_UNICODE
            };

            switch (toneType)
            {
                case EToneType.Without:
                    format.ToneType = HanyuPinyinToneType.WITHOUT_TONE;
                    break;

                case EToneType.Number:
                    format.ToneType = HanyuPinyinToneType.WITH_TONE_NUMBER;
                    break;
            }
            
            return PinyinHelper.ToHanyuPinyinStringArray(chineseCharacter, format);
        }

        public bool AreMarkPinyinSameIgnoreTone(string pinyin1, string pinyin2)
        {
            return ToSyllablesWithoutTone(pinyin1) == ToSyllablesWithoutTone(pinyin2);
        }

        string ToSyllablesWithoutTone(string syllableMarkTone)
        {
            syllableMarkTone = syllableMarkTone.ToLower();
            syllableMarkTone = Regex.Replace(syllableMarkTone, PinyinExcludeRegexPattern, string.Empty);

            syllableMarkTone = Regex.Replace(syllableMarkTone, PatternA, "a");
            syllableMarkTone = Regex.Replace(syllableMarkTone, PatternE, "e");
            syllableMarkTone = Regex.Replace(syllableMarkTone, PatternI, "i");
            syllableMarkTone = Regex.Replace(syllableMarkTone, PatternO, "o");
            syllableMarkTone = Regex.Replace(syllableMarkTone, PatternU, "u");
            syllableMarkTone = Regex.Replace(syllableMarkTone, PatternÜ, "ü");

            return syllableMarkTone;
        }

        public string ToSyllableNumberTone(string syllableMarkTone)
        {
            var toneNumber = 5;

            if (Regex.Match(syllableMarkTone, FirstTonePattern).Success)
                toneNumber = 1;
            else if (Regex.Match(syllableMarkTone, SecondTonePattern).Success)
                toneNumber = 2;
            else if (Regex.Match(syllableMarkTone, ThirdTonePattern).Success)
                toneNumber = 3;
            else if (Regex.Match(syllableMarkTone, ForthTonePattern).Success)
                toneNumber = 4;

            return ToSyllablesWithoutTone(syllableMarkTone) + toneNumber;
        }

        string[] GetAllTonesBucket(string syllableMarkTone, string patternLetter)
        {
            var result = new string[TonesTotalCount];

            var match = Regex.Match(syllableMarkTone, patternLetter);
            if (match.Success)
            {
                var lettersOnly = patternLetter.Replace("[", "").Replace("]", "");
                var lettersOnlyLength = lettersOnly.Length;

                if (lettersOnlyLength < TonesTotalCount)
                    throw new Exception($"Число букв в паттерне [{patternLetter}] меньше заданного количества тонов ({TonesTotalCount})");

                for (var i = 0; i < lettersOnlyLength; i++)
                {
                    var vowelOption = patternLetter.Replace("[", "").Replace("]", "")[i];
                    result[i] = syllableMarkTone.Replace(syllableMarkTone[match.Index], vowelOption);
                }
            }

            return result;
        }

        public string[] ToSyllablesAllTones(string syllableMarkTone)
        {
            var result = GetAllTonesBucket(syllableMarkTone, PatternA);
            if (result != null && result.Count(a => a != null) > 0)
                return result;

            result = GetAllTonesBucket(syllableMarkTone, PatternE);
            if (result != null && result.Count(a => a != null) > 0)
                return result;

            result = GetAllTonesBucket(syllableMarkTone, PatternI);
            if (result != null && result.Count(a => a != null) > 0)
                return result;

            result = GetAllTonesBucket(syllableMarkTone, PatternO);
            if (result != null && result.Count(a => a != null) > 0)
                return result;

            result = GetAllTonesBucket(syllableMarkTone, PatternU);
            if (result != null && result.Count(a => a != null) > 0)
                return result;

            result = GetAllTonesBucket(syllableMarkTone, PatternÜ);
            return result;
        }

        #endregion
    }
}
