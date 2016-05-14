using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class UnitedGroup
    {
        public int UnitedGroupID { get; set; }
        public int GroupID { get; set; }
        public int StreamID { get; set; }
        public int SubjectID { get; set; }
        public int CathedraID { get; set; }
        public int LessonTypeID { get; set; }
        public int Course { get; set; }
        public int Row { get; set; }
        public virtual Cathedra Cathedra { get; set; }
        public virtual Group Group { get; set; }
        public virtual Stream Stream { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
