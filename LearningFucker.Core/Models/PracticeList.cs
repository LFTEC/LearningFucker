using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFucker.Models
{
    public class PracticeList
    {
        public bool CanJoin { get; set; }
        public int count { get; set; }

        public List<CWeekList> list { get; set; }

    }

    public class CWeekList
    {
        public int Month { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }

        public List<CWeek> WeekList { get; set; }
    }

    public class CWeek
    {
        public decimal Integral { get; set; }
        public bool IsLastWeek { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public string State { get; set; }
    }
}
