using System;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces.Data;
using YellowDuck.LearnChinese.Providers;

namespace YellowDuck.LearnChinese.Extentions
{
    public static class MainExtensions
    {
        public static string ToEditString(this IWord word)
        {
            return
                $"{word.OriginalWord}{PinyinChineseWordParseProvider.ImportSeparator1}{word.Pronunciation.Replace("|", string.Empty)}{PinyinChineseWordParseProvider.ImportSeparator1}{word.Translation}";
        }

        public static ELearnMode ToELearnMode(this IScore score)
        {
            ELearnMode learnMode;

            if (!Enum.TryParse(score.LastLearnMode, out learnMode))
                throw new Exception(
                    $"Wrong learn mode has been set. userId={score.IdUser}, learnMode={score.LastLearnMode}");

            return learnMode;
        }
    }
}