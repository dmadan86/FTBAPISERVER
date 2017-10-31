using FTBAPISERVER.Models;
using System;
using System.IO;
using System.Text;

namespace FTBAPISERVER.Application
{
    public static class FTBResponse
    {
        public static void WriteLog()
        {

        }
        public static byte[] AFFR(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }
            var DataGram = Encoding.ASCII.GetBytes("220 action will perform"); return DataGram;
        }

        public static byte[] NEGT(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("220 action will not be completed"); return DataGram;
        }

        public static byte[] USER(FileTxInfo userdata, string Para)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("220 zfaruqi"); return DataGram;
        }

        public static byte[] PASS(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("551 Request action aborted"); return DataGram;
        }

        public static byte[] ACCT(FileTxInfo userdata, string Para)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("550 Request action not taken"); return DataGram;
        }

        public static byte[] CWDY(string Para, ref FileTxInfo userdata)
        {
            try
            {
                if (String.IsNullOrEmpty(Para.Trim()))
                {
                    return Encoding.ASCII.GetBytes("220 Your Working folder is " + userdata.RootPath);
                }

                // first check if OK
                var realroot = pretrail(userdata.FileShare);
                //var path = realroot + "/" + userdata.RootPath + "/" + Para;
                var path = Path.Combine(realroot, userdata.RootPath, Para);
                var result = "450 Directory Does Not Exists cannot change working path";
                if (Directory.Exists(@path))
                {
                    userdata.RootPath = Path.Combine(userdata.RootPath, Para);
                    result = "220 Your Working folder is " + userdata.RootPath;
                }
                var DataGram = Encoding.ASCII.GetBytes(result); return DataGram;
            }
            catch
            {
                return Encoding.ASCII.GetBytes("450 Directory Does Not Exists cannot change working path");
            }
        }

        public static byte[] CDUP(string Para, ref FileTxInfo userdata)
        {
            try
            {
                if (String.IsNullOrEmpty(Para.Trim()))
                {
                    return Encoding.ASCII.GetBytes("220 Your Working folder is " + userdata.RootPath);
                }

                // first check if OK
                var realroot = pretrail(userdata.FileShare);
                //var path = realroot + "/" + userdata.RootPath + "/" + Para;
                var path =realroot ;
                var result = "450 Directory Does Not Exists cannot change working path";
                if (Directory.Exists(@path))
                {
                    userdata.RootPath ="/";
                    result = "220 Your Working folder is " + userdata.RootPath;
                }
                var DataGram = Encoding.ASCII.GetBytes(result); return DataGram;
            }
            catch
            {
                return Encoding.ASCII.GetBytes("450 Directory Does Not Exists cannot change working path");
            }

            //var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] SMNT(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("SMNT"); return DataGram;
        }

        public static byte[] QUIT(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("220 Disconnected.Thank you for using our services."); return DataGram;
        }

        public static byte[] REIN(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] PORT(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] PASV(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] RVOK(FileTxInfo userdata)
        {
            try
            {
                var testUser = new RimSQL();
                testUser.ExecuteNonQuery("DELETE FROM TokenTrack where UserName='" + userdata.User.ToLower() + "'");
            }
            catch
            {
            }
            var DataGram = Encoding.ASCII.GetBytes("220 Token revoked by user."); return DataGram;
        }

        public static byte[] TYPE(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] STRU(FileTxInfo userdata, string Para)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] MODE(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] RETR(FileTxInfo userdata, string Para)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] STOR(FileTxInfo userdata, string Para, ref string filepathname)
        {
            try
            {
                if (String.IsNullOrEmpty(Para.Trim()))
                {
                    return Encoding.ASCII.GetBytes("450 You do not have a valid file.");
                }

                // first check if OK
                var realroot = pretrail(userdata.FileShare);
                //var filepath = realroot + "/" + (userdata.RootPath + "/").Replace("//","/") + Para;
                var filepath = Path.Combine(realroot, userdata.RootPath, Para);

                var result = "125 Data Connection already open, transfer starting";
                if (File.Exists(@filepath))
                {
                    FileInfo f = new FileInfo(@filepath);
                    long s1 = f.Length;
                    var filesize = s1.ToString();
                    result = "450 Requested file action not taken. File already exists of size " + filesize;
                    return Encoding.ASCII.GetBytes(result);
                }

                using (System.IO.FileStream fs = System.IO.File.Create(@filepath))
                {
                    fs.Close();
                    filepathname = @filepath;
                }
                var DataGram = Encoding.ASCII.GetBytes(result); return DataGram;
            }
            catch
            {
                return Encoding.ASCII.GetBytes("450 You do not have a valid file.");
            }

            //byte[] DataGram;
            //var pathString = @"C:\test\img100.jpg";
            //if (!System.IO.File.Exists(pathString))
            //{
            //    using (System.IO.FileStream fs = System.IO.File.Create(pathString))
            //    {
            //        fs.Close();
            //        DataGram = Encoding.ASCII.GetBytes(@"125 Data Connection already open , transfer starting");
            //        return DataGram;
            //    }
            //}
            //else
            //{
            //    DataGram = Encoding.ASCII.GetBytes(@"450 Requested file action not taken. File already exists.");
            //    return DataGram;
            //}
            //DataGram = Encoding.ASCII.GetBytes(@"210 Saving file img100.jpg to location \\shres\filepath\. end file with {CR}.{CR}");
            //return DataGram;
        }

        public static byte[] SYNC(FileTxInfo userdata, string Para, ref string filepathname)
        {
            try
            {
                if (String.IsNullOrEmpty(Para.Trim()))
                {
                    return Encoding.ASCII.GetBytes("450 You do not have a valid file.");
                }

                // get the filename
                string xfilename = Path.GetFileName(Para);
                string pather = Para.Replace(xfilename, "");

                // first check if OK
                var realroot = pretrail(userdata.FileShare);
                string testpath = Path.Combine(realroot, userdata.RootPath, pather);

                if (!Directory.Exists(testpath))
                {
                    Directory.CreateDirectory(testpath);
                }

                //var filepath = realroot + "/" + (userdata.RootPath + "/").Replace("//","/") + Para;
                var filepath = Path.Combine(realroot, userdata.RootPath, Para);

                var result = "125 Data Connection already open, transfer starting";
                if (File.Exists(@filepath))
                {
                    FileInfo f = new FileInfo(@filepath);
                    long s1 = f.Length;
                    var filesize = s1.ToString();
                    result = "450 Requested file action not taken. File already exists of size " + filesize;
                    return Encoding.ASCII.GetBytes(result);
                }

                using (System.IO.FileStream fs = System.IO.File.Create(@filepath))
                {
                    fs.Close();
                    filepathname = @filepath;
                }
                var DataGram = Encoding.ASCII.GetBytes(result); return DataGram;
            }
            catch
            {
                return Encoding.ASCII.GetBytes("450 You do not have a valid file.");
            }
        }

        public static byte[] STOU(FileTxInfo userdata, string Para, ref string filepathname3)
        {
            try
            {
                if (String.IsNullOrEmpty(Para.Trim()))
                {
                    return Encoding.ASCII.GetBytes("450 You do not have a valid file.");
                }

                // first check if OK
                var realroot = pretrail(userdata.FileShare);
                //var filepath = realroot + "/" + userdata.RootPath + "/" + Para;
                var filepath = Path.Combine(realroot, userdata.RootPath, Para);

                var result = "125 Data Connection already open, transfer starting";
                if (File.Exists(@filepath))
                {
                    string neeguid = Guid.NewGuid().ToString();
                    //filepath = realroot + "/" + (userdata.RootPath + "/").Replace("//", "/") + neeguid + Para;
                    filepath = Path.Combine(realroot, userdata.RootPath, neeguid + Para);
                }

                using (System.IO.FileStream fs = System.IO.File.Create(@filepath))
                {
                    fs.Close();
                    filepathname3 = @filepath;
                }
                var DataGram = Encoding.ASCII.GetBytes(result); return DataGram;
            }
            catch
            {
                return Encoding.ASCII.GetBytes("450 You do not have a valid file.");
            }
        }

        public static byte[] APPE(FileTxInfo userdata, string Para, ref string filepathname2)
        {
            try
            {
                if (String.IsNullOrEmpty(Para.Trim()))
                {
                    return Encoding.ASCII.GetBytes("450 You do not have a valid file.");
                }

                // first check if OK
                var realroot = pretrail(userdata.FileShare);
                //var filepath = realroot + "/" + (userdata.RootPath + "/").Replace("//", "/") + Para;
                var filepath = Path.Combine(realroot, userdata.RootPath, Para);
                var result = "125 Data Connection already open, transfer starting";
                if (!File.Exists(@filepath))
                {
                    //userdata.RootPath = userdata.RootPath + "/" + Para;
                    result = "450 Requested file action not taken. File does not exists.";
                }
                filepathname2 = @filepath;
                var DataGram = Encoding.ASCII.GetBytes(result); return DataGram;
            }
            catch
            {
                return Encoding.ASCII.GetBytes("450 You do not have a valid file.");
            }
        }

        public static byte[] ALLO(FileTxInfo userdata, string Para)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] REST(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] RNFR(FileTxInfo userdata, string Para)
        {
            try
            {
                // first checkif file exist
                var realroot = pretrail(userdata.FileShare);
                //var path = realroot + "/" + (userdata.RootPath + "/").Replace("//", "/") + Para;
                var path = Path.Combine(realroot, userdata.RootPath, Para);

                var tryme = path.Split('/');
                var result = "450 Directory Does Not Exists Cannot Rename";
                if (Directory.Exists(@path))
                {
                    result = "350 Send RNTO command with new folder name";
                }
                var DataGram = Encoding.ASCII.GetBytes(result); return DataGram;
            }
            catch
            {
                return Encoding.ASCII.GetBytes("450 Directory Does Not Exists Cannot Rename");
            }
        }

        public static byte[] RNTO(FileTxInfo userdata, string Para)
        {
            try
            {
                if (!Para.Contains(",")) return Encoding.ASCII.GetBytes("450 Rename from rename to parameters not correct");
                var filesplit = Para.Split(',');
                var realroot = pretrail(userdata.FileShare);
                //var path = realroot + "/" + (userdata.RootPath + "/").Replace("//", "/") + filesplit[1];
                var path = Path.Combine(realroot, userdata.RootPath, filesplit[1]);

                var result = "c Directory renamed successfully";
                if (Directory.Exists(@path))
                {
                    result = "450 Destination Directory Exists. Please select another name";
                }

                try
                {
                    Directory.Move(Path.Combine(realroot, userdata.RootPath, filesplit[0]), Path.Combine(realroot, userdata.RootPath, filesplit[1]));
                }
                catch
                {
                    result = "450 Directory could not be renamed. Errors";
                }
                var DataGram = Encoding.ASCII.GetBytes(result); return DataGram;
            }
            catch
            {
                return Encoding.ASCII.GetBytes("450 Directory Does Not Exists Cannot Rename");
            }
        }

        public static byte[] RFFR(FileTxInfo userdata, string Para)
        {
            try
            {
                // first checkif file exist
                var realroot = pretrail(userdata.FileShare);
                var path = realroot + "/" + userdata.RootPath + "/" + Para;
                var result = "450 Directory Does Not Exists Cannot Rename";
                if (Directory.Exists(@path))
                {
                    result = "350 Send RNTO command with new folder name";
                }
                var DataGram = Encoding.ASCII.GetBytes(result); return DataGram;
            }
            catch
            {
                return Encoding.ASCII.GetBytes("450 Directory Does Not Exists Cannot Rename");
            }
        }

        public static byte[] RFTO(FileTxInfo userdata, string Para)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] ABOR(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] DELE(FileTxInfo userdata, string Para)
        {
            try
            {
                if (String.IsNullOrEmpty(Para.Trim()))
                {
                    return Encoding.ASCII.GetBytes("450 You do not have a valid file.");
                }

                // first check if OK
                var realroot = pretrail(userdata.FileShare);
                //var filepath = realroot + "/" + (userdata.RootPath + "/").Replace("//","/") + Para;
                var filepath = Path.Combine(realroot, userdata.RootPath, Para);

                var result = "200 File Deleted Successfully";
                if (File.Exists(@filepath))
                {
                    File.Delete(@filepath);
                    //Directory.Move(Path.Combine(realroot, userdata.RootPath, filesplit[0]), Path.Combine(realroot, userdata.RootPath, filesplit[1]));
                    //result = "450 Requested file action not taken. File already exists of size " + filesize;
                    return Encoding.ASCII.GetBytes(result);
                }
                return Encoding.ASCII.GetBytes("450 You do not have a valid file.");
            }
            catch
            {
                return Encoding.ASCII.GetBytes("450 You do not have a valid file.");
            }
            //var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] RMDY(FileTxInfo userdata, string Para)
        {
            try
            {
                if (String.IsNullOrEmpty(Para.Trim()))
                {
                    return Encoding.ASCII.GetBytes("450 You do not have a valid folder name.");
                }

                // first check if OK
                var realroot = pretrail(userdata.FileShare);
                //var filepath = realroot + "/" + (userdata.RootPath + "/").Replace("//","/") + Para;
                var filepath = Path.Combine(realroot, userdata.RootPath, Para);
                Directory.Delete(@filepath, true);
                var result = "200 Folder deleted successfully";
                return Encoding.ASCII.GetBytes(result);
            }
            catch
            {
                return Encoding.ASCII.GetBytes("450 You do not have a valid folder name.");
            }
        }

        public static byte[] MKDY(FileTxInfo userdata, string Para)
        {
            try
            {
                if (String.IsNullOrEmpty(Para.Trim()))
                {
                    return Encoding.ASCII.GetBytes("450 You do not have a valid folder name.");
                }

                // first check if OK
                var realroot = pretrail(userdata.FileShare);
                //var filepath = realroot + "/" + (userdata.RootPath + "/").Replace("//","/") + Para;
                var filepath = Path.Combine(realroot, userdata.RootPath, Para);
                var result = "200 Folder created successfully";
                System.IO.Directory.CreateDirectory(@filepath);
                return Encoding.ASCII.GetBytes(result);
            }
            catch
            {
                return Encoding.ASCII.GetBytes("450 You do not have a valid folder name.");
            }
        }

        public static byte[] PWDR(FileTxInfo userdata, string Para)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] LIST(FileTxInfo userdata, string Para)
        {
            try
            {
                if (String.IsNullOrEmpty(Para.Trim())) Para = "*.*";
                bool filesOK = false;
                var realroot = pretrail(userdata.FileShare);
                //var path = realroot + "/" + userdata.RootPath;// + "/"+Para;

                var path = Path.Combine(realroot, userdata.RootPath);// + "/"+Para;
                var tryme = path.Split('/');
                var result = "212 Directory Not Found";
                if (Directory.Exists(@path))
                {
                    DynatreeItem di = new DynatreeItem(new System.IO.DirectoryInfo(@path), @realroot, filesOK, Para);
                    result = di.JsonToDynatree();
                }

                if (result.Length > 62 * 1024) result = "450 Return Object too large for UDP , use RestAPI for this job";
                var DataGram = Encoding.ASCII.GetBytes(result); return DataGram;
            }
            catch
            {
                var DataGram = Encoding.ASCII.GetBytes("450 Return Object too large for UDP , use RestAPI for this job"); return DataGram;
            }
        }

        public static byte[] NLST(FileTxInfo userdata, string Para)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] SITE(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] SYST(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] STAT(FileTxInfo userdata)
        {
            try
            {
                var report =
                              "Company=" + userdata.Company + "/r/n" + Environment.NewLine +
              "FileShare=" + userdata.FileShare + "/r/n" + Environment.NewLine +
              "RootPath=" + userdata.RootPath + "/r/n" + Environment.NewLine +
              "Parameter1=" + userdata.Parameter1 + "/r/n" + Environment.NewLine +
              "Parameter2=" + userdata.Parameter2 + "/r/n" + Environment.NewLine +
              "Parameter3=" + userdata.Parameter3 + "/r/n" + Environment.NewLine +
              "Parameter4=" + userdata.Parameter5 + "/r/n" + Environment.NewLine +
              "Parameter5=" + userdata.Parameter5 + "/r/n" + Environment.NewLine +
              "Time Expiration UTC=" + userdata.ValidTime.ToString() + "/r/n";

                var DataGram = Encoding.ASCII.GetBytes(report); return DataGram;
            }
            catch
            {
                var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
            }
        }

        public static byte[] HELP(FileTxInfo userdata)
        {
            try
            {
                var report = "Make Connection = /FTBFM/createconnections /r/n" + Environment.NewLine +
               "Command :AFFR  Action   affirmative /r/n" + Environment.NewLine +
               "Command :NEGT  Action   negate /r/n" + Environment.NewLine +
               "Command :USER  Action   user name  /r/n" + Environment.NewLine +
               "Command :PASS  Action   password /r/n" + Environment.NewLine +
               "Command :ACCT  Action   account Windows /r/n" + Environment.NewLine +
               "Command :CWDY  Action   change Working directory /r/n" + Environment.NewLine +
               "Command :CDUP  Action   change to Parent /r/n" + Environment.NewLine +
               "Command :SMNT  Action   structure mount /r/n" + Environment.NewLine +
               "Command :QUIT  Action   logout close connection /r/n" + Environment.NewLine +
               "Command :REIN  Action   reinitialize action,deletingevything /r/n" + Environment.NewLine +
               "Command :PORT  Action   dataport /r/n" + Environment.NewLine +
               "Command :PASV  Action   passive /r/n" + Environment.NewLine +
               "Command :TYPE  Action   representation type /r/n" + Environment.NewLine +
               "Command :STRU  Action   filestructure FRP /r/n" + Environment.NewLine +
               "Command :MODE  Action   transfermode stream block comressed /r/n" + Environment.NewLine +
               "Command :RETR  Action   retrieve file /r/n" + Environment.NewLine +
               "Command :STOR  Action   store file or savefile /r/n" + Environment.NewLine +
               "Command :STOU  Action   store as unique /r/n" + Environment.NewLine +
               "Command :APPE  Action   apppend to file /r/n" + Environment.NewLine +
               "Command :ALLO  Action   allocate enuough space /r/n" + Environment.NewLine +
               "Command :REST  Action   restart action /r/n" + Environment.NewLine +
               "Command :RNFR  Action   rename folder from  /r/n" + Environment.NewLine +
               "Command :RNTO  Action   rename folder to /r/n" + Environment.NewLine +
               "Command :RFFR  Action   rename file from  /r/n" + Environment.NewLine +
               "Command :RFTO  Action   rename file to /r/n" + Environment.NewLine +
               "Command :ABOR  Action   abort action delete filesend /r/n" + Environment.NewLine +
               "Command :RMDY  Action   remove directory /r/n" + Environment.NewLine +
               "Command :MKDY  Action   make directory /r/n" + Environment.NewLine +
               "Command :PWDR  Action   print directory /r/n" + Environment.NewLine +
               "Command :DELE  Action   delete file /r/n" + Environment.NewLine +
               "Command :LIST  Action   list current directory using pattern /r/n" + Environment.NewLine +
               "Command :NLST  Action   jsutfilenameandfoldername /r/n" + Environment.NewLine +
               "Command :SITE  Action   site information /r/n" + Environment.NewLine +
               "Command :SYST  Action   operatingsystemforclientandserver /r/n" + Environment.NewLine +
               "Command :STAT  Action   status of connection /r/n" + Environment.NewLine +
               "Command :HELP  Action   this help file /r/n" + Environment.NewLine +
               "Command :FIND: Action   find a file in the folder  and send its info /r/n" + Environment.NewLine +
               "Command :VERY: Action   verify user by send the item requested /r/n" + Environment.NewLine +
               "Command :SYNC: Action   STOR a file with the path /r/n" + Environment.NewLine +
               "Command :DRGD: Action   Disregard last block  /r/n" + Environment.NewLine +
               "Command :RVOK: Action   Disregard last block  /r/n" + Environment.NewLine +
               "Command :NOOP  Action   nooperation /r/n" + Environment.NewLine;
                var DataGram = Encoding.ASCII.GetBytes(report); return DataGram;
            }
            catch
            {
                var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
            }
        }

        public static byte[] DRGD(FileTxInfo userdata)
        {
            var DataGram = Encoding.ASCII.GetBytes("500 ACCESS DENIED"); return DataGram;
        }

        public static byte[] NOOP(FileTxInfo userdata)
        {
            try
            {
            }
            catch
            {
            }

            var DataGram = Encoding.ASCII.GetBytes("220 Command OK"); return DataGram;
        }

        private static string pretrail(string userdata)
        {
            var tempsharepath = Path.Combine(RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", userdata.Trim()));
            if (Directory.Exists(tempsharepath))
            {
                return tempsharepath;
            }

            return String.Empty;

            try
            {
                switch (userdata)
                {
                    case "London Jobs":
                        return "c:/TestShareRoot/Share 2/London Open Jobs/";
                        break;

                    case "Orlando Jobs":
                        return "c:/TestShareRoot/Share 2/Orlando Open Jobs/";
                        break;

                    case "Houston Jobs":
                        return "c:/TestShareRoot/Share 1/Houston Open Jobs/";
                        break;

                    case "Indianapolis Jobs":
                        return "c:/TestShareRoot/Share 1/Indianapolis Open Jobs/";
                        break;

                    case "Test Jobs Large":
                        return "c:/TestShareRoot/Temp/";
                        break;

                    case "Test Jobs Medium":
                        return "c:/TestShareRoot/Temp2/";
                        break;

                    default:
                        return "c:/TestShareRoot/Share 2/Orlando Open Jobs/";
                        break;
                }
            }
            catch
            {
                return String.Empty;
            }
        }
    }
}

