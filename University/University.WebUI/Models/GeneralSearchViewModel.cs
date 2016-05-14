using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using University.Domain.Models;

namespace University.WebUI.Models
{
    public class GeneralSearchViewModel
    {
        public List<Stream> Streams { get; set; }
        public List<Group> Groups { get; set; }
        public List<Classroom> Classrooms { get; set; }
        public List<Teacher> Teachers { get; set; }
        public List<Cathedra> Cathedras { get; set; }
        public List<Faculty> Faculties { get; set; }
        public List<ClassroomType> ClassroomTypes { get; set; }

    }
}