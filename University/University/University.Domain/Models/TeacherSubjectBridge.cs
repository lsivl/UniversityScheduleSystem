using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class TeacherSubjectBridge
    {
        public int TeacherSubjectBridgeID { get; set; }
        public int TeacherID { get; set; }
        public int SubjectID { get; set; }
        public int LessonTypeID { get; set; }
        public virtual LessonType LessonType { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}
