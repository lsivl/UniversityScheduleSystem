using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class Group
    {
        public Group()
        {
            this.LessonEvents = new List<LessonEvent>();
            this.StreamSubjectBridges = new List<StreamSubjectBridge>();
            this.UnitedGroups = new List<UnitedGroup>();
        }

        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public Nullable<int> StudentCount { get; set; }
        public int StreamID { get; set; }
        public virtual Stream Stream { get; set; }
        public virtual ICollection<LessonEvent> LessonEvents { get; set; }
        public virtual ICollection<StreamSubjectBridge> StreamSubjectBridges { get; set; }
        public virtual ICollection<UnitedGroup> UnitedGroups { get; set; }
    }
}
