using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class GeneralInformation
    {
        public string typeInformation { get; set; }


        public string FacultyName { get; set; }
        public string FullFacultyName { get; set; }
        public int FacultyID { get; set; }
        public string CathedraName { get; set; }
        public string FullCathedraName { get; set; }
        public int CathedraID { get; set; }
        public string StreamName { get; set; }
        public int StreamID { get; set; }
        public string GroupName { get; set; }
        public int GroupID { get; set; }
        public string TeacherName { get; set; }
        public string TeacherPost { get; set; }
        public int TeacherID { get; set; }
        public string ClassroomName { get; set; }
        public string ClassrommType { get; set; }
        public int ClassroomID { get; set; }


        public List<string> FacultyDepartments { get; set; }
        public List<string> StreamGroups { get; set; }

        public int StreamYearOfStudy { get; set; }
        public int StudentsCount { get; set; }
        public int ClassroomCapacity { get; set; }
    }
}