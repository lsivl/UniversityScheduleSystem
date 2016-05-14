using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class Subject
    {
        public Subject()
        {
            this.LessonEvents = new List<LessonEvent>();
            this.StreamSubjectBridges = new List<StreamSubjectBridge>();
            this.SubjectClassroomBridges = new List<SubjectClassroomBridge>();
            this.TeacherSubjectBridges = new List<TeacherSubjectBridge>();
        }

        public int SubjectID { get; set; }
        public string Name { get; set; }
        public int CathedraID { get; set; }
        public virtual Cathedra Cathedra { get; set; }
        public virtual ICollection<LessonEvent> LessonEvents { get; set; }
        public virtual ICollection<StreamSubjectBridge> StreamSubjectBridges { get; set; }
        public virtual ICollection<SubjectClassroomBridge> SubjectClassroomBridges { get; set; }
        public virtual ICollection<TeacherSubjectBridge> TeacherSubjectBridges { get; set; }
    }
}
