using FTBAPISERVER.Application;
using FTBAPISERVER.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace FTBAPISERVER.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        public string WebCredential;
        public string MachineCode;
        public string PhoneNumber;
        public string AuthMode;
        public string Company;
        public string mIPAddress;
        public bool TokenValid;
        public string Localization;
        public string IsMobile;
        public string OperatingSystem;
        public string DeviceType;
        public string Application;
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            try
            {
                string clientId = string.Empty;
                string clientSecret = string.Empty;
                string symmetricKeyAsBase64 = string.Empty;
                TokenValid = false;
                if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
                {
                    context.TryGetFormCredentials(out clientId, out clientSecret);
                }

                if (context.ClientId == null)
                {
                    context.SetError("invalid_clientId", "100 client_Id is not set");

                    AbpServerLog errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = clientId,
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "Login",
                        Parameters = "Invalid Client Id",
                        Exception = "Handled",
                        InfoType = "ERROR",
                        Details = "Cannot Login to AD."
                    };
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                    return Task.FromResult<object>(null);
                }
                var testUser = new RimSQL();
                var tokenExpiresAt = testUser.Doscalar("Select ExpiresAt from TokenTrack where UserName='" + context.ClientId + "' LIMIT 1");

               // try
                //{
                //    DateTime dt = Convert.ToDateTime(tokenExpiresAt);
                //    if (dt > DateTime.Now)
                //    {
                //        context.SetError("Token Issued", "Token is still Valid");
                //        TokenValid = true;
                 //       return Task.FromResult<object>(null);
                 //   }
               // }
                //catch
               // {
                    testUser.ExecuteNonQuery("DELETE FROM TokenTrack where UserName='"+context.ClientId.ToLower()+"'");
                    TokenValid = false;
               // }
                
                context.OwinContext.Response.Headers.Add("RimkusPass", new[] { "tempo" });
                WebCredential = clientSecret;
                MachineCode = context.Request.Context.Request.Headers["MachineCode"];
                PhoneNumber = context.Request.Context.Request.Headers["PhoneNumber"] ?? "7139921768";
                AuthMode = context.Request.Context.Request.Headers["AuthMode"];

                if (MachineCode.Contains(","))
                    MachineCode = MachineCode.Substring(0, MachineCode.IndexOf(',') - 1);

                // do not give the correct response
                if (MachineCode == null)
                {
                    context.SetError("invalid_MachineCode", "MachineCode is not set");
                    AbpServerLog errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = clientId,
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "Machine",
                        Parameters = "Invalid Machine Code",
                        Exception = "Handled",
                        InfoType = "ERROR",
                        Details = "Cannot Login to AD. Machine Code is Not Set"
                    };
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                    return Task.FromResult<object>(null);
                }

                Company = context.Request.Context.Request.Headers["Company"];
                if (Company == null)
                {
                    context.SetError("invalid_Company", "101 Company_Id is not set");
                    AbpServerLog errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = clientId,
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "Company",
                        Parameters = "Invalid Company",
                        Exception = "Handled",
                        InfoType = "ERROR",
                        Details = "Cannot Login to AD as no Company name."
                    };
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));

                    return Task.FromResult<object>(null);
                }
                Localization = context.Request.Context.Request.Headers["Localization"] ?? String.Empty;
                IsMobile = context.Request.Context.Request.Headers["IsMobile"] ?? String.Empty;
                OperatingSystem = context.Request.Context.Request.Headers["OperatingSystem"] ?? String.Empty;
                DeviceType = context.Request.Context.Request.Headers["DeviceType"] ?? String.Empty;
                Application = context.Request.Context.Request.Headers["Application "] ?? String.Empty;

                mIPAddress = context.Request.RemoteIpAddress;

                var audience = AudiencesStore.FindAudience(context.ClientId);
                if (audience == null)
                {
                    context.SetError("invalid_clientId", string.Format("102 Invalid audience client_id '{0}'", context.ClientId));
                    AbpServerLog errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = clientId,
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "NoName",
                        Parameters = "Invalid Name",
                        Exception = "Handled",
                        InfoType = "ERROR",
                        Details = "Cannot Login to AD as no valid name."
                    };
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));

                    return Task.FromResult<object>(null);
                }

                context.Validated();
                return Task.FromResult<object>(null);
            }
            catch (Exception)
            {

                return Task.FromResult<object>(null);
            }
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            bool hashouston = false;
            var Homedir = "";

            //if (TokenValid)
            //{
            //    context.SetError("Token Issued", "Token is still Valid ");
            //    return Task.FromResult<object>(null);
           // }

            var Description = "";
            var DisplayName = "";
            var EmailAddress = "";
            var ConnectedServer = "";
            var Name = "";
            var Sid = "";
            var UserPrincipalName = "";
            var givenName = "";
            var userName = "";

            try
            {
                const int SlidingExpire = -5;
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });                
                IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
                var authService = new AdAuthenticationService(authenticationManager);

                var logger = context.ClientId;
                if (logger.Contains("\\"))
                    logger = context.ClientId.Substring(context.ClientId.IndexOf("\\")+1);

                if (logger.Contains("/"))
                    logger = context.ClientId.Substring(context.ClientId.IndexOf("/"));


                if (logger.Contains("@"))
                {

                }
                else
                {
                    //System.IO.File.AppendAllText(@"c:\temp\ad.txt", "logging in");
                    var authenticationResult = authService.SignIn(logger, WebCredential); // model.Password);
                    //var authenticationResult = authService.SignIn(context.ClientId, WebCredential); // model.Password);
                    if (!authenticationResult.IsSuccess)
                    {
                        if (AuthMode == "VERIFYACCESS")
                        {
                            context.SetError("Invalid Access", "Failed");
                            return Task.FromResult<object>(null);
                        }

                        context.SetError("invalid_access", "103 The user name or password is incorrect , in future you will receive a null string");
                        //System.IO.File.AppendAllText(@"c:\temp\ad.txt", "logging failed");

                        AbpServerLog errorlog = new AbpServerLog()
                        {
                            LoginTime = DateTime.Now,
                            UserName = context.ClientId,
                            TenantName = RimkusHelper.APICompany,
                            MethodName = "Usr-pwd",
                            Parameters = "Invalid Username or Password",
                            Exception = "Handled",
                            InfoType = "ERROR",
                            Details = "Cannot Login to AD as username/pwd in error."
                        };
                        RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                        return Task.FromResult<object>(null);
                        
                    }
                    else
                    {
                        if (AuthMode=="VERIFYACCESS")
                        {
                            context.SetError("Authorized", "OK");
                            return Task.FromResult<object>(null);
                        }
                        Description = authenticationResult.Description;
                        DisplayName = authenticationResult.DisplayName;
                        EmailAddress = authenticationResult.EmailAddress;
                        ConnectedServer = authenticationResult.ConnectedServer;
                        Name = authenticationResult.Name;
                        Sid = authenticationResult.Sid;
                        UserPrincipalName = authenticationResult.UserPrincipalName;
                        givenName = authenticationResult.givenName;
                        userName = authenticationResult.userName;
                        Homedir= authenticationResult.HomeDirectory;

                    }


                }


