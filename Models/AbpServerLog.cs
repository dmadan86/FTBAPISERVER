using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public partial class AbpServerLog
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> LoginTime { get; set; }
        public Nullable<long> UserId { get; set; }
        public string UserName { get; set; }
        public string TenantName { get; set; }
        public string MethodName { get; set; }
        public string Parameters { get; set; }
        public string Exception { get; set; }
        public string InfoType { get; set; }
        public string Details { get; set; }
    }
}