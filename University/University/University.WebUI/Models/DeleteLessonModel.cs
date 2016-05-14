using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class DeleteLessonModel
    {
        public string ParrentTableNode { get; set; }
        public List<string> TdIds { get; set; }
        public int LessonNumber { get; set; }

    }
}