using FTBAPISERVER.Application;
using FTBAPISERVER.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FTBAPISERVER.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var test = new RimSQL();
            //var getuser = test.Doscalar("Select Id from TokenTrack LIMIT 1");
            //Todo usually this will be injected via DI. but creating this manually now during developpment
            //IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            //var authService = new AdAuthenticationService(authenticationManager);
            //var authenticationResult = authService.SignIn("zfaruqi", "Star@1234"); // model.Password);
            // adding of outlook provider will be on differn
            //if (authenticationResult.IsSuccess)
            //{
            //    // we are in!
            //}
            return View();// HttpNotFound();//  View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "API Controller for Office.";

            return View();
        }
        public ActionResult FileViewer()
        {
            return View();
        }

        public string getdata(string id, string data)
        {
            if (Request.ServerVariables["HTTP_HOST"] != "localhost") return String.Empty;

            var webclient = new System.Net.WebClient();
            var downloadedString = webclient.DownloadString(RimkusHelper.HeadServer+ "/login/getdata/" + id + "/" + data);
            return downloadedString;
        }
        public ActionResult viewme()
        {
            return View();
        }
        public string GetFileInfo() //string id,string data ,string matter)
        {

           var id = System.Web.HttpContext.Current.Request.Headers["FileShare"] ?? string.Empty;
           var data = System.Web.HttpContext.Current.Request.Headers["RootPath"] ?? string.Empty;
           var matter = System.Web.HttpContext.Current.Request.Headers["Filex"] ?? string.Empty;

            if (string.IsNullOrEmpty(id))
            {
                return "500 ERROR Destination Folder does not exists";
            }
            try
            {               
                string tempdir = pretrail(id, data);
                if (string.IsNullOrEmpty(tempdir))
                {
                    return "500 ERROR Destination Folder does not exists";
                }
                if (matter.Contains("\\"))
                {
                    FileInfo testfile = new FileInfo(matter);
                    matter = testfile.Name;
                }
              

                var filex = Path.Combine(tempdir, matter);

                FileMetaData bolax = new StreamFilesController().GetFileMetaData(@filex);
                if (bolax.FileResponseMessage.IsExists)
                {
                    return JsonConvert.SerializeObject(bolax);
                }
                else
                {
                    return "220 OK";
                }
            }
            catch (Exception eee)
            {
                return "ERROR "+eee.Message;
                throw;
            }

            return String.Empty;
        }
        //todo: remove this method
        public string GetxxxComPort()
        {
            int randomNumber = 0;
            try
            {
                //var tempo = RimkusHelper.RimTerm(randomNumber.ToString());
                Random random = new Random();
                var testUser = new RimSQL();
                for (int i = 0; i < 20; i++)
                {
                    random = new Random();
                    var randoNumber = random.Next(6999, 60001);
                    randomNumber = 6998 + random.Next((60001 - 6998) / 2) * 2;

                    if (testUser.Doscalar("SELECT PortNumber FROM PortTrack where PortNumber='" + randomNumber.ToString() + "'") == "") { break; }
                    else
                    {
                        var dater = testUser.Doscalar("SELECT RecdAt FROM PortTrack where PortNumber='" + randomNumber.ToString() + "'");
                        TimeSpan ts = DateTime.UtcNow - Convert.ToDateTime(dater);
                        if (ts.Hours > 4)
                        {
                            testUser.ExecuteNonQuery("UPDATE PortTrack SET PortNumber='" + "[" + randomNumber.ToString() + "]" + "', LogEntry='" + "Re-Using Port as no Data Received" + "',ExpiresAt='" + DateTime.UtcNow.ToString() + "' WHERE PortNumber='" + randomNumber.ToString() + "'");
                            break;
                        }
                    }
                }
                //var temptoken = JsonConvert.SerializeObject(tokendata);
                //temptoken = temptoken.Replace("'", "''");
                testUser.ExecuteNonQuery("INSERT INTO PortTrack (UserName, PortNumber , TokenKey, RecdAt) VALUES ('" + "User" + "','" + randomNumber.ToString() + "','" + "Token" + "','" + DateTime.UtcNow.ToString() + "')");
                System.Threading.Thread.Sleep((int)System.TimeSpan.FromSeconds(1).TotalMilliseconds);
                if (RimkusHelper.RimTerm(randomNumber.ToString(), "", "", "").Result == "OK")
                {
                    System.Threading.Thread.Sleep((int)System.TimeSpan.FromSeconds(1).TotalMilliseconds);
                    return "220 Service ready for user on Port " + randomNumber;
                }
                else
                {
                    return "500 Error in opening port " + randomNumber;
                }

                //return Content("220 Service ready for user " + tokendata.User + " on Port " + randomNumber);
            }
            catch (Exception rrr)
            {

                return "500 Port Error  " + rrr.Message;
            }
                     
        }

        //[Route("TOKENDATA/")]
        [HttpPost]
        public string WriteToken()
        {
            if (System.Web.HttpContext.Current.Request.Headers["Security"] != RimkusHelper.EndPointTokenCode)
            {
                return "Security Error";
            }

            var UserName = System.Web.HttpContext.Current.Request.Headers["UserName"] ?? String.Empty;
            var Token = System.Web.HttpContext.Current.Request.Headers["Token"] ?? string.Empty;
            var TokenKey = System.Web.HttpContext.Current.Request.Headers["TokenKey"] ?? string.Empty;
            var IssuedAt = System.Web.HttpContext.Current.Request.Headers["IssuedAt"] ?? string.Empty;
            var ExpiresAt = System.Web.HttpContext.Current.Request.Headers["ExpiresAt"] ?? string.Empty;
            // use the header
            if (String.IsNullOrEmpty(UserName)) return "No Name";
            try
            {
                var testUser = new RimSQL();
                testUser.ExecuteNonQuery("DELETE FROM TokenTrack where UserName='" + UserName.ToLower() + "'");
                testUser.ExecuteNonQuery("INSERT INTO TokenTrack (UserName,Token,TokenKey,IssuedAt,ExpiresAt) VALUES ('" + UserName.ToLower() + "','" + Token + "','" + TokenKey + "','" + IssuedAt + "','" + ExpiresAt + "')");
                return "OK";
            }
            catch (Exception ee)
            {
                return ee.Message;
            }
        }

        [HttpPost]
        public string logoff()
        {
            if (System.Web.HttpContext.Current.Request.Headers["Security"] != RimkusHelper.EndPointTokenCode)
            {
                return "Security Error";
            }

                var UserName = System.Web.HttpContext.Current.Request.Headers["UserName"] ?? String.Empty;
            try
            {
                var testUser = new RimSQL();
                testUser.ExecuteNonQuery("DELETE FROM TokenTrack where UserName='" + UserName.ToLower() + "'");
                return "OK";
            }
            catch (Exception ee)
            {
                return "Error " + ee.Message;
            }
        }
        public ActionResult GetDummyToken()
        {
            //Todo Remove it in production
            var bola = "{'Company':'Houston','Localization':'en-US,en;q=0.8','UserId':null,'UserEmail':null,'IsMobile':false,'DeviceType':'WinNT','OperatingSystem':'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36','OperatingSystemVersion':null,'Application':'Chrome','AllowedShares':null,'MachineCode':null,'Resources':['ALL'],'Permissions':null,'access_token':'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1bmlxdWVfbmFtZSI6InpmYXJ1cWkiLCJzdWIiOiJ6ZmFydXFpIiwicm9sZSI6WyJNYW5hZ2VyIiwiU3VwZXJ2aXNvciJdLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjI1ODU2IiwiYXVkIjoiemZhcnVxaSIsImV4cCI6MTQ3NDI5ODc4OCwibmJmIjoxNDc0Mjk2OTg4fQ.qvmRlmr-L6TAn4hKKSkVb_vihDEF9cUnjyYxxY55j9o','token_type':'bearer','expires_in':1799,'expires_at':'2016-09-20T20:08:06.8133664-05:00','FileShares':['HOUSTON/All Open Jobs ',' HOUSTON/All Closed Job ',' HOUSTON/Your folder'],'EndPoint':'http://10.0.13.101/FTBAPISERVER/ftbfm/'}";
            return Content(bola);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "RImkus Contact.";

            return View();
        }

        [HttpPost]
        public string GetTokenForDistant()
        {
            if (Request.ServerVariables["HTTP_HOST"] != "localhost") return "7777";

            var PortNumber = System.Web.HttpContext.Current.Request.Headers["PortNumber"] ?? String.Empty;
            var testUser = new RimSQL();
            return testUser.Doscalar("SELECT TokenKey FROM PortTrack WHERE PortNumber='" + PortNumber + "'");
        }

        public void SendFile()
        {
            string filex = System.Web.HttpContext.Current.Request.Headers["FileName"] ?? String.Empty;
            if (String.IsNullOrEmpty(filex)) return;

            string remotefilepath = @"C:\Users\zfaruqi\Documents\Visual Studio 2015\Projects\FTBShell\FTBShell\UploadedFiles";
            string realfilex = Path.Combine(@remotefilepath, filex);

            var filex2 = filex.Substring(0, filex.IndexOf(".") );
            //getdata all the parts
            byte[] data = Convert.FromBase64String(filex2);
            string decodedString = Encoding.UTF8.GetString(data);

            var tempfilex = decodedString.Split('*');
            string FileShare = tempfilex[0];
            string RootPath = tempfilex[1];
            string filename =@tempfilex[2];
            string fullfilename =@Path.Combine(pretrail(FileShare, RootPath), filename);

            if (!System.IO.File.Exists(@fullfilename))                //System.IO.File.Delete(fullFilePathName.Replace(".bi1", ".bin"));
            System.IO.File.Move(@realfilex, @fullfilename);
        }


        [HttpPost]
        public void WriteDistantLog()
        {
            if (Request.ServerVariables["HTTP_HOST"] != "localhost") return;

            var PortNumber = System.Web.HttpContext.Current.Request.Headers["LogData"] ?? String.Empty;
            var LogEntry = System.Web.HttpContext.Current.Request.Params.ToString() ?? string.Empty;

            var DataTx = System.Web.HttpContext.Current.Request.Headers["DataTx"] ?? "0";
            var DataRx = System.Web.HttpContext.Current.Request.Headers["DataRx"] ?? "0";


            LogEntry = HttpUtility.UrlDecode(LogEntry, System.Text.Encoding.Default);
            LogEntry = LogEntry.Replace("'", " ");
            if (LogEntry.Contains("&ALL_HTTP")) LogEntry = LogEntry.Substring(0, LogEntry.IndexOf("&ALL_HTTP"));

            var testUser = new RimSQL();
            var dataset = testUser.DbDataSet("Select * from PortTrack WHERE PortNumber='" + PortNumber + "'");
            testUser.ExecuteNonQuery("UPDATE PortTrack SET DataRx="+DataRx+", DataTx="+DataTx+", PortNumber='" + "[" + PortNumber + "]" + "', LogEntry='" + LogEntry + "',ExpiresAt='" + DateTime.UtcNow.ToString() + "' WHERE PortNumber='" + PortNumber + "'");

            //get the data
            //try
            //{
            //var dataset = testUser.DbDataSet("Select * from PortTrack WHERE PortNumber='" + PortNumber + "'");
            if (dataset.Tables[0].Rows.Count > 0)
            {
                LogData logdata = new LogData()
                {
                    UserName = dataset.Tables[0].Rows[0]["Username"].ToString(),
                    PortNumber = PortNumber,
                    LogEntry = LogEntry,
                    StartedAt = dataset.Tables[0].Rows[0]["RecdAt"].ToString(),
                    EndsAt = DateTime.UtcNow.ToString(),
                    Company = RimkusHelper.APICompany,
                    DataRx= Convert.ToInt64( DataTx),
                    DataTx = Convert.ToInt64(DataTx)
                };
                //System.IO.File.WriteAllText(@"c:\rimapi\testolog.txt", JsonConvert.SerializeObject(logdata));
                RimkusHelper.SendDataLogtoServer(logdata);
                //System.IO.File.WriteAllText(@"c:\rimapi\testolog2.txt", JsonConvert.SerializeObject(logdata));
            }
            //}
            //catch (Exception eee)
            //{
            //    //throw;
            //}
            // later delete them
            //            testUser.ExecuteNonQuery("INSERT INTO PortTrack (PortNumber , LogEntry, RecdAt) VALUES ('"+PortNumber+"','"+LogEntry+"','"+DateTime.UtcNow.ToString()+"')");
            //            return "OK"
            //write log ge the data here
            //return View();
        }

        private string pretrail(string SharePath,string RootPath)
        {
            RootPath = RootPath.Trim();
            if (RootPath.Length > 1)
            {
                if (RootPath.Contains("\\")) RootPath = RootPath.Replace("\\\\", "\\");
                RootPath = @RootPath.Substring(0, 1) == @"/" ? @RootPath.Substring(1, @RootPath.Length - 1) : @RootPath;
            }


            var FileSharePath = @RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", SharePath.Trim()).Trim();
            if (SharePath.Contains("PERSONALFOLDEROF"))
            {
                //FileSharePath = @RimkusHelper.DataParser("[AbpUsers.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim(); FileSharePath = "\\\\houserver\\users2$\\ZFaruqi";
                FileSharePath = "\\\\houserver\\users\\" + SharePath.Substring(SharePath.IndexOf("OF") +2);
                if (!Directory.Exists(@FileSharePath))
                {
                    FileSharePath = "\\\\houserver\\users2$\\" + SharePath.Substring(SharePath.IndexOf("OF") + 2); 
                }
            }
            else
            {
                FileSharePath = @RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", SharePath.Trim()).Trim();
            }






            
            var tempsharepath = "";
            try
            {
                if (RootPath.Length > 1)
                {
                    tempsharepath = Path.Combine(FileSharePath, @RootPath.Substring(0, 1) == @"\" ? @RootPath.Substring(1, @RootPath.Length - 1) : @RootPath);
                }
                else
                tempsharepath = FileSharePath;// @RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim();
            }
            catch (Exception eee)
            {
                var bola = eee.Message;
                throw;
            }
            if (Directory.Exists(tempsharepath))
            {
                return tempsharepath;
            }
            return String.Empty;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
    }
}