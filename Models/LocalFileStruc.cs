using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public class LocalFileStruc
    {
        public bool Selected { get; set; }
        public string filename { get; set; }
        public string winfiletype { get; set; }
        public string filetype { get; set; }
        public string fileicon { get; set; }
        public long filesize { get; set; }
        public string fullpath { get; set; }
        public DateTime lastwritetime { get; set; }
        public DateTime creationtime { get; set; }
        public string attributes { get; set; }

    }
}