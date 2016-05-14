﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class ShowContentModel
    {
        public string TypeData { get; set; }
        public List<ExcelTableRowModel> TableRows { get; set; }
        public int SelectedCourse { get; set; }
    }
}