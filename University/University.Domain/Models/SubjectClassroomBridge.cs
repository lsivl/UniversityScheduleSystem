using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class SubjectClassroomBridge
    {
        public int SubjecyClassroomBridgeID { get; set; }
        public int SubjectID { get; set; }
        public int ClassroomID { get; set; }
        public int LessonTypeID { get; set; }
        public virtual Classroom Classroom { get; set; }
        public virtual LessonType LessonType { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
