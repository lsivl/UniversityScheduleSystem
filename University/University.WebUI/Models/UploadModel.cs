using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class UploadModel
    {
        public string TypeData { get; set; }
        public string CathedraName { get; set; }
        public int CathedraID { get; set; }
        public List<int> Courses { get; set; }
        public List<ExcelTableRowModel> RowList { get; set; }
    }
}