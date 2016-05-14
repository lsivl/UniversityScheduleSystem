using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class ScheduleTeacherClassroomModel
    {
        public string TeacherName { get; set; }
        public int TeacherId { get; set; }
        public string ClassroomName { get; set; }
        public int ClassroomId { get; set; }
        public string CathedraName { get; set; }
        public int CathdraId { get; set; }
        public string Faculty { get; set; }
        public string TypeData { get; set; }
    }
}