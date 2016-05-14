using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class Time
    {
        public Time()
        {
            this.LessonEvents = new List<LessonEvent>();
        }

        public int TimeID { get; set; }
        public int WeekNum { get; set; }
        public string DayOfWeek { get; set; }
        public int LessonTimeID { get; set; }
        public int DayNumber { get; set; }
        public virtual ICollection<LessonEvent> LessonEvents { get; set; }
        public virtual LessonTime LessonTime { get; set; }
    }
}