/*
 *   In order to make FTB workable without needless error messages, the
      following minimum implementation is required for all servers:

         TYPE - ASCII Non-print
         MODE - Stream
         STRUCTURE - File, Record
         COMMANDS - USER, QUIT, PORT,
                    TYPE, MODE, STRU,
                      for the default values
                    RETR, STOR,
                    NOOP.

      The default values for transfer parameters are:

         TYPE - ASCII Non-print
         MODE - Stream
         STRU - File

      All hosts must accept the above as the standard defaults.
 *
 * Numeric  Order List of FTB Reply Codes

         110 Restart marker reply.
             In this case, the text is exact and not left to the
             particular implementation; it must read:
                  MARK yyyy = mmmm
             Where yyyy is User-process data stream marker, and mmmm
             server's equivalent marker (note the spaces between markers
             and "=").
         120 Service ready in nnn minutes.
         125 Data connection already open; transfer starting.
         150 File status okay; about to open data connection.
         200 Command okay.
         202 Command not implemented, superfluous at this site.
         211 System status, or system help reply.
         212 Directory status.
         213 File status.
         214 Help message.
             On how to use the server or the meaning of a particular
             non-standard command.  This reply is useful only to the
             human user.
         215 NAME system type.
             Where NAME is an official system name from the list in the
             Assigned Numbers document.
         220 Service ready for new user.
         221 Service closing control connection.
             Logged out if appropriate.
         225 Data connection open; no transfer in progress.
         226 Closing data connection.
             Requested file action successful (for example, file
             transfer or file abort).
         227 Entering Passive Mode (h1,h2,h3,h4,p1,p2).
         230 User logged in, proceed.
         250 Requested file action okay, completed.
         257 "PATHNAME" created.

         331 User name okay, need password.
         332 Need account for login.
         350 Requested file action pending further information.

         421 Service not available, closing control connection.
             This may be a reply to any command if the service knows it
             must shut down.
         425 Can't open data connection.
         426 Connection closed; transfer aborted.
         450 Requested file action not taken.
             File unavailable (e.g., file busy).
         451 Requested action aborted: local error in processing.
         452 Requested action not taken.
             Insufficient storage space in system.
         500 Syntax error, command unrecognized.
             This may include errors such as command line too long.
         501 Syntax error in parameters or arguments.
         502 Command not implemented.
         503 Bad sequence of commands.
         504 Command not implemented for that parameter.
         530 Not logged in.
         532 Need account for storing files.
         550 Requested action not taken.
             File unavailable (e.g., file not found, no access).
         551 Requested action aborted: page type unknown.
         552 Requested file action aborted.
             Exceeded storage allocation (for current directory or
             dataset).
         553 Requested action not taken.
             File name not allowed.

 *
 *
 */