using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.WebUI.Models
{
    public class SaveResultModel
    {
        public List<string> UncorrectCells { get; set; }
        public string Message { get; set; }
    }
}