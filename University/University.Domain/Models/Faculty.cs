using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class Faculty
    {
        public Faculty()
        {
            this.Cathedras = new List<Cathedra>();
            this.Streams = new List<Stream>();
        }

        public int FacultyID { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public virtual ICollection<Cathedra> Cathedras { get; set; }
        public virtual ICollection<Stream> Streams { get; set; }
    }
}
