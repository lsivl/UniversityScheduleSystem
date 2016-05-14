using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class SessionLessonEvent
    {
        public int GroupId { get; set; }
        public int LessonTypeId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public int ClassroomId { get; set; }
    }
}