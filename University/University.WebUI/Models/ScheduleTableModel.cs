using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class ScheduleTableModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public WeekModel FirstWeek { get; set; }
        public WeekModel SecondWeek { get; set; }
        public List<ScheduleTableModel> ListScheduleTableModel { get; set; }

    }
}