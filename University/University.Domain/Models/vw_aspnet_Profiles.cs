using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class vw_aspnet_Profiles
    {
        public System.Guid UserId { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public Nullable<int> DataSize { get; set; }
    }
}
