using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public class TokenData
    {
        public string unique_name { get; set; }
        public string ID { get; set; }
        public string IP { get; set; }
        public string Company { get; set; }
        public string MachineCode { get; set; }
        public string APIMachineCode { get; set; }
        public string Localization { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string Description { get; set; }
        public string HomeDirectory { get; set; }
        public string IsMobile { get; set; }
        public string OperatingSystem { get; set; }
        public string GUID { get; set; }
        public string CreationTime { get; set; }
        public string ExpirationTime { get; set; }
        public string DeviceType { get; set; }
        public string Application { get; set; }
        public List<string> role { get; set; }
        public string iss { get; set; }
        public string aud { get; set; }
        public string exp { get; set; }
        public string nbf { get; set; }
        public string Accesslevel { get; set; }
    }
}