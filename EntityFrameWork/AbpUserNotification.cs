//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FTBAPISERVER.EntityFrameWork
{
    using System;
    using System.Collections.Generic;
    
    public partial class AbpUserNotification
    {
        public System.Guid Id { get; set; }
        public long UserId { get; set; }
        public System.Guid TenantNotificationId { get; set; }
        public int State { get; set; }
        public System.DateTime CreationTime { get; set; }
        public Nullable<int> TenantId { get; set; }
    }
}