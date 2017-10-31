using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public class AbpDocuments
    {
        public long Id { get; set; }
        public string DocumentID { get; set; }
        public long FolderID { get; set; }
        public string DocumentName { get; set; }
        public string DocumentSize { get; set; }
        public DateTime DocumentDate { get; set; }
        public string DocumentJob { get; set; }
        public string DocumentAccess { get; set; }
        public string DocumentOffice { get; set; }
        public string DocumentType { get; set; }
        public string DocumentIcon { get; set; }
        public int DocumentCompressed { get; set; }
        public DateTime DocumentLastAccessed { get; set; }
        public string DocumentMetaData { get; set; }
    }
}