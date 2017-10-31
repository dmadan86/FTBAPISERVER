using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public class RCGRemoteFile
    {
        public string tokencheck { get; set; }
        public string FileShare { get; set; }
        public string RootPath { get; set; }
        public string Filename { get; set; }
    }
}