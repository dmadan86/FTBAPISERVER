﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FTBFileMgtEntities : DbContext
    {
        public FTBFileMgtEntities()
            : base("name=FTBFileMgtEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AbpAuditLog> AbpAuditLogs { get; set; }
        public virtual DbSet<AbpBackgroundJob> AbpBackgroundJobs { get; set; }
        public virtual DbSet<AbpEdition> AbpEditions { get; set; }
        public virtual DbSet<AbpFeature> AbpFeatures { get; set; }
        public virtual DbSet<AbpFileShareNotification> AbpFileShareNotifications { get; set; }
        public virtual DbSet<AbpFileShare> AbpFileShares { get; set; }
        public virtual DbSet<AbpFileShareSubscription> AbpFileShareSubscriptions { get; set; }
        public virtual DbSet<AbpGroup> AbpGroups { get; set; }
        public virtual DbSet<AbpGroupAccount> AbpGroupAccounts { get; set; }
        public virtual DbSet<AbpLanguage> AbpLanguages { get; set; }
        public virtual DbSet<AbpLanguageText> AbpLanguageTexts { get; set; }
        public virtual DbSet<AbpNotification> AbpNotifications { get; set; }
        public virtual DbSet<AbpNotificationSubscription> AbpNotificationSubscriptions { get; set; }
        public virtual DbSet<AbpOrganizationUnit> AbpOrganizationUnits { get; set; }
        public virtual DbSet<AbpPermission> AbpPermissions { get; set; }
        public virtual DbSet<AbpRole> AbpRoles { get; set; }
        public virtual DbSet<AbpServer> AbpServers { get; set; }
        public virtual DbSet<AbpServerNotification> AbpServerNotifications { get; set; }
        public virtual DbSet<AbpServerSubscription> AbpServerSubscriptions { get; set; }
        public virtual DbSet<AbpSetting> AbpSettings { get; set; }
        public virtual DbSet<AbpSharePermission> AbpSharePermissions { get; set; }
        public virtual DbSet<AbpTenantNotification> AbpTenantNotifications { get; set; }
        public virtual DbSet<AbpTenant> AbpTenants { get; set; }
        public virtual DbSet<AbpUserAccount> AbpUserAccounts { get; set; }
        public virtual DbSet<AbpUserGroup> AbpUserGroups { get; set; }
        public virtual DbSet<AbpUserLoginAttempt> AbpUserLoginAttempts { get; set; }
        public virtual DbSet<AbpUserLogin> AbpUserLogins { get; set; }
        public virtual DbSet<AbpUserNotification> AbpUserNotifications { get; set; }
        public virtual DbSet<AbpUserOrganizationUnit> AbpUserOrganizationUnits { get; set; }
        public virtual DbSet<AbpUserRole> AbpUserRoles { get; set; }
        public virtual DbSet<AbpUser> AbpUsers { get; set; }
    }
}
