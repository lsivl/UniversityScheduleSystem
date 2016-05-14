using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class Cathedra
    {
        public Cathedra()
        {
            this.Classrooms = new List<Classroom>();
            this.StreamSubjectBridges = new List<StreamSubjectBridge>();
            this.Subjects = new List<Subject>();
            this.Teachers = new List<Teacher>();
        }

        public int CathedraID { get; set; }
        public string Name { get; set; }
        public Nullable<int> FacultyID { get; set; }
        public string FullName { get; set; }
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<Classroom> Classrooms { get; set; }
        public virtual ICollection<StreamSubjectBridge> StreamSubjectBridges { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}
