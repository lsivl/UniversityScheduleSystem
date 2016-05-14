using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class LessonModel
    {
        public int NumberWeek { get; set; }
        public int NumberDay { get; set; }
        public int LessonNumber { get; set; }
        public string Subject { get; set; }
        public int SubjectID { get; set; }
        public string LessonType { get; set; }
        public int LessonTypeID { get; set; }
        public string Teacher { get; set; }
        public int TeacherID { get; set; }
        public string TeacherPost { get; set; }
        public string Classroom { get; set; }
        public int ClassroomID { get; set; }
        public string Group { get; set; }
    }
}