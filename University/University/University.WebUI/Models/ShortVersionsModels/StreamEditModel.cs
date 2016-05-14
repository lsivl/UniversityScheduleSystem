using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace University.WebUI.Models.ShortVersionsModels
{
    public class StreamEditModel
    {
        public int StreamID { get; set; }

        [Required]
        public String StreamName  { get; set; }
        [Required]
        public int YearOfStudy { get; set; }
        public int? StudentsCount { get; set; }
        public IEnumerable<SelectListItem> Faculties { get; set; }
        public IEnumerable<SelectListItem> CoursesNumbers { get; set; }
        public int FacultyID { get; set; }
        public string Message { get; set; }
    }
}