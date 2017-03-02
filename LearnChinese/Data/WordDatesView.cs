﻿using System;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Data
{

    public class WordDatesView
    {
        public IWord Word { get; set; }
        public DateTime? LastLearned { get; set; }
        public DateTime? LastView { get; set; }
    }
}
