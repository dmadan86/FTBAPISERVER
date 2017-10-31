using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public class FileAttr
    {
        public string CreatedBy { get; set; }
        public int ChunkSize { get; set; }
        public int filesize { get; set; }
        public string Extension { get; set; }
        public bool IsFolder { get; set; }
        public bool IsZipped { get; set; }
        public bool IsCompressed { get; set; }

    }
}