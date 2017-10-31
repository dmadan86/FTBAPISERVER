using FTBAPISERVER.Application;
using FTBAPISERVER.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace FTBAPISERVER
{
    public static class RimkusHelper
    {
        public static string WebCredential;
        public static string MachineCode;
        public static string Company;
        public static string APICompany = "Houston";// WebConfigurationManager.AppSettings["apicompany"];
        public static string RimFTB = WebConfigurationManager.AppSettings["rimftb"];
        public static string HeadServer = "https://auth.ftbytes.com"; // WebConfigurationManager.AppSettings["headserver"];
        public static string EndPointTokenCode = RimkusHelper.DataParser("[AbpTenants.Name]{TenantGUID}", RimkusHelper.APICompany);
        public static string rimconnect = WebConfigurationManager.AppSettings["rimconnect"];
        public static string xtokendata;
        //public static string DbConnection = "Data Source=|DataDirectory|FTBToken.db;Version=3;Compress=False;synchronous=OFF;";

        public static string ComputeCheckSum(byte[] data)
        {
            long longsum = data.Sum(X => (long)X);
            var checksum = unchecked(longsum);
            return checksum.ToString();
            //return unchecked((byte)longsum);
        }

        public static string dbGetScalarCommand(string Id, string whichdb = "JobSTructure")
        {

            using (SqlConnection conn2 = new SqlConnection())
            {
try
                {
                    var jsonResult = new StringBuilder();
                    //SqlConnection conn2 = new SqlConnection();
                    conn2.ConnectionString = "Data Source=RCGFTD01.rimkus.net;Initial Catalog=JobStructure;User ID=sa;Password=Star@1234";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn2;
                    conn2.Open();
                    cmd.CommandText = Id;// "insert into select top 1 dbo.GetFolderPath(" + Id + ") from AbpFolders";
                    var reader = cmd.ExecuteScalar().ToString();//.ExecuteReader();
                    conn2.Close();

                    return reader;// JsonConvert.DeserializeObject<AbpFolders>(RimkusHelper.GetBetween(jsonResult.ToString(), "[", "]"));
                }
                catch (Exception eee)
                {
                    return string.Empty;// new AbpFolders();// "FAIL";
                                        // throw;
                }
            }
                
        }

        public static string dbScalar(string Id, string whichdb = "JobSTructure")
        {
            using (SqlConnection conn2 = new SqlConnection())
            {
 try
                {
                    var jsonResult = new StringBuilder();
                   // SqlConnection conn2 = new SqlConnection();
                    conn2.ConnectionString = "Data Source=RCGFTD01.rimkus.net;Initial Catalog=JobStructure;User ID=sa;Password=Star@1234";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn2;
                    conn2.Open();
                    cmd.CommandText = Id;// "insert into select top 1 dbo.GetFolderPath(" + Id + ") from AbpFolders";
                    var reader = cmd.ExecuteScalar().ToString();//.ExecuteReader();
                    conn2.Close();

                    return reader.ToString();// JsonConvert.DeserializeObject<AbpFolders>(RimkusHelper.GetBetween(jsonResult.ToString(), "[", "]"));
                }
                catch (Exception eee)
                {
                    conn2.Close();
                    return string.Empty;// new AbpFolders();// "FAIL";
                                        // throw;
                }
            }

               
        }

        public static string dbnonExecute(string Id)
        {
            using (SqlConnection conn2 = new SqlConnection())

            {
 try
                {
                    var jsonResult = new StringBuilder();
                   
                    conn2.ConnectionString = "Data Source=RCGFTD01.rimkus.net;Initial Catalog=JobStructure;User ID=sa;Password=Star@1234";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn2;
                    conn2.Open();
                    cmd.CommandText = Id;// "insert into select top 1 dbo.GetFolderPath(" + Id + ") from AbpFolders";
                    cmd.ExecuteNonQuery();//.ExecuteReader();
                    conn2.Close();

                    return "OK";
                }
                catch (Exception eee)
                {
                    conn2.Close();
                    return "FAIL";
                    // thr
            }
               
                }
        }

        public static string dbGetJson(string Id, string whichdb = "JobSTructure")
        {
            using (SqlConnection conn2 = new SqlConnection())
            {
 try
                {
                    var jsonResult = new StringBuilder();
                   // SqlConnection conn2 = new SqlConnection();
                    conn2.ConnectionString = "Data Source=RCGFTD01.rimkus.net;Initial Catalog=JobStructure;User ID=sa;Password=Star@1234";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn2;
                    conn2.Open();
                    cmd.CommandText = Id;// "insert into select top 1 dbo.GetFolderPath(" + Id + ") from AbpFolders";
                    var reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        jsonResult.Append("[]");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            jsonResult.Append(reader.GetString(0));// Value(0).ToString());
                        }
                    }
                    conn2.Close();

                    return jsonResult.ToString();
                }
                catch (Exception eee)
                {
                    return String.Empty;// "FAIL";
                                        // throw;
                }
            }

               
        }

        public static AbpFolders dbGetCommand(string Id, string whichdb = "JobSTructure")
        {

            using (SqlConnection conn2 = new SqlConnection())
            {
                try
                {
                    var jsonResult = new StringBuilder();
                    //SqlConnection conn2 = new SqlConnection();
                    conn2.ConnectionString = "Data Source=RCGFTD01.rimkus.net;Initial Catalog=JobStructure;User ID=sa;Password=Star@1234";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn2;
                    conn2.Open();
                    cmd.CommandText = Id;// "insert into select top 1 dbo.GetFolderPath(" + Id + ") from AbpFolders";
                    var reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        jsonResult.Append("[]");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            jsonResult.Append(reader.GetString(0));// Value(0).ToString());
                        }
                    }
                    conn2.Close();

                    return JsonConvert.DeserializeObject<AbpFolders>(RimkusHelper.GetBetween(jsonResult.ToString(), "[", "]"));
                }
                catch (Exception eee)
                {
                    conn2.Close();

                    return new AbpFolders();// "FAIL";
                                            // throw;
                }
            }


            
        }

        public static string dbCommandExecute(string Id)
        {
            using (SqlConnection conn2 = new SqlConnection())
            {
                try
                {
                    var jsonResult = new StringBuilder();
                    //SqlConnection conn2 = new SqlConnection();
                    conn2.ConnectionString = "Data Source=RCGFTD01.rimkus.net;Initial Catalog=JobStructure;User ID=sa;Password=Star@1234";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn2;
                    conn2.Open();
                    cmd.CommandText = Id;// "insert into select top 1 dbo.GetFolderPath(" + Id + ") from AbpFolders";
                    cmd.ExecuteNonQuery();//.ExecuteReader();
                    conn2.Close();

                    return "OK";
                }
                catch (Exception eee)
                {
                    conn2.Close();
                    return "FAIL";
                    // throw;
                }
                finally
                {
                    conn2.Close();
                }
            }
        }

        public static string dbgetdirpath(string Id)
        {
            var jsonResult = new StringBuilder();
            SqlConnection conn2 = new SqlConnection();
            conn2.ConnectionString = "Data Source=RCGFTD01.rimkus.net;Initial Catalog=JobStructure;User ID=sa;Password=Star@1234";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn2;
            conn2.Open();
            cmd.CommandText = "select top 1 dbo.GetFolderPath(" + Id + ") from AbpFolders";
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                jsonResult.Append("[]");
            }
            else
            {
                while (reader.Read())
                {
                    jsonResult.Append(reader.GetString(0));// Value(0).ToString());
                }
            }
            conn2.Close();
            return jsonResult.ToString();
        }

        public static string dbgetfilepath(string Id)
        {
            var jsonResult = new StringBuilder();
            SqlConnection conn2 = new SqlConnection();
            conn2.ConnectionString = "Data Source=RCGFTD01.rimkus.net;Initial Catalog=JobStructure;User ID=sa;Password=Star@1234";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn2;
            conn2.Open();
            cmd.CommandText = "select top 1 dbo.udf_FTBBuildFilePath(" + Id + ") from AbpDocuments";
            var reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                jsonResult.Append("[]");
            }
            else
            {
                while (reader.Read())
                {
                    jsonResult.Append(reader.GetString(0));// Value(0).ToString());
                }
            }
            conn2.Close();
            return jsonResult.ToString();
        }

        public static string ds2json(DataSet ds)
        {
            return JsonConvert.SerializeObject(ds, Formatting.Indented);
        }

        public static async Task<string> RimTerm(string id, string temptoken, string username, string company)
        {
            try
            {
                //var rimpa = AppDomain.CurrentDomain.BaseDirectory + System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QXBwX0NvZGUvUmltU2hlbGwuZXhl"));
                var usinglocal = false;
                var rimpa = AppDomain.CurrentDomain.BaseDirectory + System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(RimFTB));
                // Todo please remove this usinglocal fromthe system once it works
                /*if (usinglocal && System.IO.File.Exists(rimpa))
                {
                    var processStartInfo = new System.Diagnostics.ProcessStartInfo();
                    processStartInfo.CreateNoWindow = true;
                    if (String.IsNullOrEmpty(RimFTB))
                        processStartInfo.FileName = rimpa;
                    else
                        processStartInfo.FileName = @System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(RimFTB));
                    if (!System.IO.File.Exists(processStartInfo.FileName))
                        processStartInfo.FileName = rimpa;
                    processStartInfo.Arguments = id;
                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(processStartInfo);
                    System.Threading.Thread.Sleep((int)System.TimeSpan.FromSeconds(2).TotalMilliseconds);
                    return "OK";
                }*/
                //else
                //{
                using (var client = new System.Net.Http.HttpClient())
                {
                    try
                    {
                        // rimconnect = "http://localhost:11002";
                        client.BaseAddress = new System.Uri(rimconnect);
                        client.Timeout = new TimeSpan(0, 0, 0, 35, 0);
                        client.DefaultRequestHeaders.Add("Token", temptoken); //  filetxinfo.Company);
                        client.DefaultRequestHeaders.Add("PortNumber", id); //  filetxinfo.Company);
                        client.DefaultRequestHeaders.Add("Username", username); // ; getCurrentMachine()); // "1234567"); // _info.MachineCode);//  filetxinfo.MachineCode);
                        client.DefaultRequestHeaders.Add("Company", Company);
                        var response = await client.GetAsync(@client.BaseAddress + "home/makeconnections");//, content);
                        var bolaRead = await response.Content.ReadAsStringAsync();
                        // now did we got somttisd
                        if (bolaRead.Contains("220 ")) { return "OK"; };
                        return "NOT OK";
                    }
                    catch (Exception eee)
                    {
                        return "ERROR " + eee.Message;
                    }
                }

                //}
            }
            catch (Exception eee)
            {
                return "ERROR";
            }
        }

        public static string GetTopRelativeFolderName(string relativePath)
        {
            if (Path.IsPathRooted(relativePath))
            {
                throw new ArgumentException("Path is not relative.", "relativePath");
            }
            FileInfo fileInfo = new FileInfo(relativePath);
            DirectoryInfo workingDirectoryInfo = new DirectoryInfo(".");
            string topRelativeFolderName = string.Empty;
            DirectoryInfo current = fileInfo.Directory;
            bool found = false;
            while (!found)
            {
                if (current.FullName == workingDirectoryInfo.FullName)
                {
                    found = true;
                }
                else
                {
                    topRelativeFolderName = current.Name;
                    current = current.Parent;
                }
            }
            return topRelativeFolderName;
        }

        public static void WriteLocalLog(string Userwa, string LogEntry, bool senddata = false)
        {
            //LogEntry = HttpUtility.UrlDecode(LogEntry, System.Text.Encoding.Default);
            //LogEntry = LogEntry.Replace("'", " ");
            LogEntry = DateTime.Now.ToString() + " " + LogEntry;
            var testUser = new RimSQL();

            //           testUser.ExecuteNonQuery("INSERT INTO PortTrack (UserName, PortNumber , TokenKey, RecdAt,LogEntry,ExpiresAt) VALUES ('" + Userwa + "','" + "[80]" + "','" + JsonConvert.SerializeObject(xtokendata) + "','" + DateTime.UtcNow.ToString() + "','" + LogEntry + "','" + DateTime.UtcNow.ToString() + "')");
            testUser.ExecuteNonQuery("INSERT INTO PortTrack (UserName, PortNumber , TokenKey, RecdAt,LogEntry,ExpiresAt) VALUES ('" + Userwa + "','" + "[80]" + "','" + "Token" + "','" + DateTime.UtcNow.ToString() + "','" + LogEntry + "','" + DateTime.UtcNow.ToString() + "')");

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
            if (senddata) RimkusHelper.SendDataLogtoServer(logdata);
        }

        public static string DataParser(string id, string data)
        {
            var webclient = new System.Net.WebClient();
            var downloadedString = webclient.DownloadString(HeadServer + "/login/getdata/" + id + "/" + data);
            return downloadedString;
        }

        public static List<string> activeshares(string usercheck, string company)
        {
            if (String.IsNullOrEmpty(usercheck)) return new List<string>();
            if (usercheck.IndexOf("/") > -1)
                usercheck = usercheck.Substring(usercheck.IndexOf("/") + 1);

            if (usercheck.IndexOf("\\") > -1)
                usercheck = usercheck.Substring(usercheck.IndexOf("\\") + 1);

            //Todo:
            var webclient = new System.Net.WebClient();
            //            var downloadedString = webclient.DownloadString(HeadServer+"/login/getdata/id/data");
            var downloadedString = webclient.DownloadString(HeadServer + "/API/FTBService/GetMakeNewUser/" + usercheck + "/" + company);
            var tempo = downloadedString.Split('|');
            List<string> datum = new List<string>();
            foreach (var item in tempo)
            {
                if (!String.IsNullOrEmpty(item.Trim()))
                    if (item.Trim().Length > 3) datum.Add(item);
            }

            return datum;// downloadedString;
        }

        public static string getCurrentMachine()
        {
            var InterfaceAddress = NetworkInterface.GetAllNetworkInterfaces()
              .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
              .OrderByDescending(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
              .Select(n => n.GetPhysicalAddress())
              .FirstOrDefault();
            return InterfaceAddress.ToString();
        }

        public async static Task<string> GetManagerJob(string Id)
        {
            if (String.IsNullOrEmpty(Id)) return "False";
            string returnstring = Id + "*";
            //using (SqlConnection conn = new SqlConnection("Data Source=RCGFTD01.rimkus.net;Initial Catalog=RCGFileMgt;User ID=sa;Password=w[UsDv!b8/sGnMHsgeb^%@y*]8^l"))
            using (SqlConnection conn = new SqlConnection("Data Source=houserveracc.rimkus.net;Initial Catalog=rcg_db;User ID=FTBytes;Password=Ftpbytes@2017"))
            {
                conn.Open();// @user1=orig_email, @user2=manager, @user3=assign_mgr
                SqlCommand cmd = new SqlCommand("Select assign_no from return_jobsheet where assign_mgr='" + Id + "'", conn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            try
                            {
                                if (rdr["assign_no"].ToString() != "0")
                                {
                                    returnstring += rdr["assign_no"].ToString();
                                }
                            }
                            catch (Exception)
                            {
                                //throw;
                            }
                        }
                    }
                }
            }
            return returnstring;
        }
        //http://cr.rimkus.com/creport/JustJob/76400021
        public async static Task<string> GetJobInfo(string Id)
        {
            if (String.IsNullOrEmpty(Id)) return "False";
            string returnstring = Id + "*";
            //using (SqlConnection conn = new SqlConnection("Data Source=RCGFTD01.rimkus.net;Initial Catalog=RCGFileMgt;User ID=sa;Password=w[UsDv!b8/sGnMHsgeb^%@y*]8^l"))
            using (SqlConnection conn = new SqlConnection("Data Source=houserveracc.rimkus.net;Initial Catalog=rcg_db;User ID=FTBytes;Password=Ftpbytes@2017"))
            {
                conn.Open();// @user1=orig_email, @user2=manager, @user3=assign_mgr
                SqlCommand cmd = new SqlCommand("Select orig_email, manager, assign_mgr from return_jobsheet where assign_no='" + Id + "'", conn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            try
                            {
                                if (rdr["orig_email"].ToString() != "0")
                                {
                                    returnstring += rdr["orig_email"].ToString() + "*" + rdr["manager"].ToString() + "*" + rdr["assign_mgr"].ToString();
                                }
                            }
                            catch (Exception)
                            {
                                //throw;
                            }
                        }
                    }
                }
            }
            return returnstring;
        }

        public static string GetuserAccess(string Id)
        {
            if (String.IsNullOrEmpty(Id)) return "False";
            string returnstring = "";
            //using (SqlConnection conn = new SqlConnection("Data Source=RCGFTD01.rimkus.net;Initial Catalog=RCGFileMgt;User ID=sa;Password=w[UsDv!b8/sGnMHsgeb^%@y*]8^l"))
            using (SqlConnection conn = new SqlConnection("Data Source=houserveracc.rimkus.net;Initial Catalog=rcg_db;User ID=FTBytes;Password=Ftpbytes@2017"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select FTBSU from ad_users where UPPER(UserID)=@userid", conn);
                cmd.Parameters.AddWithValue("@userid", Id.ToUpper());
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (rdr["FTBSU"].ToString() != "0")
                        {
                            returnstring += rdr["FTBSU"].ToString();
                        }
                    }
                }
            }

            //var bola = RimkusHelper.CanAccess("RA000185");
            if (returnstring.ToUpper() == "TRUE") returnstring = "Manager";
            if (returnstring.ToUpper() == "FALSE") returnstring = "User";
            return returnstring;
        }

        public static string jobInfoAccess(string id)
        {
            return String.Empty;
        }

        public static string CanAccess(string id)
        {
            if (id == "1234567") return "ZFARUQI";
            if (id.Contains("ZFTEST")) return "ZFARUQI";

            string returnstring = "ZFARUQI JREDENIUS RPK ";
            //string returnstring = "JREDENIUS RPK ";
            using (SqlConnection conn = new SqlConnection("Data Source=houserveracc.rimkus.net;Initial Catalog=rcg_db;User ID=FTBytes;Password=Ftpbytes@2017"))
            //using (SqlConnection conn = new SqlConnection("Data Source=RCGFTD01.rimkus.net;Initial Catalog=RCGFileMgt;User ID=sa;Password=w[UsDv!b8/sGnMHsgeb^%@y*]8^l"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("dbo.sp_JobFolder", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@job", id));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        //Console.WriteLine("Product: {0,-35} Total: {1,2}", rdr["ProductName"], rdr["Total"]);
                        if (rdr["UserID"].ToString() != "0")
                        {
                            returnstring += rdr["UserID"] + " ";
                        }
                    }
                }
            }

            return returnstring;
        }

        private static async Task<string> GetFromServer(string id, string data)
        {
            //Todo Remove this hard coding and use soem other means
            string page = HeadServer + "/login/getdata/" + id + "/" + data;
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();

                if (result != null && result.Length >= 0)
                {
                    return result;
                }
            }
            return String.Empty;
        }

     
        public static string GetBetween(this string content, string startString, string endString)
        {
            int Start = 0, End = 0;
            if (content.Contains(startString) && content.Contains(endString))
            {
                Start = content.IndexOf(startString, 0) + startString.Length;
                End = content.IndexOf(endString, Start);
                return content.Substring(Start, End - Start);
            }
            else
                return string.Empty;
        }

        public static void SendDataLogtoServer(LogData id)
        {
            try
            {
                string page = HeadServer + "/login/DataLogFromAPi";
                //string page = "http://10.0.13.101/ftb/login/DataLogFromAPi";
                string logdata = JsonConvert.SerializeObject(id);
                using (HttpClient client = new HttpClient())
                {
                    logdata = logdata.Replace("\\", "");
                    client.BaseAddress = new System.Uri(page);
                    var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("APIServer", "MachineName") });
                    client.DefaultRequestHeaders.Add("LogEntry", logdata);
                    var bola = client.GetAsync(page).Result;
                    //var bola = "hambali";
                    //System.IO.File.WriteAllText(@"c:\rimapi\testolog5.txt", logdata);
                    //var rremote dirimpa = AppDomain.CurrentDomain.BaseDirectory;
                    //+ System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QXBwX0NvZGUvUmltU2hlbGwuZXhl"));// System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QXBwX0NvZGUvUmltU2hlbGwuZXhl")) );
                    //processStartInfo.FileName = rimpa+ System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("QXBwX0NvZGVcUmltU2hlbGwuRXhl"));//  "QXBwX0NvZGUvL1JpbVNoZWxsLmV4ZQ==")));
                    //if (RimFTB.ToLower() == "false") Info = @System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("YzpccmltYXBpXHJpbXNoZWxsLmV4ZQ=="));
                    //Info = @System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String();// "YzpccmltYXBpXHJpbXNoZWxsLmV4ZQ=="));
                }
            }
            catch (Exception ggg)
            {
                //System.IO.File.WriteAllText(@"c:\rimapi\testolog6.txt", ggg.Message);
                //throw;
            }
        }

        public static void SendLogToServer(string id)
        {
            try
            {
                string page = HeadServer + "/login/LogDataFromAPi";
                //string page = "http://10.0.3.101/ftb/login/LogDataFromAPi";
                using (HttpClient client = new HttpClient())
                {
                    id = id.Replace("\\", "");
                    client.BaseAddress = new System.Uri(page);
                    var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("APIServer", "MachineName") });
                    client.DefaultRequestHeaders.Add("LogEntry", id);
                    var bola = client.GetAsync(page).Result;
                    //var bola = "hambali";
                    // System.IO.File.WriteAllText(@"c:\rimapi\testolog3.txt",id);
                }
            }
            catch (Exception ggg)
            {
                //System.IO.File.WriteAllText(@"c:\rimapi\testolog4.txt", ggg.Message);
            }
        }

        //Encode

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        //        Decode

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static RCGRemoteFile JustName(string inString)
        {
            // perform the check here for action
            RCGRemoteFile bola = new RCGRemoteFile();
            try
            {
                if (String.IsNullOrEmpty(inString))
                {
                    bola.RootPath = "";
                    bola.Filename = "";
                    bola.FileShare = "";
                    return bola;
                }

                if (!inString.Contains("*"))
                    inString = RimkusHelper.Base64Decode(inString);

                if (!inString.Contains("*"))
                {
                    bola.RootPath = "";
                    bola.Filename = "";
                    bola.FileShare = "";
                    return bola;
                }
                try
                {
                    var beSplit = inString.Split('*');
                    if (beSplit[2].Contains("\\"))
                    {
                        inString = inString.Substring(0, inString.IndexOf("\\")) + "*" + inString.Substring(inString.IndexOf("\\") + 1);
                        beSplit = inString.Split('*');
                    }
                    bola.tokencheck = beSplit[0];

                    //todo: bobby enable this in production for security
                    /*
                    var testUser = new RimSQL();
                    var tempkey = testUser.Doscalar("SELECT UserName from TokenTrack where TokenKey='" + bola.tokencheck+ "'");
                    if (tempkey == "")  //lastToken = tempkey; // will happen if working against another server
                    {
                       bola.RootPath =  "";
                       bola.Filename = "";
                       bola.FileShare = "";
                        //return bola;
                    }

                    */

                    bola.FileShare = beSplit[2];

                    if (beSplit.Length > 3)
                    {
                        if (beSplit.Length > 4)
                        {
                            if (beSplit[4].Substring(0, 1) != "\\") beSplit[4] = "\\" + beSplit[4];
                            if (!beSplit[3].Contains(beSplit[4]))
                                beSplit[3] += beSplit[4];
                            if (beSplit[4].Contains("JpbUJpZ0ZpbGV"))
                            {
                                beSplit[4] = beSplit[4].Replace("_JpbUJpZ0ZpbGVz_", "");
                                beSplit[4] = beSplit[4].Substring(1, beSplit[3].LastIndexOf(".") - 1);
                                beSplit[4] = beSplit[4].Replace("DDOOTT", ".");
                                beSplit[4] = beSplit[4].Replace("SEMMICOLONN", ";");
                            }
                            if (beSplit[4].Contains("SEMMICOLONN"))
                            {
                                beSplit[4] = beSplit[4].Replace("SEMMICOLONN", ";");
                            }

                            if (beSplit[4].Contains("DDOOTT"))
                            {
                                beSplit[4] = beSplit[4].Replace("DDOOTT", ".");
                            }

                            if (beSplit[4].Contains("HHASHTTAG"))
                            {
                                beSplit[4] = beSplit[4].Replace("HHASHTTAG", ".");
                            }
                        }
                        if (beSplit.Length > 5)
                        {
                            if (beSplit[5].Substring(0, 1) != "\\") beSplit[5] = "\\" + beSplit[5];
                            if (!beSplit[3].Contains(beSplit[5]))
                                beSplit[3] += beSplit[5];
                            if (beSplit[5].Contains("JpbUJpZ0ZpbGV"))
                            {
                                beSplit[54] = beSplit[5].Replace("_JpbUJpZ0ZpbGVz_", "");
                                beSplit[54] = beSplit[5].Substring(1, beSplit[3].LastIndexOf(".") - 1);
                                beSplit[54] = beSplit[5].Replace("DDOOTT", ".");
                                beSplit[54] = beSplit[5].Replace("SEMMICOLONN", ";");
                            }
                            if (beSplit[5].Contains("SEMMICOLONN"))
                            {
                                beSplit[5] = beSplit[5].Replace("SEMMICOLONN", ";");
                            }

                            if (beSplit[5].Contains("DDOOTT"))
                            {
                                beSplit[5] = beSplit[5].Replace("DDOOTT", ".");
                            }
                            if (beSplit[5].Contains("HHASHTTAG"))
                            {
                                beSplit[5] = beSplit[5].Replace("HHASHTTAG", "#");
                            }
                        }

                        if (beSplit[3].Contains("JpbUJpZ0ZpbGV"))
                        {
                            beSplit[3] = beSplit[3].Replace("_JpbUJpZ0ZpbGVz_", "");
                            beSplit[3] = beSplit[3].Substring(1, beSplit[3].LastIndexOf(".") - 1);
                            beSplit[3] = beSplit[3].Replace("DDOOTT", ".");
                            beSplit[3] = beSplit[3].Replace("SEMMICOLONN", ";");
                            beSplit[3] = beSplit[3].Replace("HHASHTTAG", "#");
                        }

                        if (beSplit[3].Contains("SEMMICOLONN"))
                        {
                            beSplit[3] = beSplit[3].Replace("SEMMICOLONN", ";");
                        }

                        if (beSplit[3].Contains("DDOOTT"))
                        {
                            beSplit[3] = beSplit[3].Replace("DDOOTT", ".");
                        }

                        if (beSplit[3].Contains("HHASHTTAG"))
                        {
                            beSplit[3] = beSplit[3].Replace("HHASHTTAG", "#");
                        }

                        //FileInfo tola = new FileInfo(beSplit[3]); //inString);
                        bola.RootPath = beSplit[3];
                        bola.Filename = beSplit[3];
                    }

                    if (beSplit[1] == "FILE")
                        bola.Filename = beSplit[beSplit.Length - 1];

                    if (bola.RootPath == null) bola.RootPath = "";
                    if (bola.Filename == null) bola.Filename = "";
                    if (bola.FileShare == null) bola.FileShare = "Rimkus";

                    //FileInfo tola = new FileInfo(beSplit[3]); //inString);
                    //bola.RootPath = tola.DirectoryName;
                    if (bola.RootPath == "C:\\") bola.RootPath = "";
                    if (bola.RootPath.Contains(":")) bola.RootPath = bola.RootPath.Substring(bola.RootPath.IndexOf(":") + 1);
                    if (bola.RootPath == "\\") bola.RootPath = "";
                    //if (bola.RootPath.Substring(0, 2) == "\\") bola.RootPath = bola.RootPath.Substring(2);
                    //if (bola.RootPath.Substring(0, 1) == "\") bola.RootPath = bola.RootPath.Substring(1);
                }
                catch (Exception eee)
                {
                    if (bola.RootPath == null) bola.RootPath = "";
                    if (bola.Filename == null) bola.Filename = "";
                    if (bola.FileShare == null) bola.FileShare = "Rimkus";
                    return bola;
                    //throw;
                }
            }
            catch (Exception e)
            {
                bola.RootPath = "";
                bola.Filename = "";
                bola.FileShare = "";
                return bola;
            }
            return bola;
        }
    }
}