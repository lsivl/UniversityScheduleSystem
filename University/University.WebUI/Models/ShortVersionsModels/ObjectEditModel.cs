using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace University.WebUI.Models.ShortVersionsModels
{
    public class ObjectEditModel
    {
        public string TypeObject { get; set; }
        public int ID { get; set; }
        public bool IsDelete { get; set; }
        public bool IsCreate { get; set; }
        
        public string Name  { get; set; }
        public string ShortName { get; set; }
        public int YearOfStudy { get; set; }
        public int? StudentsCount { get; set; }
        public IEnumerable<SelectListItem> Streams { get; set; }
        public IEnumerable<SelectListItem> Cathedras { get; set; }
        public IEnumerable<SelectListItem> Faculties { get; set; }
        public IEnumerable<SelectListItem> CoursesNumbers { get; set; }
        public IEnumerable<SelectListItem> ClassroomTypes { get; set; }
        public int? StreamID { get; set; }
        public int? CathedraID { get; set; }
        public int? FacultyID { get; set; }
        public int? ClassroomTypeID { get; set; }
        public string TeacherPost { get; set; }
        public string OperationName { get; set; }
        public string Message { get; set; }
        public string DeleteMessage { get; set; }
        public string ChangeFaculty { get; set; }
    }
}