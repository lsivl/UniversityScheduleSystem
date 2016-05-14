using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class ScheduleModel
    {
        public string TeacherName { get; set; }
        public int TeacherId { get; set; }
        public string FacultyName { get; set; }
        public int FacultyId { get; set; }
        public string CathdraName { get; set; }
        public int CathedraID { get; set; }
        public string StreamName { get; set; }
        public string GroupName { get; set; }
        public string TypeData { get; set; }
        public int TypeDataNumber { get; set; }

        public string ClassroomName { get; set; }
        public int ClassroomId { get; set; }
        public int StreamId { get; set; }
    }
}