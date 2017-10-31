using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public class FileTxInfo
    {
        public string User { get; set; }
 	public string CommandName { get; set; }
	public string EndPoint { get; set; }
        public string Company { get; set; }
        public string MachineCode { get; set; }
        public string Authenticity { get; set; }
        public string OTP { get; set; }
        public string FileShare { get; set; }
        public string RootPath { get; set; }
        public string Parameter1 { get; set; }
        public FileAttr Parameter1Attr { get; set; }
        public string Parameter2 { get; set; }
        public FileAttr Parameter2Attr { get; set; }
        public string Parameter3 { get; set; }
        public FileAttr Parameter3Attr { get; set; }
        public string Parameter4 { get; set; }
        public FileAttr Parameter4Attr { get; set; }
        public String Parameter5 { get; set; }
        public FileAttr Parameter5Attr { get; set; }
        public DateTime ValidTime { get; set; }
    }
}