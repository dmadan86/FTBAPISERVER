using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public class LogData
    {
        public string UserName{ get; set; }
        public string PortNumber { get; set; }
        public string LogEntry { get; set; }
        public string StartedAt { get; set; }
        public string EndsAt { get; set; }
        public string Company { get; set; }
        public long DataRx { get; set; }
        public long DataTx { get; set; }
    }
}



