using System;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Extentions
{
    public static class MainExtensions
    {
        public static ELearnMode ToELearnMode(this IScore score)
        {
            ELearnMode learnMode;

            if (!Enum.TryParse(score.LastLearnMode, out learnMode))
                throw new Exception($"Задан неверный режим обучения. userId={score.IdUser}, learnMode={score.LastLearnMode}");

            return learnMode;
        }
    }
}
