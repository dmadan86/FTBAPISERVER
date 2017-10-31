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
    
    public partial class AbpTenant
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AbpTenant()
        {
            this.AbpServers = new HashSet<AbpServer>();
        }
    
        public int Id { get; set; }
        public string TenancyName { get; set; }
        public string TenantGUID { get; set; }
        public string Name { get; set; }
        public string TenantOfficeCode { get; set; }
        public string TenantAddress1 { get; set; }
        public string TenantAddress2 { get; set; }
        public string TenantCity { get; set; }
        public string TenantState { get; set; }
        public string TenantCounty { get; set; }
        public string TenantPostalCode { get; set; }
        public string TenantCountry { get; set; }
        public string TenantPhone { get; set; }
        public string TenantMobile { get; set; }
        public string TenantWebSite { get; set; }
        public string TenantManager { get; set; }
        public string TenantManagerEmail { get; set; }
        public string TenantTerritory { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<long> DeleterUserId { get; set; }
        public Nullable<System.DateTime> DeletionTime { get; set; }
        public Nullable<System.DateTime> LastModificationTime { get; set; }
        public Nullable<long> LastModifierUserId { get; set; }
        public Nullable<System.DateTime> CreationTime { get; set; }
        public Nullable<long> CreatorUserId { get; set; }
        public Nullable<int> EditionId { get; set; }
        public string ConnectionString { get; set; }
    
        public virtual AbpEdition AbpEdition { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpServer> AbpServers { get; set; }
        public virtual AbpUser AbpUser { get; set; }
        public virtual AbpUser AbpUser1 { get; set; }
        public virtual AbpUser AbpUser2 { get; set; }
    }
}