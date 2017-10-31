using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public class FileData
    {
        public long Id { get; set; }
        public long FolderID { get; set; }
        public string DocumentName { get; set; }
    }
}