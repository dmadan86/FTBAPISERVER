using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public class FolderData
    {
        public long Id { get; set; }
        public string FolderID { get; set; }
        public string ParentFolderID { get; set; }
        public string FolderName { get; set; }
        public string FolderJob { get; set; }
    }
}