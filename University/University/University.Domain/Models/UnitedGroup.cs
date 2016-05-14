using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class UnitedGroup
    {
        public int UnitedGroupsID { get; set; }
        public int FirstGroupID { get; set; }
        public int SecondGroupID { get; set; }
        public Nullable<int> ThirdGroupID { get; set; }
        public Nullable<int> FourthGroupID { get; set; }
        public Nullable<int> FifthGroupID { get; set; }
        public virtual Group Group { get; set; }
        public virtual Group Group1 { get; set; }
        public virtual Group Group2 { get; set; }
        public virtual Group Group3 { get; set; }
        public virtual Group Group4 { get; set; }
    }
}
