using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class Stream
    {
        public Stream()
        {
            this.Groups = new List<Group>();
            this.LessonEvents = new List<LessonEvent>();
            this.StreamSubjectBridges = new List<StreamSubjectBridge>();
            this.UnitedGroups = new List<UnitedGroup>();
        }

        public int StreamID { get; set; }
        public string StreamName { get; set; }
        public int YearOfStudy { get; set; }
        public Nullable<int> FacultyID { get; set; }
        public Nullable<int> StudentsCount { get; set; }
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<LessonEvent> LessonEvents { get; set; }
        public virtual ICollection<StreamSubjectBridge> StreamSubjectBridges { get; set; }
        public virtual ICollection<UnitedGroup> UnitedGroups { get; set; }
    }
}
