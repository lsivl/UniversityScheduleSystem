using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class LessonTime
    {
        public LessonTime()
        {
            this.Times = new List<Time>();
        }

        public int LessonTimeID { get; set; }
        public System.TimeSpan StartTime { get; set; }
        public System.TimeSpan EndTime { get; set; }
        public virtual ICollection<Time> Times { get; set; }
    }
}
