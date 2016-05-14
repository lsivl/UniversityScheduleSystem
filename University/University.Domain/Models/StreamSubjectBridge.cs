using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class StreamSubjectBridge
    {
        public int StreamSubjectBridgeID { get; set; }
        public int StreamID { get; set; }
        public int SubjectID { get; set; }
        public int GroupID { get; set; }
        public int LessonTypeID { get; set; }
        public int TeacherID { get; set; }
        public Nullable<int> ClassroomID { get; set; }
        public int CountHours { get; set; }
        public int Сoupled { get; set; }
        public Nullable<int> CathedraID { get; set; }
        public virtual Cathedra Cathedra { get; set; }
        public virtual Classroom Classroom { get; set; }
        public virtual Group Group { get; set; }
        public virtual LessonType LessonType { get; set; }
        public virtual Stream Stream { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
