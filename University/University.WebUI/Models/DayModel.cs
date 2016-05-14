using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.Domain.Models;

namespace University.WebUI.Models
{
    public class DayModel
    {
        public string DayName { get; set; } 
        public int DayNumber { get; set; }
        public List<LessonModel> DayLessons { get; set; }
        public int FirstLesson { get; set; }
        public int WeekNum { get; set; }
    }
}