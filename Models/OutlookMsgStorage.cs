using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER
{
    public class OutlookMsgStorage
    {
        
        public string fileloc { get; set; }
        public int Id { get; set; }
        public string From { get; set; }
        public string ShortFrom { get; set; }
        public string FromEmail { get; set; }
        public string subject { get; set; }
        public string shortsubject { get; set; }
        public List<string> Recipient { get; set; }
        public string Recipients { get; set; }
        public List<string> RecipientEmail { get; set; }
        public string RecipientEmails { get; set; }
        public List<string> Attachments { get; set; }
        public string Attachment { get; set; }
        public string Message { get; set;}
        public string SubMessage { get; set; }

        public List<string> SubMessages { get; set; }
        public DateTime ReceivedDate { get; set; }

    }
}