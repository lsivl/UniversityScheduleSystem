using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class ClassroomType
    {
        public ClassroomType()
        {
            this.Classrooms = new List<Classroom>();
        }

        public int ClassroomTypeID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public virtual ICollection<Classroom> Classrooms { get; set; }
    }
}
