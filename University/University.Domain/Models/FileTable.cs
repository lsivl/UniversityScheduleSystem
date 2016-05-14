using System;
using System.Collections.Generic;

namespace University.Domain.Models
{
    public partial class FileTable
    {
        public int FileID { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
