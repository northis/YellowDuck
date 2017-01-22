﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using YellowDuck.LearnChinese.Data;
using YellowDuck.LearnChinese.Data.Ef;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Interfaces.Data;

// ReSharper disable LoopCanBeConvertedToQuery

namespace YellowDuck.LearnChinese.Providers
{
    public sealed class PinyinChineseWordParseProvider : IChineseWordParseProvider
    {
        #region Constructors

        public PinyinChineseWordParseProvider(ISyllableColorProvider syllableColorProvider, IChinesePinyinConverter chinesePinyinConverter, ISyllablesToStringConverter syllablesToStringConverter)
        {
            _syllableColorProvider = syllableColorProvider;
            _chinesePinyinConverter = chinesePinyinConverter;
            _syllablesToStringConverter = syllablesToStringConverter;
        }

        #endregion

        #region Fields

        public const ushort MaxSyllablesToParse = 10;
        public const string ImportSeparator= ";";
        public const string PinyinExludeRegexPattern = "[^a-zāáǎàēéěèīíǐìōóǒòūúǔùǖǘǚǜ]";
        private readonly ISyllableColorProvider _syllableColorProvider;
        private readonly IChinesePinyinConverter _chinesePinyinConverter;
        private readonly ISyllablesToStringConverter _syllablesToStringConverter;

        #endregion

        #region Methods
        
        Syllable GetSyllable(char charWord, string pinyinWithMark)
        {
            return new Syllable(charWord, pinyinWithMark,
                _syllableColorProvider.GetSyllableColor(charWord, _chinesePinyinConverter.ToSyllableNumberTone(pinyinWithMark)));
        }


        public Syllable[] GetOrderedSyllables(string word)
        {
            return GetOrderedSyllables(word, EToneType.Mark);
        }

        bool IsChineseCharacter(char character)
        {
            return character >= 0x4e00 && character <= 0x9fbb;
        }

        public Syllable[] GetOrderedSyllables(IWord word)
        {
            var syllableStrings = _syllablesToStringConverter.Parse(word.Pronunciation);
            if (syllableStrings == null)
                return null;

            var syllArray = syllableStrings.ToArray();

            var sylls = new List<Syllable>();
            var chineseOnly = word.OriginalWord.Where(IsChineseCharacter).ToArray();

            for (var i = 0; i < chineseOnly.Length; i++)
            {
                var character = chineseOnly[i];
                sylls.Add(GetSyllable(character, syllArray[i]));
            }
            return sylls.ToArray();
        }

        /// <summary>
        /// Строит верный слог для иероглифа и латинского представления слога
        /// </summary>
        /// <param name="chineseChar">Китайский иероглиф, например, 电</param>
        /// <param name="pinyinWithNumber">Представление в латинском алфавите с тоном цифрой, например, dian4</param>
        /// <returns>Верный слог, содержащий латинское представление с тоном сверху, например, diàn</returns>
        public Syllable BuildSyllable(char chineseChar, string pinyinWithNumber)
        {
            var pinyinStringArray = _chinesePinyinConverter.Convert(chineseChar, EToneType.Number);
            if (pinyinStringArray == null || pinyinStringArray.Length == 0)
                return null;

            var rightSyllableIndex = pinyinStringArray
                    .TakeWhile(a => a != pinyinWithNumber)
                    .Count();
            
            var outPinyin =  _chinesePinyinConverter.Convert(chineseChar, EToneType.Mark)[rightSyllableIndex];

            return GetSyllable(chineseChar, outPinyin);
        }

