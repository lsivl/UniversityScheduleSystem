using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class WeekModel
    {
        public DayModel Monday { get; set; }
        public DayModel Tuesday { get; set; }
        public DayModel Wednesday { get; set; }
        public DayModel Thursday { get; set; }
        public DayModel Friday { get; set; }

    }
}