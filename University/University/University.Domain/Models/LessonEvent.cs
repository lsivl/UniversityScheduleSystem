using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class LessonEvent
    {
        public int LessonEventID { get; set; }
        public int TimeID { get; set; }
        public int SubjectID { get; set; }
        public int LessonTypeID { get; set; }
        public int TeacherID { get; set; }
        public int GroupID { get; set; }
        public int StreamID { get; set; }
        public int ClassroomID { get; set; }
        public virtual Classroom Classroom { get; set; }
        public virtual Group Group { get; set; }
        public virtual LessonType LessonType { get; set; }
        public virtual Stream Stream { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual Time Time { get; set; }
    }
}