/*
                Description = "testing User";
                DisplayName = "testing User";// authenticationResult.DisplayName;
                EmailAddress = "testing User";// authenticationResult.EmailAddress;
                ConnectedServer = "testing User";//authenticationResult.ConnectedServer;
                Name = "testing User";//authenticationResult.Name;
                Sid = "testing User";//authenticationResult.Sid;
                UserPrincipalName = "testing User";//authenticationResult.UserPrincipalName;
                givenName = "testing User";// authenticationResult.givenName;
                userName = "testing User";// authenticationResult.userName;
                Homedir = "testing User";//authenticationResult.HomeDirectory;
*/



                /*
                                // toDO:BOBBY BOBBY NO USER CHECKED
                                var authenticationResult = authService.SignIn(logger, WebCredential); // model.Password);

                //                var authenticationResult = authService.SignIn(context.ClientId, WebCredential); // model.Password);
                                if (!authenticationResult.IsSuccess)
                                {
                                    context.SetError("invalid_access", "103 The user name or password is incorrect , in future you will receive a null string");
                                    AbpServerLog errorlog = new AbpServerLog()
                                    {
                                        LoginTime = DateTime.Now,
                                        UserName = context.ClientId,
                                        TenantName = RimkusHelper.APICompany,
                                        MethodName = "Usr-pwd",
                                        Parameters = "Invalid Username or Password",
                                        Exception = "Handled",
                                        InfoType = "ERROR",
                                        Details = "Cannot Login to AD as username/pwd in error."
                                    };
                                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                                    return Task.FromResult<object>(null);
                                }
                                else
                                {
                                     Description = authenticationResult.Description ;
                                     DisplayName = authenticationResult.DisplayName ;
                                     EmailAddress = authenticationResult.EmailAddress ;
                                     ConnectedServer = authenticationResult.ConnectedServer ;
                                     Name = authenticationResult.Name ;
                                     Sid = authenticationResult.Sid ;
                                     UserPrincipalName = authenticationResult.UserPrincipalName ;
                                     givenName = authenticationResult.givenName ;
                                     userName = authenticationResult.userName ;
                                }      

                */



                // now do the MFA
                //todo i have removed mfa from micorosft temporarariliy

                /*
                PfAuthParams pfauthparams = new PfAuthParams();
                pfauthparams.Username = context.ClientId;
                pfauthparams.Phone = PhoneNumber;
                pfauthparams.Mode = pf_auth.MODE_STANDARD;
                pfauthparams.CertFilePath = HttpContext.Current.Server.MapPath("~/pf/cert_key.p12");

                int callstatus, errorId;

                try
                {
                    if (pf_auth.pf_authenticate(pfauthparams, out callstatus, out errorId))
                    {
                      
                    }
                    else
                    {
                        context.SetError("invalid_access", "103 MFA failure");
                        AbpServerLog errorlog = new AbpServerLog()
                        {
                            LoginTime = DateTime.Now,
                            UserName = context.ClientId,
                            TenantName = RimkusHelper.APICompany,
                            MethodName = "Usr-pwd",
                            Parameters = "MFA failed",
                            Exception = "Handled",
                            InfoType = "ERROR",
                            Details = "MFA Failed."
                        };
                        RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                        return Task.FromResult<object>(null);
                    }
                }
                catch (Exception eee)
                {
                    context.SetError("invalid_access", "103 MFA failure");
                    AbpServerLog errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = context.ClientId,
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "Usr-pwd",
                        Parameters = "MFA failed",
                        Exception = "Handled",
                        InfoType = "ERROR",
                        Details = "MFA Failed. " /// check here the length of field b4 placing message
                    };
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                    return Task.FromResult<object>(null);

                }
                */

                // get the machine code

                // check the token 

                var testUser = new RimSQL();
                var tokenExpiresAt = testUser.Doscalar("Select ExpiresAt from TokenTrack where UserName='" + context.ClientId.ToLower() + "' LIMIT 1");

                if (tokenExpiresAt.Length > 5)
                {
                    try
                    {
                        DateTime dt = Convert.ToDateTime(tokenExpiresAt).AddMinutes(SlidingExpire);
                        if (dt > DateTime.Now)
                        {
                            context.SetError("Token Issued", "Token is still Valid " + dt.ToString());
                            return Task.FromResult<object>(null);
                        }
                    }
                    catch
                    {
                        testUser.ExecuteNonQuery("DELETE FROM TokenTrack where UserName='" + context.ClientId.ToLower() + "'");
                    }

                }
                testUser.ExecuteNonQuery("DELETE FROM TokenTrack where UserName='" + context.ClientId.ToLower() + "'");

                // so if the user authenticates check it on the server
                //if (FTBAPISERVER.RimkusHelper.getUserInfo("Select UserName from dbo.AbpUsers where UserName='"+context.ClientId+"'")==context.ClientId  )
                //{
                // getall command

                //}
                //else
                //{
                //    // create new
                // FTBAPISERVER.RimkusHelper.putUserInfo("INSERT INTO dbo.AbpUsers (TenantId ,AuhenticationSource,UserName,Name,Surname,Password ,EmailAddress,IsActive) VALUES ('company','RIMKUSAPI','username','Name','Surname','Password','EmailAddress',1)");
                //}

                if (MachineCode.Contains(","))
                    MachineCode = MachineCode.Substring(0, MachineCode.IndexOf(',') - 1);
                var identity = new ClaimsIdentity("JWT");

                identity.AddClaim(new Claim("DisplayName", DisplayName??""));
                identity.AddClaim(new Claim("EmailAddress", EmailAddress ?? ""));
                identity.AddClaim(new Claim("ConnectedServer", ConnectedServer ?? ""));
                identity.AddClaim(new Claim("Name", Name ?? ""));
                identity.AddClaim(new Claim("Sid", Sid ?? ""));
                identity.AddClaim(new Claim("UserPrincipalName", UserPrincipalName ?? ""));
                identity.AddClaim(new Claim("givenName", givenName ?? ""));
                identity.AddClaim(new Claim("userName", userName ?? ""));
                identity.AddClaim(new Claim("Description", Description ?? ""));
                identity.AddClaim(new Claim("HomeDirectory", Homedir ?? ""));
                identity.AddClaim(new Claim(ClaimTypes.Name, context.ClientId ?? ""));
                identity.AddClaim(new Claim("ID", context.ClientId ?? ""));
                identity.AddClaim(new Claim("IP", mIPAddress ?? ""));               
                identity.AddClaim(new Claim("Company", Company));
                identity.AddClaim(new Claim("MachineCode", MachineCode));
                identity.AddClaim(new Claim("APIMachineCode", RimkusHelper.getCurrentMachine()));
                identity.AddClaim(new Claim("Localization", Localization));
                identity.AddClaim(new Claim("IsMobile", IsMobile));
                identity.AddClaim(new Claim("OperatingSystem", OperatingSystem));
                identity.AddClaim(new Claim("DeviceType", DeviceType));
                identity.AddClaim(new Claim("Application", Application));
                identity.AddClaim(new Claim("GUID", Guid.NewGuid().ToString()));
                identity.AddClaim(new Claim("CreationTime", DateTime.Now.ToString()));
                identity.AddClaim(new Claim("ExpirationTime", DateTime.Now.AddMinutes(120).ToString()));
                identity.AddClaim(new Claim("Accesslevel", "accesslevel"));
                

                List<string> shares;
                try
                {
                    shares = RimkusHelper.activeshares(context.ClientId, Company);
                }
                catch (Exception e)
                {
                    shares = new List<string>();
                    shares.Add(String.Empty);// Cannot obtain share info");
                }
                //shares = RimkusHelper.activeshares(context.ClientId,Company);

                foreach (var item in shares)
                {
                    if (item == "Houston Open Jobs") hashouston = true;
                    identity.AddClaim(new Claim(ClaimTypes.Role, item));
                }
                if (!hashouston) identity.AddClaim(new Claim(ClaimTypes.Role, "Houston Open Jobs"));

                //identity.AddClaim(new Claim(ClaimTypes.Role, "RCGHOUSTON/Close Jobs Houston"));
                //identity.AddClaim(new Claim(ClaimTypes.Role, "RCGHOUSTON/Accounts"));
                //identity.AddClaim(new Claim(ClaimTypes.Role, "DALLAS/Accounts"));
                //identity.AddClaim(new Claim(ClaimTypes.Role, "HOUSTON/Houston//Personal"));
                //identity.AddClaim(new Claim(ClaimTypes.Role, "TORONTO/Manager"));
                //identity.AddClaim(new Claim(ClaimTypes.Role, "RCGFTB/Photos from Cell"));

                var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                         "audience", (context.ClientId == null) ? string.Empty : context.ClientId
                    }
                });

                var ticket = new AuthenticationTicket(identity, props);
                context.Validated(ticket);
                //testUser = new RimSQL(); this entry is made by admin
                //testUser.ExecuteNonQuery("INSERT INTO TokenTrack (UserName,TokenKey,IssuedAt,ExpiresAt) VALUES ('"+ticket.Identity.Name.ToLower() +"','"+ticket.Identity.Label +"','"+ DateTime.Now.ToString()+"','"+DateTime.Now.AddMinutes(120).ToString() +"')");
                return Task.FromResult<object>(null);
            }
            catch (Exception)
            {

                return Task.FromResult<object>(null);
            }
        }
    }
}