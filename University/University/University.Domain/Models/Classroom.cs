using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class Classroom
    {
        public Classroom()
        {
            this.LessonEvents = new List<LessonEvent>();
            this.StreamSubjectBridges = new List<StreamSubjectBridge>();
            this.SubjectClassroomBridges = new List<SubjectClassroomBridge>();
        }

        public int ClassroomID { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int ClassroomTypeID { get; set; }
        public Nullable<int> CathedraID { get; set; }
        public virtual Cathedra Cathedra { get; set; }
        public virtual ClassroomType ClassroomType { get; set; }
        public virtual ICollection<LessonEvent> LessonEvents { get; set; }
        public virtual ICollection<StreamSubjectBridge> StreamSubjectBridges { get; set; }
        public virtual ICollection<SubjectClassroomBridge> SubjectClassroomBridges { get; set; }
    }
}
