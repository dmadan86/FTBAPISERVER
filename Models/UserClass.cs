using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    public class UserClass
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
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
        public string EmailAddress { get; set; }
        public string InGroup { get; set; }
        public string UserPrincipalName { get; set; }
        public string VoiceTelephoneNumber { get; set; }
        public int IsEmailConfirmed { get; set; }
        public string EmailConfirmationCode { get; set; }
        public string PasswordResetCode { get; set; }
        public string Description { get; set; }
        public DateTime LastLoginTime { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
        public int DeleterUserId { get; set; }
        public DateTime DeletionTime { get; set; }
        public DateTime AccountExpirationDate { get; set; }
        public DateTime LastModificationTime { get; set; }
        public DateTime LastLogon { get; set; }
        public int LastModifierUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreatorUserId { get; set; }
    }
}