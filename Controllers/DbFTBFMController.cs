using FTBAPISERVER.Application;
using FTBAPISERVER.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace FTBAPISERVER.Controllers
{
   
    public class DbFTBFMController : Controller
    {
        #region Member Variables
        private bool folderaccess = false;
        private const uint SHGFI_TYPENAME = 0x0400;
        private bool IsRootPath = true;
        private bool MasterCall = false;
        private string Userwa = String.Empty;
        private bool IsAuthenticated = false;
        private bool OTPRequired = false;
        private string Company = String.Empty;
        private string MachineCode = String.Empty;
        private string Authenticity = String.Empty;
        private string OTP = String.Empty;
        private FileTxInfo tokendata;
        private string FileShare = "";
        private string FileSharePath = "";
        private string RootPath = "";
        private string AccessType = "Manager";
        private string Parameter1;
        private FileAttr Parameter1Attr;
        private string Parameter2;
        private FileAttr Parameter2Attr;
        private string Parameter3;
        private FileAttr Parameter3Attr;
        private string Parameter4;
        private FileAttr Parameter4Attr;
        private string Parameter5;
        private FileAttr Parameter5Attr;
        private string searchpatternwa = "";
        private string accesslevel = "";

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        #endregion Member Variables

        public DbFTBFMController()
        {

            searchpatternwa = System.Web.HttpContext.Current.Request.Headers["SearchPattern"] ?? string.Empty;

            string moder = "LIVE";
            MasterCall = false;
            if ((System.Web.HttpContext.Current.Request.Headers["SERVERCALL"] ?? string.Empty) != string.Empty && (System.Web.HttpContext.Current.Request.Headers["MachineCode"] ?? string.Empty) != string.Empty)
            {
                MachineCode = System.Web.HttpContext.Current.Request.Headers["MachineCode"];
                Authenticity = System.Web.HttpContext.Current.Request.Headers["SERVERCALL"];
                //if (Authenticity== RimkusHelper.APICompany && MachineCode==RimkusHelper.EndPointTokenCode)
                //{
                RootPath = System.Web.HttpContext.Current.Request.Headers["RootPath"];
                MasterCall = true;
                IsAuthenticated = true;
                return;
                //}
            }
            try
            {
                var tokenrecddata = new TokenData();
                var dummytoken = "bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWlucGgiLCJJRCI6ImFkbWlucGgiLCJJUCI6Ijo6MSIsIkNvbXBhbnkiOiJIb3VzdG9uIiwiTWFjaGluZUNvZGUiOiIxOEE2RjcwQ0YzMEQiLCJBUElNYWNoaW5lQ29kZSI6IjAwNTA1NkFCMkI4QyIsIkxvY2FsaXphdGlvbiI6IiIsIklzTW9iaWxlIjoiRmFsc2UiLCJPcGVyYXRpbmdTeXN0ZW0iOiIiLCJEZXZpY2VUeXBlIjoiVW5rbm93biIsIkFwcGxpY2F0aW9uIjoiIiwiR1VJRCI6IjI4NWQ3YjNkLTFlZmYtNGFiMy05YjY4LThmYTMyMmNjY2ZmMyIsIkNyZWF0aW9uVGltZSI6IjExLzMvMjAxNiAyOjIwOjEwIFBNIiwiRXhwaXJhdGlvblRpbWUiOiIxMS8zLzIwMTYgNDoyMDoxMCBQTSIsInJvbGUiOlsiXCJMb25kb24gSm9icyxBbGxBY2Nlc3M6RmFsc2UsUmVhZE9ubHk6VHJ1ZSxXcml0ZU9ubHk6VHJ1ZSxDYW5EZWxldGU6VHJ1ZSxNYXhGaWxlczowIiwiT3JsYW5kbyBKb2JzLEFsbEFjY2VzczpUcnVlLFJlYWRPbmx5OlRydWUsV3JpdGVPbmx5OlRydWUsQ2FuRGVsZXRlOlRydWUsTWF4RmlsZXM6MSIsIkhvdXN0b24gSm9icyxBbGxBY2Nlc3M6RmFsc2UsUmVhZE9ubHk6VHJ1ZSxXcml0ZU9ubHk6VHJ1ZSxDYW5EZWxldGU6VHJ1ZSxNYXhGaWxlczoyMFwiIl0sImlzcyI6Imh0dHA6Ly9SQ0dTUUwwNC1ELnJpbWt1cy5kZXYvZnRiIiwiYXVkIjoiYWRtaW5waCIsImV4cCI6MTQ3ODIwODAxMCwibmJmIjoxNDc4MjAwODEwfQ.65LmOlfp7RcF96Xnh-vGSPxdlpig6Q0CP-K37ZVWEPk";
                var errorlog = new AbpServerLog();

                if ((System.Web.HttpContext.Current.Request.Headers["DEMO"] ?? string.Empty) != string.Empty)
                {
                    moder = "DEMO";
                }

                if (moder == "DEMO")
                {
                    Company = "Houston";
                    MachineCode = "12345678";
                    Authenticity = dummytoken; ; // string.Empty;
                    FileShare = "London Jobs";
                    RootPath = System.Web.HttpContext.Current.Request.Headers["RootPath"] ?? string.Empty;
                    AccessType = "Manager";
                    Parameter1 = System.Web.HttpContext.Current.Request.Headers["Parameter1"] ?? string.Empty;
                    Parameter2 = System.Web.HttpContext.Current.Request.Headers["Parameter2"] ?? string.Empty;
                    Parameter3 = System.Web.HttpContext.Current.Request.Headers["Parameter3"] ?? string.Empty;
                    Parameter4 = System.Web.HttpContext.Current.Request.Headers["Parameter4"] ?? string.Empty;
                    Parameter5 = System.Web.HttpContext.Current.Request.Headers["Parameter5"] ?? string.Empty;
                    Parameter1Attr = JsonConvert.DeserializeObject<FileAttr>(System.Web.HttpContext.Current.Request.Headers["Parameter1Attr"] ?? string.Empty);
                    Parameter2Attr = JsonConvert.DeserializeObject<FileAttr>(System.Web.HttpContext.Current.Request.Headers["Parameter2Attr"] ?? string.Empty);
                    Parameter3Attr = JsonConvert.DeserializeObject<FileAttr>(System.Web.HttpContext.Current.Request.Headers["Parameter3Attr"] ?? string.Empty);
                    Parameter4Attr = JsonConvert.DeserializeObject<FileAttr>(System.Web.HttpContext.Current.Request.Headers["Parameter4Attr"] ?? string.Empty);
                    Parameter5Attr = JsonConvert.DeserializeObject<FileAttr>(System.Web.HttpContext.Current.Request.Headers["Parameter5Attr"] ?? string.Empty);
                }
                else
                {
                    //Todo: Change it to user
                    AccessType = System.Web.HttpContext.Current.Request.Headers["AccessType"] ?? "User"; // change it to user

                    //AccessType = RimkusHelper.GetuserAccess(Id);

                    Company = System.Web.HttpContext.Current.Request.Headers["Company"];
                    MachineCode = System.Web.HttpContext.Current.Request.Headers["MachineCode"] ?? string.Empty;
                    Authenticity = System.Web.HttpContext.Current.Request.Headers["Authorization"]; // string.Empty;
                    FileShare = System.Web.HttpContext.Current.Request.Headers["FileShare"];
                    RootPath = System.Web.HttpContext.Current.Request.Headers["RootPath"] ?? string.Empty;
                    Parameter1 = System.Web.HttpContext.Current.Request.Headers["Parameter1"] ?? string.Empty;
                    Parameter2 = System.Web.HttpContext.Current.Request.Headers["Parameter2"] ?? string.Empty;
                    Parameter3 = System.Web.HttpContext.Current.Request.Headers["Parameter3"] ?? string.Empty;
                    Parameter4 = System.Web.HttpContext.Current.Request.Headers["Parameter4"] ?? string.Empty;
                    Parameter5 = System.Web.HttpContext.Current.Request.Headers["Parameter5"] ?? string.Empty;
                    Parameter1Attr = JsonConvert.DeserializeObject<FileAttr>(System.Web.HttpContext.Current.Request.Headers["Parameter1Attr"] ?? string.Empty);
                    Parameter2Attr = JsonConvert.DeserializeObject<FileAttr>(System.Web.HttpContext.Current.Request.Headers["Parameter2Attr"] ?? string.Empty);
                    Parameter3Attr = JsonConvert.DeserializeObject<FileAttr>(System.Web.HttpContext.Current.Request.Headers["Parameter3Attr"] ?? string.Empty);
                    Parameter4Attr = JsonConvert.DeserializeObject<FileAttr>(System.Web.HttpContext.Current.Request.Headers["Parameter4Attr"] ?? string.Empty);
                    Parameter5Attr = JsonConvert.DeserializeObject<FileAttr>(System.Web.HttpContext.Current.Request.Headers["Parameter5Attr"] ?? string.Empty);
                }


                Company = WebUtility.HtmlDecode(Company);
                MachineCode = WebUtility.HtmlDecode(MachineCode);
                Authenticity = WebUtility.HtmlDecode(Authenticity);
                FileShare = WebUtility.HtmlDecode(FileShare);
                RootPath = WebUtility.HtmlDecode(RootPath);
                Parameter1 = WebUtility.HtmlDecode(Parameter1);
                Parameter2 = WebUtility.HtmlDecode(Parameter2);
                Parameter3 = WebUtility.HtmlDecode(Parameter3);
                Parameter4 = WebUtility.HtmlDecode(Parameter4);
                Parameter5 = WebUtility.HtmlDecode(Parameter5);





                try
                {
                    Authenticity = Authenticity.Substring(Authenticity.IndexOf(" ") + 1);
                    IsAuthenticated = true;
                }
                catch (Exception eee)
                {
                    IsAuthenticated = false;
                    errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = "UnKnown",
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "Access",
                        Parameters = "bearer token not received",
                        Exception = eee.Message,
                        InfoType = "ERROR",
                        Details = "Token not received"
                    };
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                    return;
                }

                if (RootPath != "")
                {
                    if (RootPath.Substring(0, 1) == "/") RootPath = RootPath.Substring(1);
                }

                var mysecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw";
                var bola = false;
                //var tokenrecddata = new TokenData();
                try
                {
                    var secret = Encoding.ASCII.GetBytes(mysecret);//  new Buffer(mysecret, "base64").ToString();
                    var claims = JWT.JsonWebToken.Decode(Authenticity, mysecret, bola);// false);
                    tokenrecddata = JsonConvert.DeserializeObject<TokenData>(claims);
                }
                catch (Exception e)
                {
                    var tt = e.Message;
                    errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = "Unknown",
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "TokenRead",
                        Parameters = "Token is not valid Reading Error",
                        Exception = "Handled",
                        InfoType = "ERROR",
                        Details = "Token is not valid Reading Error"
                    };
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                    return;
                }
                Userwa = tokenrecddata.unique_name.ToLower();
                AccessType = RimkusHelper.GetuserAccess(Userwa);

                // GetuserAccess(string Id)


                if (tokenrecddata == null)
                {
                    errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = "Unknown",
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "Access",
                        Parameters = "Access Token Error",
                        Exception = "Handled",
                        InfoType = "ERROR",
                        Details = "Does not has a token."
                    };
                    IsAuthenticated = false;
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                    return;
                }

                if (String.IsNullOrEmpty(FileShare))
                {
                    errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = tokenrecddata.unique_name.ToLower(),
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "Access",
                        Parameters = "Share Folder not mentioned",
                        Exception = "Handled",
                        InfoType = "ERROR",
                        Details = "Share Folder not mentioned"
                    };
                    IsAuthenticated = false;
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                    return;
                }

                if (FileShare == "No share active") FileShare = String.Empty;
                if (RootPath == "No share active") RootPath = String.Empty;

                if (String.IsNullOrEmpty(FileShare))
                {
                    errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = tokenrecddata.unique_name.ToLower(),
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "Access",
                        Parameters = "Share Folder not mentioned",
                        Exception = "Handled",
                        InfoType = "ERROR",
                        Details = "Share Folder not mentioned"
                    };
                    IsAuthenticated = false;
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                    return;
                }
                //todo bobby remove this
                //accesslevel = tokenrecddata.Accesslevel; 
                //moder = "DEMO";
                if (moder == "DEMO") tokenrecddata.MachineCode = MachineCode;







                //todo:bobby bobby remove this immdly
                //tokenrecddata.MachineCode = MachineCode;

                if (MachineCode.Trim() != tokenrecddata.MachineCode.Trim())
                {
                    //IsAuthenticated = false;
                    errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = tokenrecddata.unique_name.ToLower(),
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "Access",
                        Parameters = "Machine Code Validation Failed",
                        Exception = "Handled",
                        InfoType = "ERROR",
                        Details = "Machine Code Error.[" + MachineCode + "][" + tokenrecddata.MachineCode.Trim() + "]"
                    };
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                    return;
                }

                var lastToken = RimkusHelper.ComputeCheckSum(System.Text.Encoding.ASCII.GetBytes(Authenticity));
                // read the value of from sql
                var testUser = new RimSQL();
                var tempkey = testUser.Doscalar("SELECT TokenKey from TokenTrack where UserName='" + tokenrecddata.unique_name.ToLower() + "' ORDER BY Id DESC");
                if (tempkey == "") lastToken = tempkey; // will happen if working against another server

                if (moder == "DEMO") lastToken = tempkey;
                //todo:bobby bobby remove this immdly
                lastToken = tempkey;


                if (lastToken != tempkey)
                {
                    IsAuthenticated = false;
                    errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = tokenrecddata.unique_name.ToLower(),
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "Access",
                        Parameters = "Token Validation Failed",
                        Exception = "Handled",
                        InfoType = "ERROR",
                        Details = "Token is not validated. " + tempkey + " [" + lastToken + "]"
                    };
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                    return;
                }

                //var claims = JWT.JsonWebToken.Decode(Authenticity, mysecret, bola);// false);

                //tokenrecddata = JsonConvert.DeserializeObject<TokenData>(claims);


                //  enable it soonest
                if (moder != "DEMO")
                {
                    IsAuthenticated = false;
                    if (tokenrecddata.role != null)
                    {
                        foreach (var item in tokenrecddata.role)
                        {
                            if (item.Contains(FileShare))
                            {
                                IsAuthenticated = true;
                                break;
                            }
                        }
                    }
                }


                if (!IsAuthenticated)
                {
                    errorlog = new AbpServerLog()
                    {
                        LoginTime = DateTime.Now,
                        UserName = tokenrecddata.unique_name.ToLower(),
                        TenantName = RimkusHelper.APICompany,
                        MethodName = "Access",
                        Parameters = "Share is not mentioned in Token",
                        Exception = "Handled",
                        InfoType = "ERROR",
                        Details = "Share is not assessible."
                    };
                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                    return;
                }

                if (moder != "DEMO" && !String.IsNullOrEmpty(FileShare))
                {
                    if (!Directory.Exists(Path.Combine(RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim())))
                    {
                        IsAuthenticated = false;
                        errorlog = new AbpServerLog()
                        {
                            LoginTime = DateTime.Now,
                            UserName = tokenrecddata.unique_name.ToLower(),
                            TenantName = RimkusHelper.APICompany,
                            MethodName = "Access",
                            Parameters = FileShare,
                            Exception = "Handled",
                            InfoType = "ERROR",
                            Details = "Folder does not exists or has been removed."
                        };
                        RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                        return;
                    }
                }

                if (moder != "DEMO" && !String.IsNullOrEmpty(RootPath))
                {
                    if (!Directory.Exists(Path.Combine(RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim(), @RootPath)))
                    {
                        IsAuthenticated = false;
                        errorlog = new AbpServerLog()
                        {
                            LoginTime = DateTime.Now,
                            UserName = tokenrecddata.unique_name.ToLower(),
                            TenantName = RimkusHelper.APICompany,
                            MethodName = "Access",
                            Parameters = RootPath,
                            Exception = "Handled",
                            InfoType = "ERROR",
                            Details = "Sub Folder does not exists or has been removed."
                        };
                        //Todo Bobby bring it back
                        //RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                        //return;
                    }
                }

                //Userwa = tokenrecddata.unique_name ?? "TestUser";//  "zfaruqi";


                if (moder == "DEMO") Userwa = "TestUser";
                if (tokenrecddata.ExpirationTime == null) tokenrecddata.ExpirationTime = DateTime.Now.AddMinutes(30).ToString();

                if (moder == "DEMO") tokenrecddata.ExpirationTime = DateTime.Now.AddMinutes(100).ToString();



                //todo bobby enable this immdeiatly to check time

                /*                if (Convert.ToDateTime(tokenrecddata.ExpirationTime) < DateTime.Now)
                                {
                                    IsAuthenticated = false;
                                    errorlog = new AbpServerLog()
                                    {
                                        LoginTime = DateTime.Now,
                                        UserName = tokenrecddata.unique_name.ToLower(),
                                        TenantName = RimkusHelper.APICompany,
                                        MethodName = "Expired",
                                        Parameters = RootPath,
                                        Exception = "Handled",
                                        InfoType = "ERROR",
                                        Details = "Token has expired."
                                    };
                                    // do a revoke
                                    RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(errorlog));
                                    return;
                                }

                */
                tokendata = new Models.FileTxInfo()
                {
                    User = Userwa,
                    Company = Company,
                    Authenticity = Authenticity,
                    OTP = "",
                    FileShare = FileShare,
                    RootPath = RootPath.Length == 1 ? "" : RootPath,
                    Parameter1 = Parameter1,
                    Parameter1Attr = Parameter1Attr,
                    Parameter2 = Parameter2,
                    MachineCode = MachineCode,
                    Parameter2Attr = Parameter3Attr,
                    Parameter3 = Parameter3,
                    Parameter3Attr = Parameter3Attr,
                    Parameter4 = Parameter4,
                    Parameter4Attr = Parameter3Attr,
                    Parameter5 = (IsAuthenticated == false) ? "Authentication Failed" : Parameter5,
                    Parameter5Attr = Parameter5Attr,
                    ValidTime = Convert.ToDateTime(tokenrecddata.ExpirationTime) //  DateTime.Now.AddSeconds(30) // +  new TimeSpan(0, 2, 0, 0) // change this to get it from the token.
                };
            }
            catch (Exception eeee)
            {
                var verrorlog = new AbpServerLog()
                {
                    LoginTime = DateTime.Now,
                    UserName = "User",
                    TenantName = RimkusHelper.APICompany,
                    MethodName = "Fatal",
                    Parameters = "Generic",
                    Exception = "Handled",
                    InfoType = "ERROR",
                    Details = eeee.Message
                };
                RimkusHelper.SendLogToServer(JsonConvert.SerializeObject(verrorlog));
                IsAuthenticated = false;
            }
        }

        public string HELO()
        {
            var bola = new StreamFilesController().GetFileMetaData(@"c:\test\bola1.ico");
            return "Helo from FTB Server at " + DateTime.Now.ToLongDateString();
        }

        public bool getfolderaccess()
        {

            return false;

        }
        // GET: FTBFM
        public ActionResult Index(string id)
        {
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();
            if (!IsAuthenticated)
            {
                // return Content("");// Authentication Failed - In Production you will receive a null string.");
            }
            return Content(String.Empty);
        }

        public ActionResult justtest()
        {
            try
            {
                var path = @"C:/TestShareRoot/Share 2/Orlando Open Jobs";
                DynatreeItem di = new DynatreeItem(new System.IO.DirectoryInfo(@path), "tester", true);
                //string result = "[" + di.JsonToDynatree() + "]";
                string result = di.JsonToDynatree();
                //return result;
                return Content(result, "application/json", Encoding.UTF8);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult SendFile(string id)//HttpPostedFileBase files)
        {
            //var bolax = new StreamFilesController().GetFileMetaData(@"c:\test\bola1.ico");
            //return Content(bolax);

            // var bolan = files.FileName;
            try
            {
                var path = pretrail();
                var bola = Request.Files;
                var filename = bola[0].FileName;
                var saveaspath = Path.Combine(@path, filename);
                if (saveaspath.Contains("\\"))
                {
                    // check for directory
                    var tempdir = saveaspath.Substring(0, saveaspath.LastIndexOf("\\"));
                    if (!System.IO.Directory.Exists(tempdir))
                    {
                        System.IO.Directory.CreateDirectory(tempdir);
                    }

                }

                bola[0].SaveAs(Path.Combine(@path, filename));
                //get the fileinfo
                FileInfo myfileinfo = new FileInfo(Path.Combine(@path, filename));
                AbpFolders oldrec = RimkusHelper.dbGetCommand("Select * from AbpFolders where Id =" + id + " FOR JSON AUTO");
                if (oldrec.Id > 0)
                {
                    RimkusHelper.dbCommandExecute("INSERT INTO AbpDocuments(FolderId ,DocumentName,DocumentSize,DocumentJob,DocumentDate,DocumentAccess,DocumentOffice,DocumentType)   VALUES(" + id + ",'" + filename + "','" + myfileinfo.Length.ToString() + "','" + oldrec.FolderJob + "','" + myfileinfo.CreationTimeUtc.ToString() + "','" + "+R+E+D" + "','" + oldrec.FolderOffice + "','" + myfileinfo.Extension.ToLower() + "')");
                }


                justapilog("File uploaded " + @saveaspath, bola[0].InputStream.Length, 0);

                return Content("File uploaded successfully");
                //bola[0].SaveAs(@"C:\test\bolababy"+Guid.NewGuid().ToString() +".apk");
                //if (files == null || files.Count != 2)
                //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch (Exception eee)
            {
                return Content("File upload failed.");
                throw;
            }
            return Content("OK");
        }




        public async Task<string> createconnections()
        {
            int randomNumber = 0;
            try
            {
                //var tempo = RimkusHelper.RimTerm(randomNumber.ToString());
                Random random = new Random();

                if (!IsAuthenticated)
                {
                    return "";// Authentication Failed  );
                }
                if (tokendata.ValidTime < DateTime.Now) IsAuthenticated = false;
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
                var temptoken = JsonConvert.SerializeObject(tokendata);
                temptoken = temptoken.Replace("'", "''");
                testUser.ExecuteNonQuery("INSERT INTO PortTrack (UserName, PortNumber , TokenKey, RecdAt) VALUES ('" + tokendata.User + "','" + randomNumber.ToString() + "','" + temptoken + "','" + DateTime.UtcNow.ToString() + "')");
                System.Threading.Thread.Sleep((int)System.TimeSpan.FromSeconds(1).TotalMilliseconds);

                var opennewport = await RimkusHelper.RimTerm(randomNumber.ToString(), temptoken, tokendata.User, tokendata.Company);

                //                if (RimkusHelper.RimTerm(randomNumber.ToString(), temptoken, tokendata.User, tokendata.Company) == "OK")

                if (opennewport == "OK")
                {
                    System.Threading.Thread.Sleep((int)System.TimeSpan.FromSeconds(1).TotalMilliseconds);
                    return "220 Service ready for user " + tokendata.User + " on Port " + randomNumber;
                }
                else
                {
                    return "500 Error in opening port for user " + tokendata.User + " on Port " + randomNumber;
                }

                //return Content("220 Service ready for user " + tokendata.User + " on Port " + randomNumber);
            }
            catch (Exception rrr)
            {
                return "500 Port Error  " + rrr.Message;
            }
            //Todo: verify user now
        }



        public ActionResult FileMenu(string id, string data = "")
        {
            //Response.ClearHeaders();
            //Response.ClearContent();
            //Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            string realroot = "";
            string file1 = "";
            string file2 = "";
            string myid = "";
            var lastindex = "";
            //todo bobby enable these line immdly

            /*
            if (!IsAuthenticated)
            {
                return Content("TRY AGAIN");// Authentication Failed - In Production you will receive a null string.");
            }
            if (tokendata.ValidTime < DateTime.Now) { IsAuthenticated = false; return Content("TOKEN EXPIRED"); }
            */

            data = data.ToLower();
            data = data.Replace("star", "*");
            data = data.Replace("dot", ".");

            id = id.ToLower();
            RimkusHelper.WriteLocalLog(Userwa, id + " " + pretrail() + " " + data, false);
            try
            {
                switch (id)
                {
                    case "getdir":
                        if (MasterCall)
                            return Content(GetMasterFoldwithInfo(), "application/json", Encoding.UTF8);
                        else
                            return Content("");

                    case "getdirfiles":


                        return Content(getdirfiles(), "application/json", Encoding.UTF8);

                    case "getsubdir":
                        return Content(getsubdir(), "application/json", Encoding.UTF8);

                    case "getsubdirfiles":
                        return Content(getsubdirfiles(), "application/json", Encoding.UTF8);

                    case "getallshares":

                        return Content(getAllSharesFromServer(), "application/json", Encoding.UTF8);

                    case "getdirectorywithinfo":
                        return Content(GetDirectoryWithInfo(), "application/json", Encoding.UTF8);

                    case "getfileinfo":
                        return Content("Test");

                    case "searchfolder":
                        return Content("Test");

                    case "searchfile":
                        return Content("Test");

                    case "createfolder":
                        var pathcreate1 = "";
                        try
                        {
                            if (Parameter1.Contains(",")) return Content("450 Create Folder parameter not correct");
                            if (String.IsNullOrEmpty(Parameter1.Trim())) return Content("450 Create folder parameter not correct");
                            if (RootPath == "") return Content("Cannot Create Folder in Root File Share");
                            myid = RimkusHelper.GetBetween(RootPath, "[", "]");

                            if (RootPath.Contains("Personal Folder^"))
                            {
                                realroot = RimkusHelper.dbScalar("Select FolderName from AbpFolders where Id="+myid);// .dbgetdirpath(RimkusHelper.GetBetween(RootPath, "[", "]"));//   pretrail().Trim();
                            }
                                else
                            {
                                realroot = RimkusHelper.dbgetdirpath(RimkusHelper.GetBetween(RootPath, "[", "]"));//   pretrail().Trim();
                                if (realroot == "") return Content("450 operation failed");
                                lastindex = RootPath.Substring(RootPath.LastIndexOf('\\') + 1);
                                lastindex = lastindex.Substring(0, lastindex.IndexOf("^") - 1).Trim();
                                realroot = Path.Combine(realroot, lastindex);
                            }
                            //realroot = RimkusHelper.dbgetdirpath(RimkusHelper.GetBetween(RootPath, "[", "]"));//   pretrail().Trim();
                           // if (realroot == "") return Content("450 operation failed");
                            //lastindex = RootPath.Substring(RootPath.LastIndexOf('\\') + 1);
                            //lastindex = lastindex.Substring(0, lastindex.IndexOf("^") - 1).Trim();
                            //realroot = Path.Combine(realroot, lastindex);



                            //string foldername = realroot.Substring(RootPath.LastIndexOf("\\") + 1);
                            //string folderbola = RimkusHelper.CanAccess(foldername);
                            var folderbola = Userwa.ToUpper();

                            pathcreate1 = Path.Combine(realroot, Parameter1);

                            if (Directory.Exists(pathcreate1))
                                {
                                return Content("450 Folder "+Parameter1 +" already exists.");
                            }

                            System.IO.Directory.CreateDirectory(pathcreate1);

                            AbpFolders oldrec = RimkusHelper.dbGetCommand("Select * from AbpFolders where Id =" + myid + " FOR JSON AUTO");
                            if (oldrec.Id > 0)
                            {
                                RimkusHelper.dbCommandExecute("INSERT INTO AbpFolders  (FolderDate , FolderJob , ParentFolderID, Foldername,FolderRoot,FolderOffice,FolderRank) OUTPUT Inserted.Id VALUES('" + DateTime.Now.ToString() + "','" + oldrec.FolderJob + "'," + myid + ",'" + Parameter1 + "','" + oldrec.FolderRoot + "','" + oldrec.FolderOffice + "'," + "3" + ")");
                            }

                            justapilog("Created Folder. Please refresh folder tree. " + @pathcreate1);

                        }
                        catch
                        {
                            RimkusHelper.WriteLocalLog(Userwa, "Folder creation error " + pathcreate1);
                            return Content("450 Directory cannot be created");
                        }
                        RimkusHelper.WriteLocalLog(Userwa, "Folder created " + pathcreate1);
                        return Content("220 Folder created " + Parameter1);

                    case "renamefolder":
                        try
                        {





                            if (Parameter1.Contains(",")) return Content("450 Rename from Rename to parameters not correct");
                            if (Parameter2.Contains(",")) return Content("450 Rename from Rename to parameters not correct");

                            if (String.IsNullOrEmpty(Parameter1.Trim())) return Content("450 Rename from Rename to parameters not correct");
                            //if (String.IsNullOrEmpty(Parameter2.Trim())) return Content("450 Rename from Rename to parameters not correct");



                            if (Parameter1.Contains(",")) return Content("450 Delete Folder parameter not correct");
                            if (String.IsNullOrEmpty(Parameter1.Trim())) return Content("450 Create folder parameter not correct");
                            if (RootPath == "") return Content("Cannot Create Folder in Root File Share");
                            myid = RimkusHelper.GetBetween(RootPath, "[", "]");
                            realroot = RimkusHelper.dbgetdirpath(RimkusHelper.GetBetween(RootPath, "[", "]"));//   pretrail().Trim();
                            if (realroot == "") return Content("450 operation failed");
                            lastindex = RootPath.Substring(RootPath.LastIndexOf('\\') + 1);
                            lastindex = lastindex.Substring(0, lastindex.IndexOf("^") - 1).Trim();
                            realroot = Path.Combine(realroot, lastindex);

















                            //realroot = pretrail().Trim();
                            if (realroot == "") return Content("450 operation failed, Folder does not exists or access denial. Please refresh Foldr Tree and try again.");

                            if (!System.IO.Directory.Exists(realroot))
                            {
                                RimkusHelper.dbCommandExecute("DELETE FROM AbpFolders  WHERE ID=" + myid);
                                return Content("450 Folder to rename does not exists.Removed.");

                            }



                            string foldername = realroot.Substring(RootPath.LastIndexOf("\\") + 1);
                            //string folderbola = RimkusHelper.CanAccess(foldername);
                            var folderbola = Userwa.ToUpper();






                            ///if (IsRootPath) return Content("450 Attempt to change Root Share failed.");

                            // string lastFolderName = Path.GetFileName(realroot.TrimEnd(Path.DirectorySeparatorChar).TrimEnd(Path.AltDirectorySeparatorChar));

                            // if (lastFolderName != Parameter1)
                            // {
                            //     return Content("450 Parameter1 must contain the name of the Folder to rename");
                            // }

                            var pathTo = realroot.Substring(0, realroot.LastIndexOf("\\") + 1) + Parameter1;// Index   realroot.Replace(Parameter1, Parameter2);
                            if (Directory.Exists(@pathTo))
                            {
                                return Content("450 Destination Directory Exists. Please select another name");
                            }

                            try
                            {
                                Directory.Move(realroot, pathTo);
                                RimkusHelper.WriteLocalLog(Userwa, "Folder renamed  from " + realroot + " to" + pathTo);
                                RimkusHelper.dbCommandExecute("UPDATE AbpFolders SET FolderName='" + Parameter1 + "'  WHERE ID=" + myid);

                                justapilog("Folder Refresh Required. Folder renamed  from " + realroot + " to" + pathTo);

                            }
                            catch

                            {
                                RimkusHelper.WriteLocalLog(Userwa, "Folder rename error  from " + realroot + " to" + pathTo);
                                return Content("450 Directory could not be renamed. Errors");
                            }
                        }
                        catch
                        {
                            return Content("450 Directory Does Not Exists Cannot Rename");
                        }
                        return Content("220 Folder renamed successfully.Please Refesh Folder Tree");

                    case "renamefile":
                        try
                        {
                            if (Parameter1.Contains(",")) return Content("450 Rename from Rename to parameters not correct");
                            if (Parameter2.Contains(",")) return Content("450 Rename from Rename to parameters not correct");

                            myid = Parameter1;// RimkusHelper.GetBetween(RootPath, "[", "]");

                            realroot = RimkusHelper.dbgetfilepath(Parameter1);//  pretrail().Trim();

                            //realroot = pretrail();
                            if (realroot == "") return Content("450 operation failed");

                            file1 = realroot;// Path.Combine(realroot, Parameter1);
                            //file2 = Path.Combine(realroot, Parameter2);
                            file2 = realroot.Substring(0, realroot.LastIndexOf("\\") + 1) + Parameter2;
                            if (!System.IO.File.Exists(file1))
                            {
                                RimkusHelper.dbCommandExecute("UPDATE AbpDocuments SET DocumentName='" + Parameter2 + "'  WHERE ID=" + myid);
                                return Content("450 File to rename does not exists.");
                            }

                            if (System.IO.File.Exists(file2)) return Content("450 File destination exists, choose another name for new file.");

                            try
                            {
                                System.IO.File.Move(file1, file2);

                                RimkusHelper.dbCommandExecute("UPDATE AbpDocuments SET DocumentName='" + Parameter2 + "'  WHERE ID=" + myid);


                                RimkusHelper.WriteLocalLog(Userwa, "File renamed  from " + file1 + " to" + file2);
                                justapilog("File renamed  from " + file1 + " to" + file2);

                                return Content("220 File renamed successfully.");
                            }
                            catch (Exception ccc)
                            {
                                return Content("450 File error , cannot be renamed.");//  +ccc.Message);
                            }
                        }
                        catch
                        {
                            return Content("450 File destination exists, choose another name for new file.");
                        }

                    case "movefolder":
                        try
                        {
                            if (Parameter1.Contains(",")) return Content("450 Move from Move to parameters not correct");
                            if (Parameter2.Contains(",")) return Content("450 Move from Move to parameters not correct");

                            if (String.IsNullOrEmpty(Parameter1.Trim())) return Content("450 Move from Move to parameters not correct");
                            if (String.IsNullOrEmpty(Parameter2.Trim())) return Content("450 Move from Move to parameters not correct");

                            realroot = pretrail().Trim();
                            if (realroot == "") return Content("450 operation failed");

                            if (IsRootPath) return Content("450 Attempt to move Root Share failed.");

                            string lastFolderName = Path.GetFileName(realroot.TrimEnd(Path.DirectorySeparatorChar).TrimEnd(Path.AltDirectorySeparatorChar));

                            if (lastFolderName != Parameter1)
                            {
                                return Content("450 Parameter1 must contain the name of the Folder to move");
                            }

                            var pathTo = realroot.Replace(Parameter1, Parameter2);
                            if (Directory.Exists(@pathTo))
                            {
                                return Content("450 Destination Directory Exists. Please select another name");
                            }

                            try
                            {
                                Directory.Move(realroot, pathTo);
                                RimkusHelper.WriteLocalLog(Userwa, "Folder moved  from " + realroot + " to" + pathTo);
                                justapilog("Folder moved  from " + realroot + " to" + pathTo);

                            }
                            catch
                            {
                                return Content("450 Directory could not be move. Errors");
                            }
                        }
                        catch
                        {
                            return Content("450 Directory Does Not Exists Cannot move");
                        }
                        return Content("220 Folder renamed successfully");

                    case "movefiles":
                        try
                        {
                            if (Parameter1.Contains(",")) return Content("450 Move from Move to parameters not correct");
                            if (Parameter2.Contains(",")) return Content("450 Move from Move to parameters not correct");

                            realroot = pretrail();
                            if (realroot == "") return Content("500 operation failed");

                            file1 = Path.Combine(realroot, Parameter1);
                            file2 = Path.Combine(realroot, Parameter2);

                            if (!System.IO.File.Exists(file1)) return Content("450 File to move does not exists.");

                            if (System.IO.File.Exists(file2)) return Content("450 File destination exists, choose another name for new file.");

                            try
                            {
                                System.IO.File.Move(file1, file2);
                                RimkusHelper.WriteLocalLog(Userwa, "File moved  from " + file1 + " to" + file2);
                                justapilog("File moved  from " + @file1 + " to" + @file2);
                                return Content("220 File moved successfully.");
                            }
                            catch
                            {
                                return Content("450 File error , cannot be move.");
                            }
                        }
                        catch
                        {
                            return Content("450 File destination exists, choose another name for new file.");
                        }

                    case "deletefolder":
                        if (Parameter1.Contains(",")) return Content("450 Delete Folder parameter not correct");
                        if (String.IsNullOrEmpty(Parameter1.Trim())) return Content("450 Create folder parameter not correct");
                        if (RootPath == "") return Content("Cannot Create Folder in Root File Share");
                        myid = RimkusHelper.GetBetween(RootPath, "[", "]");
                        realroot = RimkusHelper.dbgetdirpath(RimkusHelper.GetBetween(RootPath, "[", "]"));//   pretrail().Trim();
                        if (realroot == "") return Content("450 operation failed");
                        lastindex = RootPath.Substring(RootPath.LastIndexOf('\\') + 1);
                        lastindex = lastindex.Substring(0, lastindex.IndexOf("^") - 1).Trim();
                        realroot = Path.Combine(realroot, lastindex);





                        //realroot = pretrail().Trim();
                        if (realroot == "") return Content("450 operation failed");
                        //if (IsRootPath) return Content("450 Attempt to move Root Share failed.");




                        try
                        {

                            if (String.IsNullOrEmpty(realroot.Trim()))
                            {
                                return Content("450 You do not have a valid folder name.");
                            }

                            if (!Directory.Exists(realroot))
                            {
                                RimkusHelper.dbCommandExecute("DELETE FROM AbpFolders  WHERE ID=" + myid);
                                return Content("220 You do not have a valid folder name.");
                            }

                            if (Directory.EnumerateFileSystemEntries(realroot).Any())
                            {
                                return Content("450 Folder is not empty.");
                            }






                            Directory.Delete(@realroot, true);
                            var result = "220 Folder deleted successfully";
                            RimkusHelper.dbCommandExecute("DELETE FROM AbpFolders  WHERE ID=" + myid);




                            justapilog("Deleted Folder " + @realroot);

                            return Content(result);
                        }
                        catch
                        {
                            return Content("450 You do not have a valid folder name.");
                        }
                    case "deletefile":

                        myid = Parameter1;// RimkusHelper.GetBetween(RootPath, "[", "]");

                        realroot = RimkusHelper.dbgetfilepath(Parameter1);//  pretrail().Trim();
                        if (realroot == "") return Content("500 operation failed");

                        file1 = realroot; // Path.Combine(realroot, Parameter1);

                        try
                        {
                            if (String.IsNullOrEmpty(file1.Trim()))
                            {
                                RimkusHelper.dbCommandExecute("DELETE FROM AbpDocuments  WHERE ID=" + myid);
                                return Content("450 You do not have a valid file name.");

                            }
                            if (!System.IO.File.Exists(file1.Trim()))
                            {
                                return Content("450 You do not have a valid file name.");
                            }

                            System.IO.File.Delete(@file1);
                            RimkusHelper.dbCommandExecute("DELETE FROM AbpDocuments WHERE ID=" + myid);
                            var result = "220 File deleted successfully";
                            justapilog("Deleted File " + @file1);

                            return Content(result);
                        }
                        catch
                        {
                            return Content("450 You do not have a valid folder name.");
                        }

                    case "copyfolder":
                        try
                        {
                            if (String.IsNullOrEmpty(Parameter1.Trim())) return Content("450 Copy from Copy to parameters not correct");
                            if (String.IsNullOrEmpty(Parameter2.Trim())) return Content("450 Copy from Copy to parameters not correct");

                            //if (Parameter1.Contains(",")) return Content("450 Copy from Copy to parameters not correct");
                            //if (Parameter2.Contains(",")) return Content("450 Copy from Copy to parameters not correct");
                            // now let us get the folderfrom
                            RCGRemoteFile bola = RimkusHelper.JustName(Parameter1);
                            if (bola.FileShare == "") return Content("450 Folder Action error");
                            FileShare = bola.FileShare;
                            RootPath = "";// bola.RootPath;// .FileShare;// ""; // bola.RootPath;
                            var path = pretrail();





                            realroot = path.Trim();
                            if (realroot == "") return Content("450 operation failed");

                            //string foldername = Parameter1.Substring(Parameter1.LastIndexOf("\\") + 1);
                            //string folderbola = RimkusHelper.CanAccess(foldername);
                            var folderbola = Userwa.ToUpper();
                            if (Parameter1.Count(c => c == '\\') == 1)
                            {
                                return Content("450 operation failed");
                            }













                            string xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                            if (!System.IO.Directory.Exists(xfilex))
                                xfilex = @Path.Combine(path, bola.RootPath);
                            if (!System.IO.Directory.Exists(xfilex))
                                xfilex = @Path.Combine(path, bola.RootPath.Substring(0, bola.RootPath.LastIndexOf("\\")));


                            if (!System.IO.Directory.Exists(xfilex)) return Content("450 Folder Error");

                            var folderfrom = xfilex;
                            var myfilename = bola.Filename;
                            if (myfilename.Contains("\\"))
                                myfilename = myfilename.Substring(myfilename.LastIndexOf("\\"));


                            bola = RimkusHelper.JustName(Parameter2);
                            if (bola.FileShare == "") return Content("450 Folder Action error");
                            FileShare = bola.FileShare;
                            RootPath = "";// bola.RootPath;// .FileShare;// ""; // bola.RootPath;
                            path = pretrail();
                            xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                            if (!System.IO.Directory.Exists(xfilex))
                                xfilex = @Path.Combine(path, bola.RootPath);
                            if (!System.IO.Directory.Exists(xfilex))
                                xfilex = @Path.Combine(path, bola.RootPath.Substring(0, bola.RootPath.LastIndexOf("\\")));


                            if (!System.IO.Directory.Exists(xfilex)) return Content("450 Folder Error");

                            var folderto = xfilex;

                            if (!String.IsNullOrEmpty(myfilename))
                                folderto = xfilex + myfilename;


                            try
                            {
                                DirectoryCopy(folderfrom, folderto, true);
                                RimkusHelper.WriteLocalLog(Userwa, "Folder copied  from " + folderfrom + " to " + folderto);
                                justapilog("Folder copied   from " + @folderfrom + " to" + @folderto);

                            }
                            catch
                            {
                                return Content("450 Directory could not be copied. Errors");
                            }
                        }
                        catch
                        {
                            return Content("450 Directory Does Not Exists Cannot copy");
                        }
                        return Content("220 Folder copied successfully");

                    case "copyfiles":
                        try
                        {
                            if (String.IsNullOrEmpty(Parameter1.Trim())) return Content("450 Copy from Copy to parameters not correct");
                            if (String.IsNullOrEmpty(Parameter2.Trim())) return Content("450 Copy from Copy to parameters not correct");

                            //if (Parameter1.Contains(",")) return Content("450 Copy from Copy to parameters not correct");
                            //if (Parameter2.Contains(",")) return Content("450 Copy from Copy to parameters not correct");
                            // now let us get the folderfrom
                            RCGRemoteFile bola = RimkusHelper.JustName(Parameter1);
                            if (bola.FileShare == "") return Content("450 Folder Action error");
                            FileShare = bola.FileShare;
                            var myfilename = bola.Filename;
                            if (myfilename.Contains("\\"))
                                myfilename = myfilename.Substring(myfilename.LastIndexOf("\\"));

                            RootPath = "";// bola.RootPath;// .FileShare;// ""; // bola.RootPath;
                            var path = pretrail();
                            string xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                            if (!System.IO.File.Exists(xfilex))
                                xfilex = @Path.Combine(path, bola.RootPath);
                            if (!System.IO.File.Exists(xfilex))
                                xfilex = @Path.Combine(path, bola.RootPath.Substring(0, bola.RootPath.LastIndexOf("\\")));


                            if (!System.IO.File.Exists(xfilex)) return Content("450 File not found");

                            var folderfrom = xfilex;

                            bola = RimkusHelper.JustName(Parameter2);
                            if (bola.FileShare == "") return Content("450 Folder Action error");
                            FileShare = bola.FileShare;
                            RootPath = "";// bola.RootPath;// .FileShare;// ""; // bola.RootPath;
                            path = pretrail();
                            xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                            if (!System.IO.Directory.Exists(xfilex))
                                xfilex = @Path.Combine(path, bola.RootPath);
                            if (!System.IO.Directory.Exists(xfilex))
                                xfilex = @Path.Combine(path, bola.RootPath.Substring(0, bola.RootPath.LastIndexOf("\\")));


                            if (!System.IO.Directory.Exists(xfilex)) return Content("450 Folder Error");

                            var folderto = xfilex;
                            file2 = folderto + myfilename;



                            //file2= @Path.Combine(folderto, myfilename);

                            if (!System.IO.File.Exists(folderfrom)) return Content("450 File to copy does not exists.");

                            if (System.IO.File.Exists(file2)) return Content("450 File destination exists, choose another name for new file.");

                            try
                            {
                                System.IO.File.Copy(folderfrom, file2);
                                RimkusHelper.WriteLocalLog(Userwa, "File copied from " + folderfrom + " to" + file2);
                                justapilog("File copied from " + @folderfrom + " to" + @file2);

                                return Content("220 File copied successfully.");
                            }
                            catch
                            {
                                return Content("450 File error , cannot be copy.");
                            }
                        }
                        catch
                        {
                            return Content("450 File destination exists, choose another name for new file.");
                        }

                    case "copyasfiles":
                        try
                        {
                            if (String.IsNullOrEmpty(Parameter1.Trim())) return Content("450 Copy from Copy to parameters not correct");
                            if (String.IsNullOrEmpty(Parameter2.Trim())) return Content("450 Copy from Copy to parameters not correct");

                            //if (Parameter1.Contains(",")) return Content("450 Copy from Copy to parameters not correct");
                            //if (Parameter2.Contains(",")) return Content("450 Copy from Copy to parameters not correct");
                            // now let us get the folderfrom
                            RCGRemoteFile bola = RimkusHelper.JustName(Parameter1);
                            if (bola.FileShare == "") return Content("450 Folder Action error");
                            FileShare = bola.FileShare;
                            var myfilename = bola.Filename;
                            if (myfilename.Contains("\\"))
                                myfilename = myfilename.Substring(myfilename.LastIndexOf("\\"));

                            RootPath = "";// bola.RootPath;// .FileShare;// ""; // bola.RootPath;
                            var path = pretrail();
                            string xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                            if (bola.RootPath.Contains("."))
                            {
                                file2 = path + bola.RootPath.Insert(bola.RootPath.LastIndexOf("."), "-[" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "]");
                            }
                            else
                            {
                                file2 = path + bola.RootPath.Insert(bola.RootPath.LastIndexOf("."), "-[" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "]");
                            }

                            //file2 = path + bola.RootPath;
                            if (!System.IO.File.Exists(xfilex))
                                xfilex = @Path.Combine(path, bola.RootPath);
                            if (!System.IO.File.Exists(xfilex))
                                xfilex = @Path.Combine(path, bola.RootPath.Substring(0, bola.RootPath.LastIndexOf("\\")));




                            if (!System.IO.File.Exists(xfilex)) return Content("450 File not found");

                            var folderfrom = xfilex;


                            //file2 = folderfrom.replace;



                            //file2= @Path.Combine(folderto, myfilename);

                            if (!System.IO.File.Exists(folderfrom)) return Content("450 File to copy does not exists.");

                            if (System.IO.File.Exists(file2))
                            {
                                if (bola.RootPath.Contains("."))
                                {
                                    file2 = path + bola.RootPath.Insert(bola.RootPath.LastIndexOf("."), "-[" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ssss") + "]");
                                }
                                else
                                {
                                    file2 = path + bola.RootPath.Insert(bola.RootPath.LastIndexOf("."), "-[" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ssss") + "]");
                                }
                            }

                            try
                            {
                                System.IO.File.Copy(folderfrom, file2);
                                RimkusHelper.WriteLocalLog(Userwa, "File copied from " + folderfrom + " to" + file2);
                                justapilog("Files copied  from " + @folderfrom + " to" + @file2);

                                return Content("220 File copied successfully.");
                            }
                            catch (Exception eee)
                            {
                                return Content("450 File error , cannot be copy." + folderfrom + " " + file2 + " " + eee.Message);
                            }
                        }
                        catch (Exception eee)
                        {
                            return Content("450 File destination exists, choose another name for new file." + eee.Message);
                        }

                    case "mobiledir":

                        return Content(mobiledir(), "application/json", Encoding.UTF8);

                    case "getfilefromshare":
                        string directoryPath = pretrail();
                        string fileName = Parameter1;
                        justapilog("Request file from share " + @directoryPath);

                        return File(directoryPath, "multipart/form-data", fileName);

                    case "mobiledownload":
                        var fileinbytes = getFile();
                        //byte[] originalArray = new byte[32];
                        //rng.GetBytes(key);
                        string toEncode = Convert.ToBase64String(fileinbytes);
                        //byte[] temp_backToBytes = Encoding.UTF8.GetBytes(temp_inBase64);
                        //var toEncode = Encoding.ASCII.GetString(fileinbytes);
                        return Content(toEncode);

                    case "downloadfile":
                        var xfileinbytes = getFile();
                        //byte[] originalArray = new byte[32];
                        //rng.GetBytes(key);
                        string xtoEncode = Convert.ToBase64String(xfileinbytes);
                        //byte[] temp_backToBytes = Encoding.UTF8.GetBytes(temp_inBase64);
                        //var toEncode = Encoding.ASCII.GetString(fileinbytes);
                        return Content(xtoEncode);

                    case "list":
                        // check if folder exists
                        return Content(GetList(data), "application/json", Encoding.UTF8);

                    case "getrootfolder":
                        // check if folder exists
                        return Content(GetFoldwithInfo(), "application/json", Encoding.UTF8);

                    case "getfolderwithfiles":
                        // check if folder exists
                        return Content(GetFoldwithInfo(), "application/json", Encoding.UTF8);
                }
            }
            catch (HttpException err)
            {
                return Content(Error(err.Message), "application/json", Encoding.UTF8);
            }
            return View();
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            //important important
            // make sure you call this in a transaction to ensure file copying before deleteing the original share folder if moving

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }
            var tagger = dir.Name;

            if (!destDirName.Contains(tagger))
            {
                destDirName = Path.Combine(destDirName, tagger);
            }
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            String[] allfiles = System.IO.Directory.GetFiles(sourceDirName, "*.*", System.IO.SearchOption.AllDirectories);

            foreach (var item in allfiles)
            {
                try
                {
                    var DestFile = destDirName + item.Substring(item.IndexOf(tagger) + tagger.Length);
                    FileInfo myMainFile = new FileInfo(item);
                    FileInfo mynewfile = new FileInfo(DestFile);
                    if (!Directory.Exists(mynewfile.Directory.ToString()))
                    {
                        Directory.CreateDirectory(mynewfile.Directory.ToString());
                    }

                    myMainFile.CopyTo(DestFile, true);

                }
                catch (Exception eee)
                {

                    continue;
                }

            }


            /*
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
            */
            //if (mover)
            //{
            //    Directory.Delete(sourceDirName);
            //}
        }

        private string GetDirectoryWithInfo()
        {
            try
            {
                var path = pretrail();
                var dir = new DirectoryInfo(@path);
                DirectoryInfo directoryInfo = dir;
                if (directoryInfo == null || directoryInfo.Exists == false) return JsonConvert.SerializeObject(new DirectoryInfo[0]);
                var tempbola = directoryInfo.GetDirectories();
                var bola = JsonConvert.SerializeObject(directoryInfo.GetDirectories());
                justapilog("Folder information requested " + @path);

                return bola;
            }
            catch
            {
                return JsonConvert.SerializeObject(new DirectoryInfo[0]);
            }
        }

        private byte[] getFile()
        {
            var path = pretrail();
            string xfilex = Path.Combine(path, Parameter1);

            byte[] fileBytes = System.IO.File.ReadAllBytes(@xfilex);
            string fileName = Parameter1;
            justapilog("Received file " + xfilex, 0, fileBytes.Length);

            return fileBytes; // File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        private bool ThumbnailCallback()
        {
            return false;
        }

        private string makethumb(FileInfo filename)
        {
            if (filename.Extension.ToLower() == ".jpg" || filename.Extension.ToLower() == ".png" || filename.Extension.ToLower() == ".gif" || filename.Extension.ToLower() == ".bmp" || filename.Extension.ToLower() == ".jpeg")
            {
                try
                {
                    Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    Bitmap myBitmap = new Bitmap(@filename.FullName);// "c:\rimkus\testfile.jpg");
                    Image myThumbnail = myBitmap.GetThumbnailImage(40, 40, myCallBack, IntPtr.Zero);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        myThumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);//.Bmp);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
                catch (Exception eee)
                {
                    var bbb = eee.Message;
                    return filename.Name;
                }
            }
            return filename.Name;
        }




        private string mobiledir()
        {
            try
            {
                // error if path is not found on allshare look
                var path = pretrail();


                /*
                if (!String.IsNullOrEmpty(searchpatternwa))
                {
                    if (System.IO.Directory.EnumerateDirectories(@path, searchpatternwa, SearchOption.AllDirectories).Count() > 0)
                    {
                        directories = System.IO.Directory.EnumerateDirectories(@path, searchpatternwa, SearchOption.AllDirectories);
                        searchpatternwa = "";
                    }
                }
                */

                if (!IsRootPath)
                {
                    if (AccessType != "Manager")
                    {
                        // get the folder name
                        //string foldername = RootPath.Substring(RootPath.LastIndexOf("\\") + 1);
                        //string folderbola = RimkusHelper.CanAccess(foldername);


                        string foldername = RootPath.Substring(RootPath.LastIndexOf("\\") + 1);
                        //string folderbola = RimkusHelper.CanAccess(foldername);
                        var folderbola = Userwa.ToUpper();
                        if (RootPath.Count(c => c == '\\') == 1)
                        {
                            folderbola = RimkusHelper.CanAccess(foldername);
                        }






                        if (!folderbola.ToUpper().Contains(Userwa.ToUpper()))
                        {
                            var serverblankfiles = new List<LocalFileStruc>();
                            serverblankfiles.Add(new LocalFileStruc()
                            {
                                filename = "Access Denied",
                                filesize = 0,
                                filetype = "DIR",
                                fileicon = "directory.png",
                                creationtime = DateTime.Now,
                                lastwritetime = DateTime.Now,
                                fullpath = "CHKSUM*DIR*ACCESSDENIED",
                                attributes = "-R-E-D"
                            });
                            return JsonConvert.SerializeObject(serverblankfiles);
                            //return String.Empty;
                        }
                    }
                }

                var myfolders = new List<string>();
                var serverfiles = new List<LocalFileStruc>() { };
                var directories = System.IO.Directory.EnumerateDirectories(@path);


                foreach (var directory in directories)
                {
                    var dirX = new DirectoryInfo(@directory);
                    serverfiles.Add(new LocalFileStruc()
                    {
                        Selected = false,
                        filename = Path.GetFileName(directory),
                        filesize = 0,
                        filetype = "DIR",
                        fileicon = "directory.png",
                        creationtime = dirX.CreationTimeUtc,
                        lastwritetime = dirX.LastWriteTimeUtc,
                        //fullpath = directory.Replace(FileSharePath, FileShare),
                        fullpath = "CHKSUM*RCGFILESHARE*" + directory.Replace(FileSharePath, FileShare + "*"),
                        attributes = "+R+E+D"
                    });
                }
                var dir = new DirectoryInfo(@path);
                FileInfo[] files = dir.GetFiles();

                if (!String.IsNullOrEmpty(searchpatternwa))
                    files = dir.GetFiles(searchpatternwa);


                foreach (System.IO.FileInfo fileinfo in files)
                {
                    serverfiles.Add(new LocalFileStruc()
                    {
                        Selected = false,
                        filename = fileinfo.Name,
                        filesize = fileinfo.Length,
                        filetype = fileinfo.Extension,
                        //fileicon = "file.png",
                        fileicon = makethumb(fileinfo),
                        winfiletype = GetFileType(fileinfo.FullName),
                        //fullpath = fileinfo.FullName.Replace(FileSharePath,FileShare),
                        fullpath = "CHKSUM*RCGFILESHARE*" + fileinfo.FullName.Replace(FileSharePath, FileShare + "*"),
                        creationtime = fileinfo.CreationTimeUtc,
                        lastwritetime = fileinfo.LastWriteTimeUtc,
                        attributes = "+R+E+D"
                    });
                }
                // now
                justapilog("Requested Folder Listing " + @path);

                return JsonConvert.SerializeObject(serverfiles);
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        private string toshare(string mainpath)
        {
            return String.Empty;
        }

        private string GetList(string wildcard = "*.*", bool filesOK = false)
        {
            var path = pretrail();
            var tryme = path.Split('/');
            DynatreeItem di = new DynatreeItem(new System.IO.DirectoryInfo(@path), tryme[tryme.Length - 2], filesOK, wildcard);
            string result = di.JsonToDynatree();
            return result;
        }

        private string Error(string msg)
        {
            return JsonConvert.SerializeObject(new
            {
                Error = msg,
                Code = -1
            });
        }

        private string justTrail()
        {
            var tempsharepath = "";
            tempsharepath = @RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim();
            //var dtempsharepath =Path.Combine(@RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim(),@RootPath);
            if (Directory.Exists(tempsharepath))
            {
                return tempsharepath;
            }
            return String.Empty;
        }

        private string pretrail()
        {
            RootPath = RootPath.Trim();
            IsRootPath = true;
            if (RootPath.Length > 1)
            {
                if (RootPath.Contains("\\")) RootPath = RootPath.Replace("\\\\", "\\");
                RootPath = @RootPath.Substring(0, 1) == @"/" ? @RootPath.Substring(1, @RootPath.Length - 1) : @RootPath;
            }



            // here here


            FileSharePath = @RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim();
            if (FileShare.Contains("PERSONALFOLDEROF"))
            {
                //FileSharePath = @RimkusHelper.DataParser("[AbpUsers.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim(); FileSharePath = "\\\\houserver\\users2$\\ZFaruqi";
                FileSharePath = "\\\\houserver\\users\\" + FileShare.Substring(FileShare.IndexOf("OF") + 2);
                if (!Directory.Exists(@FileSharePath))
                {
                    FileSharePath = "\\\\houserver\\users2$\\" + FileShare.Substring(FileShare.IndexOf("OF") + 2);
                }
            }
            else
            {
                FileSharePath = @RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim();
            }





            // FileSharePath = @RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim();
            var tempsharepath = "";
            try
            {
                if (RootPath.Length > 1)
                {
                    tempsharepath = Path.Combine(FileSharePath, @RootPath.Substring(0, 1) == @"\" ? @RootPath.Substring(1, @RootPath.Length - 1) : @RootPath);

                    //tempsharepath = Path.Combine(@RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim(), @RootPath.Substring(0, 1) == @"\" ? @RootPath.Substring(1, @RootPath.Length - 1) : @RootPath);
                    IsRootPath = false;
                }
                else
                    tempsharepath = FileSharePath;// @RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim();
            }
            catch (Exception eee)
            {
                var bola = eee.Message;
                IsRootPath = true;
                throw;
            }
            //var dtempsharepath =Path.Combine(@RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim(),@RootPath);
            if (Directory.Exists(tempsharepath))
            {
                return tempsharepath;
            }

            return String.Empty;

            switch (FileShare)
            {
                case "London Jobs":
                    return "c:/TestShareRoot/Share 2/London Open Jobs/" + @RootPath;
                    break;

                case "Orlando Jobs":
                    return "c:/TestShareRoot/Share 2/Orlando Open Jobs/" + @RootPath;
                    break;

                case "Houston Jobs":
                    return "c:/TestShareRoot/Share 1/Houston Open Jobs/" + @RootPath;
                    break;

                case "Indianapolis Jobs":
                    return "c:/TestShareRoot/Share 1/Indianapolis Open Jobs/" + @RootPath;
                    break;

                case "Test Jobs Large":
                    return "c:/TestShareRoot/Temp/" + @RootPath;
                    break;

                case "Test Jobs Medium":
                    return "c:/TestShareRoot/Temp2/" + @RootPath;
                    break;

                default:
                    return "c:/TestShareRoot/Share 2/Orlando Open Jobs/" + @RootPath;
                    break;
            }
            //  return "C:/TestShareRoot/Share 2/Orlando Open Jobs";
            //return "C:/TestShareRoot/Share 1/Houston Open Jobs/11010276";
            //rn Path.Combine(RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare).Trim() + @RootPath);
            //var tempsharepath = RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare).Trim() + @RootPath;
            //return tempsharepath;
        }

        private string getAllSharesFromServer()
        {
            var tempfolder = RimkusHelper.DataParser("RREPORTT[AbpFileShares.TenantId]{SharedFriendlyName}", "1");
            var templist = tempfolder.Replace("\r\n", "*").Split('*');
            var bola = JsonConvert.SerializeObject(templist);

            return bola;
        }

        private string getdir()
        {
            var rootDir = pretrail();

            try
            {
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(rootDir));
                for (int i = 0; i < dirs.Count; i++)
                {
                    dirs[i] = dirs[i].Replace(rootDir, "");
                }
                return JsonConvert.SerializeObject(dirs);
            }
            catch
            {
                return JsonConvert.SerializeObject(new List<string>());
            }
            //return rootDir;

            //            return Parameter1 + "=" + getJustRawFolder(rootDir);
            //if (Parameter1 == null) Parameter1 = "";// return String.Empty;
            //if (String.IsNullOrEmpty(Parameter1) )
            //var rootdirectory = RimkusHelper.GetTopRelativeFolderName(Parameter1);
            //string subpath = Parameter1.Replace(rootdirectory, "");
            //var truepath =RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", rootdirectory).Trim()+subpath;
            //return Parameter1+"="+ getJustRawFolder(truepath);
        }

        private string getdirfiles()
        {
            if (Parameter1 == null) return String.Empty;
            var rootdirectory = RimkusHelper.GetTopRelativeFolderName(Parameter1);
            string subpath = Parameter1.Replace(rootdirectory, "");
            var truepath = RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", rootdirectory).Trim() + subpath;
            return Parameter1 + "=" + GetFoldwithInfo();
            //return Parameter1 + "=" + getJustRawFolder(truepath,true);
        }

        private string getsubdir()
        {
            if (Parameter1 == null) return String.Empty;
            var rootdirectory = RimkusHelper.GetTopRelativeFolderName(Parameter1);
            string subpath = Parameter1.Replace(rootdirectory, "");
            var truepath = RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", rootdirectory).Trim() + subpath;
            return Parameter1 + "=" + getRawFolder(truepath);
        }

        private string getsubdirfiles()
        {
            if (Parameter1 == null) return String.Empty;
            var rootdirectory = RimkusHelper.GetTopRelativeFolderName(Parameter1);
            string subpath = Parameter1.Replace(rootdirectory, "");
            var truepath = RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", rootdirectory).Trim() + subpath;
            return Parameter1 + "=" + GetFoldwithInfo();
            //return Parameter1 + "=" + getRawFolder(truepath,true);
        }

        private string getJustRawFolder(string path, bool filesOK = false)
        {
            return "[" + GetJustRawDirectory(new DirectoryInfo(@path), filesOK).ToString() + "]";
        }

        private JToken GetJustRawDirectory(DirectoryInfo directory, Boolean filesOK = false)
        {
            if (filesOK)
            {
                return JToken.FromObject(new
                {
                    directory = directory.EnumerateDirectories()
                  .ToDictionary(x => x.Name, x => x.CreationTime),
                    file = directory.EnumerateFiles().Select(x => x.Name).ToList()
                });
            }
            else
            {
                return JToken.FromObject(new
                {
                    directory = directory.EnumerateDirectories()
                  .ToDictionary(x => x.Name, x => x.CreationTime)
                });
            }
        }

        //        IEnumerable<string> filenames
        //      = Directory.EnumerateFiles(targetdirectory, "backup-*.zip")
        //        .Select(p => File.GetFileName(p));

        private string getRawFolder(string path, bool filesOK = false)
        {
            return "[" + GetRawDirectory(new DirectoryInfo(@path), filesOK).ToString() + "]";
        }

        public string GetFileType(string fileName)
        {
            SHFILEINFO shFileInfo = new SHFILEINFO();

            SHGetFileInfo(fileName, 0, ref shFileInfo, (uint)Marshal.SizeOf(shFileInfo), SHGFI_TYPENAME);

            return shFileInfo.szTypeName;
        }

        private JToken GetRawDirectory(DirectoryInfo directory, Boolean filesOK = false)
        {
            if (filesOK)
            {
                return JToken.FromObject(new
                {
                    directory = directory.EnumerateDirectories()
                  .ToDictionary(x => x.Name, x => GetRawDirectory(x)),
                    file = directory.EnumerateFiles().Select(x => x.Name).ToList()
                });
            }
            else
            {
                return JToken.FromObject(new
                {
                    directory = directory.EnumerateDirectories()
                  .ToDictionary(x => x.Name, x => GetRawDirectory(x))
                });
            }
        }

        private string GetFoldwithInfo(bool filesOK = true) // false)
        {
            var path = pretrail();
            var tryme = path.Split('/');
            var justtrail = justTrail();
            DynatreeItem di = new DynatreeItem(new System.IO.DirectoryInfo(@path), justtrail, filesOK);
            string result = di.JsonToDynatree();
            return result;
        }

        private string GetMasterFoldwithInfo(bool filesOK = true) // false)
        {
            var path = RootPath;
            //var justtrail = justTrail();
            DynatreeItem di = new DynatreeItem(new System.IO.DirectoryInfo(@path), @path, filesOK);
            string result = di.JsonToDynatree();
            return result;
        }

        public void justapilog(string Msg, Int64 rx = 0, Int64 tx = 0)
        {
            LogData logdata = new LogData()
            {
                UserName = Userwa,
                PortNumber = "443",
                LogEntry = Msg,
                StartedAt = DateTime.UtcNow.ToString(),
                EndsAt = DateTime.UtcNow.ToString(),
                Company = RimkusHelper.APICompany,
                DataRx = rx, // Convert.ToInt64(DataTx),
                DataTx = tx// Convert.ToInt64(DataTx)
            };
            //System.IO.File.WriteAllText(@"c:\rimapi\testolog.txt", JsonConvert.SerializeObject(logdata));
            RimkusHelper.SendDataLogtoServer(logdata);

        }

        ///////////////////////////////////////////////////////////////////////////////
        private void xWriteLocalLog(string LogEntry)
        {
            //LogEntry = HttpUtility.UrlDecode(LogEntry, System.Text.Encoding.Default);
            //LogEntry = LogEntry.Replace("'", " ");
            LogEntry = DateTime.Now.ToString() + " " + LogEntry;
            var testUser = new RimSQL();

            testUser.ExecuteNonQuery("INSERT INTO PortTrack (UserName, PortNumber , TokenKey, RecdAt,LogEntry,ExpiresAt) VALUES ('" + Userwa + "','" + "[80]" + "','" + JsonConvert.SerializeObject(tokendata) + "','" + DateTime.UtcNow.ToString() + "','" + LogEntry + "','" + DateTime.UtcNow.ToString() + "')");

            LogData logdata = new LogData()
            {
                UserName = Userwa,
                PortNumber = "80",
                LogEntry = LogEntry,
                StartedAt = DateTime.UtcNow.ToString(),
                EndsAt = DateTime.UtcNow.ToString(),
                Company = RimkusHelper.APICompany,
                DataRx = 0,
                DataTx = 0
            };
            RimkusHelper.SendDataLogtoServer(logdata);
        }
    }
}