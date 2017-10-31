using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public class AbpFolders
    {
        public long Id { get; set; }
        public string FolderID { get; set; }
        public string ParentFolderID { get; set; }
        public string FolderName { get; set; }
        public string FolderJob { get; set; }
        public DateTime FolderDate { get; set; }
        public string FolderAccess { get; set; }
        public string FolderMeta { get; set; }     
        public string FolderRoot { get; set; }
        public string FolderOffice { get; set; }
        public int FolderRank { get; set; }
        public string FolderFiles { get; set; }
        public int FolderFileCount { get; set; }
        public int FolderSubFolderCount { get; set; }
        public DateTime FolderCreatedDate { get; set; }
        public DateTime FolderUpdated { get; set; }
        public string FolderSubChecksum { get; set; }

    }
}