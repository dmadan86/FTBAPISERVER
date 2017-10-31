using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.DirectoryServices.AccountManagement;
using FTBAPI;
using System.DirectoryServices;

namespace FTBAPISERVER.Application
{
    public class AdAuthenticationService
    {
        public class AuthenticationResult
        {
            public AuthenticationResult(string errorMessage = null, string description = null, string displayname = null, string emailaddress = null, string connectedserver = null, string name = null, string sid = null, string userprincipalname = null, string givenname = null, string username = null, string homedirectory="")
            {
                ErrorMessage = errorMessage;
                Description = description;                
                DisplayName = displayname;
                EmailAddress = emailaddress;
                ConnectedServer = connectedserver;
                Name = name;
                Sid = sid;
                UserPrincipalName = userprincipalname;
                givenName = givenname;
                userName = username;
                HomeDirectory = homedirectory;
            }

            public String ErrorMessage { get; private set; }
            public Boolean IsSuccess => String.IsNullOrEmpty(ErrorMessage);
            public string Description { get; set; }
            public string DisplayName { get; set; }
            public string EmailAddress { get; set; }
            public string ConnectedServer { get; set; }
            public string Name { get; set; }
            public string Sid { get; set; }
            public string UserPrincipalName { get; set; }
            public string givenName { get; set; }
            public string userName { get; set; }
            public string HomeDirectory { get; set; }

        }

        private readonly IAuthenticationManager authenticationManager;

        public AdAuthenticationService(IAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
        }


        /// <summary>
        /// Check if username and password matches existing account in rimkus AD , use Dev Ad. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public AuthenticationResult SignIn(String username, String password)
        {           
            var HomeDir = "";
#if DEBUG
            // authenticates against my local machine - for development time
            ContextType authenticationType = ContextType.Domain;  // Machine;
#else
            // authenticates against Rimkus Domain AD
            ContextType authenticationType = ContextType.Domain;
#endif
            //System.IO.File.AppendAllText(@"c:\temp\ad.txt", username + " " + password + Environment.NewLine); 
            //todo : please remove this line on prod
            //return new AuthenticationResult();
            /////////////////////////////////////

             PrincipalContext principalContext = new PrincipalContext(authenticationType);
            bool isAuthenticated = false;
            UserPrincipal userPrincipal = null;


            //isAuthenticated = true;
            //return new AuthenticationResult(null, userPrincipal.Description, userPrincipal.DisplayName, userPrincipal.EmailAddress, userPrincipal.Context.ConnectedServer, userPrincipal.Name, userPrincipal.Sid.ToString(), userPrincipal.UserPrincipalName, userPrincipal.GivenName, username, HomeDir);


            try
            {
                
                isAuthenticated = principalContext.ValidateCredentials(@username, password, ContextOptions.Negotiate);
                if (isAuthenticated)
                {
                    userPrincipal = UserPrincipal.FindByIdentity(principalContext, username);
                    HomeDir = userPrincipal.HomeDirectory; 
                    //Description = userPrincipal.Description;  
                }
            }
            catch (Exception eee)
            {
                //System.IO.File.AppendAllText(@"c:\temp\ad.txt", eee.Message);

                isAuthenticated = false;
                userPrincipal = null;
            }

            try
            {
                if (!isAuthenticated || userPrincipal == null)
                {
                    //System.IO.File.AppendAllText(@"c:\temp\ad.txt", "not authenticated 1");

                    return new AuthenticationResult("Username or Password is not correct");
                }

                if (userPrincipal.IsAccountLockedOut())
                {
                   // System.IO.File.AppendAllText(@"c:\temp\ad.txt", "not authenticated 2");

                    //Todo : Ask Justin to  revealing this information
                    return new AuthenticationResult("Your account is locked.");
                }

                if (userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value == false)
                {
                    // Todo : Ask Justin  weather it is worth 
                    // revealing this information
                  //  System.IO.File.AppendAllText(@"c:\temp\ad.txt", "not authenticated 3");

                    return new AuthenticationResult("Your account is disabled");
                }

                //System.IO.File.AppendAllText(@"c:\temp\ad.txt", "create identity");
                var identity = CreateIdentity(userPrincipal);
               // System.IO.File.AppendAllText(@"c:\temp\ad.txt", "created check point");
                authenticationManager.SignOut(FTBAuthentication.ApplicationCookie);
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);
                return new AuthenticationResult(null, userPrincipal.Description, userPrincipal.DisplayName, userPrincipal.EmailAddress, userPrincipal.Context.ConnectedServer, userPrincipal.Name, userPrincipal.Sid.ToString(), userPrincipal.UserPrincipalName, userPrincipal.GivenName, username, HomeDir);

            }
            catch (Exception eee)
            {
                //System.IO.File.AppendAllText(@"c:\temp\ad.txt", eee.Message);
                return new AuthenticationResult(eee.Message);
                //throw;
            }

        }


        public List<GroupPrincipal> GetGroups(string userName)
        {
            List<GroupPrincipal> result = new List<GroupPrincipal>();

            // establish domain context
            PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);

            // find your user
            UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, userName);

            // if found - grab its groups
            if (user != null)
            {
                PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

                // iterate over all groups
                foreach (Principal p in groups)
                {
                    // make sure to add only group principals
                    if (p is GroupPrincipal)
                    {
                        result.Add((GroupPrincipal)p);
                    }
                }
            }

            return result;
        }

        public string GetDepartment(string username)
        {
            string result = string.Empty;

            PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);

            // find the user
            UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, username);

            // if user is found
            if (user != null)
            {
                // get DirectoryEntry underlying it
                DirectoryEntry de = (user.GetUnderlyingObject() as DirectoryEntry);

                if (de != null)
                {
                    if (de.Properties.Contains("department"))
                    {
                        result = de.Properties["department"][0].ToString();
                    }
                }
            }

            return result;
        }

        private ClaimsIdentity CreateIdentity(UserPrincipal userPrincipal)
        {
            var identity = new ClaimsIdentity(FTBAuthentication.ApplicationCookie, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "Active Directory"));
            identity.AddClaim(new Claim(ClaimTypes.Name, userPrincipal.SamAccountName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userPrincipal.SamAccountName));
            if (!String.IsNullOrEmpty(userPrincipal.EmailAddress))
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, userPrincipal.EmailAddress));
            }

            // add your own claims if you need to add more information stored on the cookie

            return identity;
        }
    }
}