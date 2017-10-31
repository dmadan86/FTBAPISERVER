using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using FTBAPISERVER.Formats;
using FTBAPISERVER.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Configuration;
using System.Web.Hosting;

namespace FTBAPISERVER
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            // Web API routes
            config.MapHttpAttributeRoutes();

            ConfigureOAuth(app);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(config);
            // let us put he license here 

            Aspose.Words.License wordsLicense = new Aspose.Words.License();
            wordsLicense.SetLicense(@HostingEnvironment.ApplicationPhysicalPath+"/Aspose.Total.lic");

            Aspose.Cells.License cellsLicense = new Aspose.Cells.License();
            cellsLicense.SetLicense(@HostingEnvironment.ApplicationPhysicalPath + "/Aspose.Total.lic");

            Aspose.Pdf.License pdfLicense = new Aspose.Pdf.License();
            pdfLicense.SetLicense(@HostingEnvironment.ApplicationPhysicalPath + "/Aspose.Total.lic");

            Aspose.Imaging.License imgLicense = new Aspose.Imaging.License();
            imgLicense.SetLicense(@HostingEnvironment.ApplicationPhysicalPath + "/Aspose.Total.lic");

            Aspose.Slides.License slideLicense = new Aspose.Slides.License();
            slideLicense.SetLicense(@HostingEnvironment.ApplicationPhysicalPath + "/Aspose.Total.lic");

            Aspose.Note.License noteLicense = new Aspose.Note.License();
            noteLicense.SetLicense(@HostingEnvironment.ApplicationPhysicalPath + "/Aspose.Total.lic");

        }

        public void ConfigureOAuth(IAppBuilder app)
        {

            try
            {
                OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
                {
                    //Todo
                    //For Dev enviroment only (on future deployment should be AllowInsecureHttp = false)                
                    AllowInsecureHttp = true,
                    TokenEndpointPath = new PathString("/39d9a96c-4adb-4c24-9111-023673c6b3d6/login"),

                    //TokenEndpointPath = new PathString("/"+RimkusHelper.DataParser("[AbpTenants.Id]{TenantGUID}","1")+ "/login"),
                    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(120),
                    // you have to change expiration time at three places , remember
                    Provider = new CustomOAuthProvider(),
                    AccessTokenFormat = new CustomJwtFormat("https://RCGFTB01.rimkus.net") //RimkusHelper.HeadServer)
                };
                //System.IO.File.WriteAllText(@"C:\Temp\Endpoint.txt", OAuthServerOptions.TokenEndpointPath.ToString());
                app.UseOAuthAuthorizationServer(OAuthServerOptions);
            }
            catch (Exception dd) 
            {


                throw;
            }
            // OAuth 2.0 Bearer Access Token Generation
           

        }
    }
}