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
    
    public partial class AbpUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AbpUser()
        {
            this.AbpPermissions = new HashSet<AbpPermission>();
            this.AbpRoles = new HashSet<AbpRole>();
            this.AbpRoles1 = new HashSet<AbpRole>();
            this.AbpRoles2 = new HashSet<AbpRole>();
            this.AbpSettings = new HashSet<AbpSetting>();
            this.AbpSharePermissions = new HashSet<AbpSharePermission>();
            this.AbpTenants = new HashSet<AbpTenant>();
            this.AbpTenants1 = new HashSet<AbpTenant>();
            this.AbpTenants2 = new HashSet<AbpTenant>();
            this.AbpUserLogins = new HashSet<AbpUserLogin>();
            this.AbpUserRoles = new HashSet<AbpUserRole>();
            this.AbpUsers1 = new HashSet<AbpUser>();
            this.AbpUsers11 = new HashSet<AbpUser>();
            this.AbpUsers12 = new HashSet<AbpUser>();
        }
    
        public long Id { get; set; }
        public Nullable<int> TenantId { get; set; }
        public string AuthenticationSource { get; set; }
        public string AuthenticationType { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string DistinguishedName { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public string SamAccountName { get; set; }
        public string Password { get; set; }
        public string OTPPassword { get; set; }
        public string OTPSource { get; set; }
        public Nullable<System.DateTime> OTPExpires { get; set; }
        public string EmailAddress { get; set; }
        public string InGroup { get; set; }
        public string UserPrincipalName { get; set; }
        public string VoiceTelephoneNumber { get; set; }
        public string MobileTelephoneNumber { get; set; }
        public Nullable<bool> IsEmailConfirmed { get; set; }
        public string EmailConfirmationCode { get; set; }
        public string PasswordResetCode { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> LastLoginTime { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<long> DeleterUserId { get; set; }
        public Nullable<System.DateTime> DeletionTime { get; set; }
        public Nullable<System.DateTime> AccountExpirationDate { get; set; }
        public Nullable<System.DateTime> LastModificationTime { get; set; }
        public Nullable<System.DateTime> LastLogon { get; set; }
        public Nullable<long> LastModifierUserId { get; set; }
        public Nullable<System.DateTime> CreationTime { get; set; }
        public Nullable<long> CreatorUserId { get; set; }
        public string SID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpPermission> AbpPermissions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpRole> AbpRoles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpRole> AbpRoles1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpRole> AbpRoles2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpSetting> AbpSettings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpSharePermission> AbpSharePermissions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpTenant> AbpTenants { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpTenant> AbpTenants1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpTenant> AbpTenants2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpUserLogin> AbpUserLogins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpUserRole> AbpUserRoles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpUser> AbpUsers1 { get; set; }
        public virtual AbpUser AbpUser1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpUser> AbpUsers11 { get; set; }
        public virtual AbpUser AbpUser2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbpUser> AbpUsers12 { get; set; }
        public virtual AbpUser AbpUser3 { get; set; }
    }
}
