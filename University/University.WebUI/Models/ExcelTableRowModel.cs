using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class ExcelTableRowModel
    {
        public string Subject { get; set; }
        public int Course { get; set; }
        public int Coupled { get; set; }
        public string Faculty { get; set; }
        public string Cathedra { get; set; }
        public string Stream { get; set; }
        public string GroupsOfStream { get; set; }
        public string UnitedGroups { get; set; }
        public int StudentsCount { get; set; }
        public int LectionHour { get; set; }
        public string LectionTeacher { get; set; }
        public string LectionClassroom { get; set; }
        public int PracticalHour { get; set; }
        public string PracticalTeacher { get; set; }
        public string PracticalClassroom { get; set; }
        public int LabHour { get; set; }
        public string LabTeacher { get; set; }
        public string LabClassroom { get; set; }


        public string ClassroomForDownload { get; set; }
        public int ClassroomCapacity { get; set; }
        public string ClassroomType { get; set; }

    }
}