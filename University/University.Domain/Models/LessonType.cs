using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class LessonType
    {
        public LessonType()
        {
            this.LessonEvents = new List<LessonEvent>();
            this.StreamSubjectBridges = new List<StreamSubjectBridge>();
            this.SubjectClassroomBridges = new List<SubjectClassroomBridge>();
            this.TeacherSubjectBridges = new List<TeacherSubjectBridge>();
        }

        public int LessonTypeID { get; set; }
        public string LessonTypeName { get; set; }
        public virtual ICollection<LessonEvent> LessonEvents { get; set; }
        public virtual ICollection<StreamSubjectBridge> StreamSubjectBridges { get; set; }
        public virtual ICollection<SubjectClassroomBridge> SubjectClassroomBridges { get; set; }
        public virtual ICollection<TeacherSubjectBridge> TeacherSubjectBridges { get; set; }
    }
}
