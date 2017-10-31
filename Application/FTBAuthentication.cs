using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;


namespace FTBAPI
{
    public static class FTBAuthentication
    {
        public const String ApplicationCookie = "FTBAuthenticationType";
    }

    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // need to add UserManager into owin, because this is used in cookie invalidation
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = FTBAuthentication.ApplicationCookie,
                LoginPath = new PathString("/Login"),  //Login"),
                Provider = new CookieAuthenticationProvider(),
                CookieName = "FTBCookie",
                CookieHttpOnly = true,
                // Todo - Ask Justin
                ExpireTimeSpan = TimeSpan.FromHours(2), // adjust to needs , 
            });
        }
    }
}