        /// <summary>
        /// Формат для импорта слов: Иероглиф[ImportSeparator]Пининь[ImportSeparator]Перевод, либо в случае usePinyin=false
        /// Иероглиф[ImportSeparator]Перевод. 
        /// Например, 电;diàn;электричество (usePinyin=true, ImportSeparator=";")
        /// Например, 电;электричество (usePinyin=false, ImportSeparator=";")
        /// </summary>
        /// <param name="rawWords">Массив строк для импорта</param>
        /// <param name="usePinyin">Флаг, использовать ли пининь из импортируемых строк</param>
        /// <returns>Результат из распознанных и нераспознанных слов</returns>
        public ImportWordResult ImportWords(string[] rawWords, bool usePinyin)
        {
            var goodWords = new List<Word>();
            var badWords = new List<string>();

            foreach (var word in rawWords)
            {
                var arrayToParse = word.Split(new[] {ImportSeparator}, StringSplitOptions.RemoveEmptyEntries);
                if (arrayToParse.Length < 2)
                {
                    badWords.Add(word);
                    continue;
                }

                var mainWord = arrayToParse[0];

                var translationIndex = usePinyin ? 2 : 1;
                var translationNative = string.Join(ImportSeparator, arrayToParse.Skip(translationIndex));

                var syllables = GetOrderedSyllables(mainWord, EToneType.Mark);
                
                var separatedSyllables = _syllablesToStringConverter.Join(syllables.Select(a => a.Pinyin));

                var solidSyllables = separatedSyllables.Replace(_syllablesToStringConverter.GetSeparator(),
                    string.Empty);

                if (usePinyin)
                {
                    var pinyin = Regex.Replace(arrayToParse[1].ToLower(), PinyinExludeRegexPattern, string.Empty);

                    if (pinyin.Contains(solidSyllables))
                    {
                        goodWords.Add(new Word
                        {
                            OriginalWord = mainWord,
                            Pronunciation = separatedSyllables,
                            Translation = translationNative
                        });
                    }
                    else if (syllables.Length > MaxSyllablesToParse)
                    {
                        badWords.Add(word +
                                     $" (Слишком длинная строка. Максимальное количество слогов - {MaxSyllablesToParse}.)");
                    }
                    else
                    {
                        var leftPinyin = pinyin;
                        var successFlag = false;

                        var importedSyllables = new List<Syllable>();

                        foreach (var syllable in syllables.Reverse())
                        {
                            successFlag = false;
                            var chineseChar = syllable.ChineseChar;

                            foreach (var pinyinOption in _chinesePinyinConverter.Convert(chineseChar, EToneType.Mark))
                            {
                                var allTones = _chinesePinyinConverter.ToSyllablesAllTones(pinyinOption);

                                foreach (var tone in allTones.Where(a => a != null))
                                {
                                    if (leftPinyin.EndsWith(tone))
                                    {
                                        leftPinyin = leftPinyin.Remove(leftPinyin.Length - tone.Length,
                                            tone.Length);

                                        successFlag = true;
                                        importedSyllables.Insert(0, GetSyllable(chineseChar, tone));
                                        break;
                                    }
                                }

                                if (successFlag)
                                    break;
                            }

                            if (!successFlag)
                                break;
                        }

                        if (successFlag)
                        {
                            goodWords.Add(new Word
                            {
                                OriginalWord = mainWord,
                                Pronunciation =
                                    _syllablesToStringConverter.Join(importedSyllables.Select(a => a.Pinyin)),
                                Translation = translationNative
                            });
                        }
                        else
                        {
                            badWords.Add(word + " (Пиньинь не полностью соответствует иероглифам.)");
                        }
                    }
                    continue;
                }

                goodWords.Add(new Word
                {
                    OriginalWord = mainWord,
                    Pronunciation = separatedSyllables,
                    Translation = translationNative
                });
            }

            return new ImportWordResult(goodWords.ToArray(), badWords.ToArray());
        }

        Syllable[] GetOrderedSyllables(string word, EToneType format)
        {
            var outSyllables = new List<Syllable>();

            foreach (var charWord in word)
            {
                var pinyinStringArray = _chinesePinyinConverter.Convert(charWord, format);
                if (pinyinStringArray == null || pinyinStringArray.Length == 0)
                    continue;

                var pinyinWithMark = pinyinStringArray[0];

                var syllable = GetSyllable(charWord, pinyinWithMark);
                outSyllables.Add(syllable);
            }

            return outSyllables.ToArray();
        }

        #endregion
    }
}