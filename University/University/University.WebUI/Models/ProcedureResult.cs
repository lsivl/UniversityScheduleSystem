using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class ProcedureResult
    {
        public int WeekNum { get; set; }
        public string DayOfWeek { get; set; }
        public string LessonTypeName { get; set; }
        public int LessonTimeID { get; set; }
        public string SubjectName { get; set; }
        public int SubjectID { get; set; }
        public string TeacherName { get; set; }
        public int TeacherID { get; set; }
        public string Post { get; set; }
        public string GroupName { get; set; }
        public string StreamName { get; set; }
        public string ClassroomName { get; set; }
        public int ClassroomID { get; set; }
    }
}