using Aspose.Cells;
using DocumentFormat.OpenXml.Packaging;
using FTBAPISERVER.Models;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using OpenXmlPowerTools;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using System.Xml.Linq;
using Aspose.Words;
using Aspose.Imaging.FileFormats.Png;
using Aspose.Imaging.FileFormats.Psd;
using Aspose.Imaging.ImageOptions;
using System.Threading.Tasks;
using Ionic.Zip;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net;

namespace FTBAPISERVER.Controllers
{
    public class DbrimkusController : Controller
    {
        #region variables

        private const uint SHGFI_TYPENAME = 0x0400;
        private bool IsRootPath = true;
        private string FileShare = "";
        private string FileSharePath = "";
        private string RootPath = "";
        private string Parameter1;
        private string Parameter2;
        private string Parameter3;
        private string userwa = "";
        private string Companywa = "";
        private string searchpatternwa = "";
        private string AccessType = "Users"; // Manager";// System.Web.HttpContext.Current.Request.Headers["AccessType"] ?? "Manager"; // change it to user


        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        #endregion variables

        // GET: rimkus
        public DbrimkusController()
        { //Todo: change the manager to user
            try
            {
                userwa = System.Web.HttpContext.Current.Request.Headers["UserName"] ?? string.Empty;
                Companywa = "Houston";// System.Web.HttpContext.Current.Request.Headers["Company"] ?? string.Empty;
                searchpatternwa = System.Web.HttpContext.Current.Request.Headers["SearchPattern"] ?? string.Empty;
                //AccessType=    System.Web.HttpContext.Current.Request.Headers["AccessType"] ?? "Users"; // change it to user
                AccessType = RimkusHelper.GetuserAccess(userwa);
                //var bola = RimkusHelper.CanAccess("01234567");

                userwa = WebUtility.HtmlDecode(userwa);
                Companywa = "Houston";// System.Web.HttpContext.Current.Request.Headers["Company"] ?? string.Empty;
                searchpatternwa = WebUtility.HtmlDecode(searchpatternwa);



            }
            catch (Exception eee)
            {

                throw;
            }
        }

      
        public ActionResult Index()
        {
            // var bola = RimkusHelper.JustName(FileShare);
            return View();
        }
        public ActionResult welcome()
        {
            return View();
        }

        private string HTMLFromRtf(string rtfString)
        {
            Clipboard.SetData(DataFormats.Rtf, rtfString);
            return Clipboard.GetData(DataFormats.Html).ToString();
        }

        public ActionResult ViewText(string Id)
        {
            try
            {
                string xfilex = dbgetfilepath(Id).Replace("\\\\", "\\");
                xfilex = "\\" + xfilex;
                if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");
                FileInfo Excelfile = new FileInfo(xfilex);
                TempData["Doc"] = Excelfile.Name;
                TempData["DocPath"] = @xfilex;
                

                ViewBag.rimkus = Id;
                if (System.IO.File.Exists(@xfilex))
                {
                    ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Path.GetFileName(@xfilex).Replace(".", "DDOOTT");

                    ViewBag.Matter = System.IO.File.ReadAllText(xfilex).Replace(Environment.NewLine, "<br/>");
                    //userwa = bola.tokencheck;
                    justapilog("Viewed text Files in " + @ViewBag.fileloc + " " + Path.GetFileName(@xfilex).Replace(".", "DDOOTT"));
                }
                else
                    ViewBag.Matter = "File Not Found";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Matter = "File Not Found";
                return View();
                //throw;
            }
        }
        public string RTFView(string Id)
        {
            try
            {
                RCGRemoteFile bola = RimkusHelper.JustName(Id);
                if (bola.FileShare == "") return String.Empty;
                FileShare = bola.FileShare;
                //userwa = bola.tokencheck;

                RootPath = "";
                var path = pretrail();
                string xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                if (!System.IO.File.Exists(xfilex)) return "";
                var rtfText = System.IO.File.ReadAllText(xfilex);
                return rtfText;
            }
            catch (Exception)
            {
                return String.Empty;
                // throw;
            }
        }

        public String CombinePDF(string[] files)
        {
            //var fileDataPath ="@c:\";// WebConfigurationManager.AppSettings["FileSystemPathLaHomeOwner"];
            try
            {
                PdfDocument outputDocument = new PdfDocument();
                PdfDocument inputDocument1;
                foreach (string file in files)
                {
                    if (file == "") break;
                    if (file == null) break;

                    // PdfDocument inputDocument1 = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                    inputDocument1 = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                    int count = inputDocument1.PageCount;
                    for (int idx = 0; idx < count; idx++)
                    {
                        PdfPage page = inputDocument1.Pages[idx];
                        outputDocument.AddPage(page);
                    }
                    //inputDocument1 = null;
                    // cannot close it but can delete it inputDocument1.Close(); 
                    // delete the base file if it is Temp DOC
                    if (file.Contains("TempDOC"))
                    {
                        System.IO.File.Delete(file);
                    }
                }
                //string newFileName = @fileDataPath + @"FileData\TempDOC\" + _documentservices.GetUniqueKey(64) + ".pdf";
                //outputDocument.Save(newFileName);

            }
            catch (Exception)
            {

                // throw;
            }

            return "";// newFileName;
        }


        public string GetAllFileInfo(string Id)
        {

            string xfilex = dbgetfilepath(Id).Replace("\\\\", "\\");
            if (xfilex.Substring(0,1)=="\\") xfilex = "\\" + xfilex;
            if (!System.IO.File.Exists(xfilex)) return "[]";
            FileInfo Excelfile = new FileInfo(xfilex);
            string[] mystring = new string[5];
            mystring[0] = Excelfile.CreationTime.ToString();
            mystring[1] = Excelfile.Name.ToString();
            mystring[2] = Excelfile.LastAccessTime.ToString();
            mystring[3] = Excelfile.LastWriteTime.ToString();
            mystring[4] = Excelfile.Length.ToString();
            return JsonConvert.SerializeObject(mystring);           
        }

        public void TestPDF()
        {
            //var fileDataPath = "@c:\";     // WebConfigurationManager.AppSettings["FileSystemPatr"];
            PdfDocument outputDocument = new PdfDocument();
            outputDocument = PdfReader.Open(@"C:\TestShareRoot\test.xlsx", PdfDocumentOpenMode.Modify);
            PdfPage page = outputDocument.Pages[0];
            page.Orientation = PageOrientation.Portrait;
            // XGraphics gfx = XGraphics.FromPdfPage(page, XPageDirection.Downwards);
            XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend, XPageDirection.Downwards);
            // Draw background
            gfx.DrawImage(XImage.FromFile(@"C:\TestShareRoot\ftbxls.jpg"), 0, 0);
            outputDocument.Save("sdf");
        }

        public ActionResult ViewOutlook(string Id)
        {
            OutlookMsgStorage mymsglist = new OutlookMsgStorage();
            try
            {
                //RCGRemoteFile bola = RimkusHelper.JustName(Id);
                if (String.IsNullOrEmpty(Id)) return RedirectToAction("welcome");
                //FileShare = bola.FileShare;
                //RootPath = "";
                //var path = pretrail();
                ViewBag.rimkus = Id;

                string xfilex = dbgetfilepath(Id).Replace("\\\\", "\\");
                xfilex = "\\" + xfilex;
                if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");
                FileInfo Excelfile = new FileInfo(xfilex);
                TempData["Doc"] = Excelfile.Name;



                // remmemebr rootpath contains the file name compltet
                //string xfilex = Path.Combine(path, Parameter1);
                //string xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                //if (String.IsNullOrEmpty(Id)) return RedirectToAction("welcome");
                TempData["TempFolder"] = Excelfile.FullName; // @path;// "C:\Rimkus\TestBigFile\";

                //userwa = bola.tokencheck;

               // var inString = RimkusHelper.Base64Decode(Id);

              //  ViewBag.rimkus = RimkusHelper.Base64Encode(inString.Substring(0, inString.LastIndexOf("\\")));

                // get the list of all msg messages
                ViewBag.NoMessage = "";


                try
                {
                    var a = 0;
                    Stream messageStream = System.IO.File.Open(@xfilex, FileMode.Open, FileAccess.Read);
                    MsgStorage.Message message = new MsgStorage.Message(messageStream);
                    messageStream.Close();
                    // now we have all the stuff here 
                    DateTime mydate = DateTime.Now;
                    try
                    {
                        mydate = message.ReceivedDate;
                    }
                    catch (Exception)
                    {
                        //mydate = null;getdownicon
                    }
                    // RimkusHelper.SendLogToServer("Read Files "+@xfilex);
                    var greatfilex = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Path.GetFileName(@xfilex).Replace(".", "DDOOTT");

                    if (greatfilex.Contains(";"))
                        greatfilex = greatfilex.Replace(";", "SEMMICOLONN");

                    if (greatfilex.Contains("#"))
                        greatfilex = greatfilex.Replace("#", "HHASHTTAG");



                    mymsglist = new OutlookMsgStorage()
                    {
                        Id = a,
                        fileloc = greatfilex, // "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Path.GetFileName(@xfilex).Replace(".", "DDOOTT"),
                        From = message.From,
                        FromEmail = "NA",
                        ReceivedDate = mydate,// DateTime.Now , //message.ReceivedDate==null ? DateTime.Now :message.ReceivedDate   ,
                        ShortFrom = (message.From + "                            ").Substring(0, 20).Trim(),
                        Message = message.BodyText,//.Replace("\r\n","<br/>"),
                        shortsubject = (message.Subject + "                                 ").Substring(0, 30).Trim(),
                        subject = message.Subject,
                        Attachments = message.Attachments.Select(m => m.Filename).ToList(),
                        Recipient = message.Recipients.Select(m => m.DisplayName + " <" + m.Email + ">").ToList(),
                        RecipientEmail = message.Recipients.Select(m => m.Email).ToList(),
                        SubMessages = message.Messages.Select(m => m.From).ToList(),

                        Attachment = String.Join("<br/>", message.Attachments.Select(m => m.Filename).ToList()),
                        Recipients = String.Join("<br/>", message.Recipients.Select(m => m.DisplayName + " <" + m.Email + ">").ToList()),
                        RecipientEmails = String.Join("<br/>", message.Recipients.Select(m => m.Email).ToList()),
                        SubMessage = String.Join("<br/>", message.Messages.Select(m => m.From).ToList())
                    };
                    a++;
                    //userwa = bola.tokencheck;
                    justapilog("Viewed Mail Files in " + Path.GetFileName(@xfilex).Replace(".", "DDOOTT"));
                    //ViewBag.Message = mymsglist[0];
                }
                catch (Exception e)
                {
                    // work on it 
                    ViewBag.NoMessage = "No Outlook Messages found at location";//  e.Message;
                }
                //ViewBag.NoMessage = "";
            }
            catch (Exception)
            {
                mymsglist = new OutlookMsgStorage();
                //  throw;
            }


            return View(mymsglist);
        }

        public ActionResult Wait()
        {
            return View();
        }
        public ActionResult HTMLView(string Id)
        {
            try
            {
//                RCGRemoteFile bola = RimkusHelper.JustName(Id);
//                if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
//                FileShare = bola.FileShare;
                //userwa = bola.tokencheck;

//                RootPath = "";
//                var path = pretrail();
                ViewBag.rimkus = Id;
//                string xfilex = path + bola.RootPath;

                string xfilex = dbgetfilepath(Id);
                if (!System.IO.File.Exists(xfilex)) return Content("File access denied");

                System.IO.FileInfo fileinfo = new System.IO.FileInfo(xfilex);

                //string xfilex = @"C:\Testing RSE\testing.xlsx";// path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");


                string myhtml = System.IO.File.ReadAllText(xfilex);

                List<string> mylist;

                ViewBag.Matter = myhtml;
                ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Path.GetFileName(xfilex).Replace(".", "DDOOTT");
                //userwa = bola.tokencheck;
                justapilog("Viewed Files " + @ViewBag.fileloc + " " + Path.GetFileName(xfilex));
            }
            catch (Exception)
            {

                //throw;
            }
            return View();
        }


        public ActionResult VideoView()
        {
            var Id = TempData["VDO"];
            try
            {

                RCGRemoteFile bola = RimkusHelper.JustName(Id.ToString());
                if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
                FileShare = bola.FileShare;
                //userwa = bola.tokencheck;

                RootPath = "";
                var path = pretrail();
                ViewBag.rimkus = Id;
                string xfilex = path + bola.RootPath;

                //string xfilex = dbgetfilepath(Id);
                //string xfilex = @"C:\Testing RSE\testing.xlsx";// path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");


                var objFile = new FileInfo(xfilex);

                var stream = objFile.OpenRead();
                var objBytes = new byte[stream.Length];
                stream.Read(objBytes, 0, (int)objFile.Length);
                return File(objBytes, "video/mp4");
                //HttpContext.Response.BinaryWrite(objBytes);
                justapilog("Viewed Video Files " + path + " " + Path.GetFileName(xfilex));
            }
            catch (Exception)
            {

                //throw;
            }
            return View();
        }

        public ActionResult VDOView(string Id)
        {


            TempData["VDO"] = Id;
            string xfilex = dbgetfilepath(Id);
            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");


            var objFile = new FileInfo(xfilex);

            var stream = objFile.OpenRead();
            var objBytes = new byte[stream.Length];
            stream.Read(objBytes, 0, (int)objFile.Length);
            return File(objBytes, "video/mp4");
            //HttpContext.Response.BinaryWrite(objBytes);
            justapilog("Viewed Video Files " + Path.GetFileName(xfilex));




            return View();

        }




        public ActionResult ExcelViewXSL(string Id)
        {
            RCGRemoteFile bola = RimkusHelper.JustName(Id);
            if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
            FileShare = bola.FileShare;
            //userwa = bola.tokencheck;

            RootPath = "";
            var path = pretrail();
            ViewBag.rimkus = Id;
            // remmemebr rootpath contains the file name compltet
            //string xfilex = Path.Combine(path, Parameter1);
            string xfilex = path + bola.RootPath;
            //string xfilex = @"C:\Testing RSE\testing.xlsx";// path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");

            FileInfo Excelfile = new FileInfo(xfilex);
            TempData["Excel"] = Excelfile.Name;
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(@xfilex);
            MemoryStream mss = new MemoryStream();
            //workbook.Save("c:\\test\\myfile.htm", SaveFormat.Html);
            workbook.Save(mss, Aspose.Cells.SaveFormat.Pdf);//  FileFormatType.Pdf);
                                                            //now i have the stream check 
            byte[] fileBytes = mss.ToArray();   //System.IO.File.ReadAllBytes(@result2);
            //return view
            Response.AppendHeader("Content-Disposition", "inline; filename=" + "excel" + ".pdf");
            //userwa = bola.tokencheck;
            justapilog("Viewed Excel Files in " + @path + " " + Excelfile.Name);
            return File(fileBytes, "application/pdf");
        }

        public ActionResult About()
        {
            return View();
        }
        public ActionResult AsExcel(string Id)
        {
         //   RCGRemoteFile bola = RimkusHelper.JustName(Id);
         //   if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
         //   FileShare = bola.FileShare;
         //   RootPath = "";
         //   //userwa = bola.tokencheck;



            string xfilex = dbgetfilepath(Id).Replace("\\\\", "\\");
            xfilex = "\\" + xfilex;
            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");
            FileInfo Excelfile = new FileInfo(xfilex);
            TempData["Doc"] = Excelfile.Name;



           // var path = pretrail();
            ViewBag.rimkus = Id;
           // string xfilex = path + bola.RootPath;


            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");
            //FileInfo Excelfile = new FileInfo(xfilex);
            TempData["Excel"] = Excelfile.Name;
            TempData["ExcelPath"] = @xfilex;
            ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Excelfile.Name.Replace(".", "DDOOTT");
            justapilog("Viewed Excel Files in " + @ViewBag.fileloc + " " + Excelfile.Name);
            return View();
        }
        public ActionResult AsExcelPDF()
        {
            try
            {
                string Id = TempData["ExcelPath"].ToString();
                if (String.IsNullOrEmpty(Id))
                {
                    return Content("");
                }
                if (!System.IO.File.Exists(Id)) return Content("");

                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(@Id);
                MemoryStream mss = new MemoryStream();
                workbook.Save(mss, Aspose.Cells.SaveFormat.Pdf);//  FileFormatType.Pdf);
                byte[] fileBytes = mss.ToArray();   //System.IO.File.ReadAllBytes(@result2);           
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "excel" + ".pdf");
                ////userwa = bola.tokencheck;
                justapilog("Viewed Mail Files in " + @Id);
                return File(fileBytes, "application/pdf");

            }
            catch (Exception)
            {
                return Content("Cannot open file.Try downloading");
            }
        }


        public ActionResult AsAny(string Id)
        {
            string xfilex = dbgetfilepath(Id).Replace("\\\\", "\\");
            xfilex = "\\" + xfilex;
            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");
            FileInfo Excelfile = new FileInfo(xfilex);
            TempData["Doc"] = Excelfile.Name;
            TempData["DocPath"] = @xfilex;
            ViewBag.rimkus = Id;
            ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Excelfile.Name.Replace(".", "DDOOTT");
            justapilog("Viewed Files in " + @ViewBag.fileloc + " " + Excelfile.Name);
            return View();
        }

        public ActionResult AsAnyPDF()
        {
            MemoryStream mss; //= new MemoryStream(); 
            byte[] fileBytes;
            try
            {
                string Id = TempData["DocPath"].ToString();
                if (String.IsNullOrEmpty(Id))
                {
                    return Content("");
                }
                if (!System.IO.File.Exists(Id)) return Content("");
                FileInfo myfileinfo = new FileInfo(Id);
                switch (myfileinfo.Extension)
                {
                    case ".mp4":
                        break;

                    case ".psd":
                    case ".tiff":
                    case ".tif":
                    case ".emf":
                    case ".wmf":
                        //Stream stream = System.IO.File.OpenRead(@Id);
                        Aspose.Imaging.Image docimage = Aspose.Imaging.Image.Load(@Id);
                        //var psdImage = (PsdImage)docimage;
                        //Aspose.Imaging.Image docbook = new Aspose.Imaging.Image.Load(); // .Load(stream);
                        //stream.Close();
                        mss = new MemoryStream();
                        docimage.Save(mss, new Aspose.Imaging.ImageOptions.JpegOptions());   //System.IO.File.ReadAllBytes(@result2);    
                        fileBytes = mss.ToArray();
                        Response.AppendHeader("Content-Disposition", "inline; filename=" + "psdtiff" + ".jpg");
                        return File(fileBytes, "image/jpeg");
                        break;

                    case ".ppt":
                    case ".pot":
                    case ".potx":
                    case ".ppsx":
                    case ".pptx":
                    case ".pps":
                    case ".odp":
                        Stream stream = System.IO.File.OpenRead(@Id);
                        Aspose.Slides.Presentation docbook = new Aspose.Slides.Presentation(stream);
                        stream.Close();
                         mss = new MemoryStream();
                        docbook.Save(mss, Aspose.Slides.Export.SaveFormat.Pdf);   //System.IO.File.ReadAllBytes(@result2);    
                         fileBytes = mss.ToArray();
                        Response.AppendHeader("Content-Disposition", "inline; filename=" + "ppt" + ".pdf");
                        return File(fileBytes, "application/pdf");
                        break;
                    case ".one":
                        Aspose.Note.Document docnote = new Aspose.Note.Document(@Id);
                       
                        mss = new MemoryStream();
                        docnote.Save(mss, Aspose.Note.SaveFormat.Pdf);   //System.IO.File.ReadAllBytes(@result2);    
                        fileBytes = mss.ToArray();
                        Response.AppendHeader("Content-Disposition", "inline; filename=" + "onenote" + ".pdf");
                        return File(fileBytes, "application/pdf");
                        break;

                    case ".dwg":
                    case ".dwf":
                        break;

                    case ".3ds":
                    case ".obj":
                    case ".fbx":
                    case ".stl":

                        break;

                    case ".xls":
                    case ".xlsx":
                    case ".xlsm":
                    case ".xltx":
                    case ".xltm":
                    case ".csv":
                    case ".ods":

                        Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(@Id);
                        mss = new MemoryStream();
                        workbook.Save(mss, Aspose.Cells.SaveFormat.Pdf);//  FileFormatType.Pdf);
                        fileBytes = mss.ToArray();   //System.IO.File.ReadAllBytes(@result2);           
                        Response.AppendHeader("Content-Disposition", "inline; filename=" + "excel" + ".pdf");
                        ////userwa = bola.tokencheck;
                        justapilog("Viewed Mail Files in " + @Id);
                        return File(fileBytes, "application/pdf");
                        break;                   

                  
                    default:
                        break;
                }


                /* Stream stream = System.IO.File.OpenRead(@Id);
                 Aspose.Words.Document docbook = new Aspose.Words.Document(stream);
                 stream.Close();
                 MemoryStream mss = new MemoryStream();
                 docbook.Save(mss, Aspose.Words.SaveFormat.Pdf);//  FileFormatType.Pdf);
                 byte[] fileBytes = mss.ToArray();   //System.IO.File.ReadAllBytes(@result2);           
                 Response.AppendHeader("Content-Disposition", "inline; filename=" + "doc" + ".pdf");
                 ////userwa = bola.tokencheck;
                 justapilog("Viewed Word Files in " + @Id);
                 return File(fileBytes, "application/pdf");*/
                return Content("Cannot open file.Try downloading");
            }
            catch (Exception)
            {
                return Content("Cannot open file.Try downloading");
            }
        }


        public ActionResult AsDoc(string Id)
        {
            /*  RCGRemoteFile bola = RimkusHelper.JustName(Id);
              if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
              FileShare = bola.FileShare;
              RootPath = "";
              var path = pretrail();
              //userwa = bola.tokencheck;

              ViewBag.rimkus = Id;
              string xfilex = path + bola.RootPath;*/
            string xfilex = dbgetfilepath(Id).Replace("\\\\", "\\");
            xfilex = "\\" + xfilex;
            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");
            FileInfo Excelfile = new FileInfo(xfilex);
            TempData["Doc"] = Excelfile.Name;
            TempData["DocPath"] = @xfilex;
            ViewBag.rimkus = Id;
            ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Excelfile.Name.Replace(".", "DDOOTT");
            justapilog("Viewed Word Files in " + @ViewBag.fileloc + " " + Excelfile.Name);
            return View();
        }
        public ActionResult AsDocPDF()
        {
            try
            {
                string Id = TempData["DocPath"].ToString();
                if (String.IsNullOrEmpty(Id))
                {
                    return Content("");
                }
                if (!System.IO.File.Exists(Id)) return Content("");

                Stream stream = System.IO.File.OpenRead(@Id);
                Aspose.Words.Document docbook = new Aspose.Words.Document(stream);
                stream.Close();
                MemoryStream mss = new MemoryStream();
                docbook.Save(mss, Aspose.Words.SaveFormat.Pdf);//  FileFormatType.Pdf);
                byte[] fileBytes = mss.ToArray();   //System.IO.File.ReadAllBytes(@result2);           
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "doc" + ".pdf");
                ////userwa = bola.tokencheck;
                justapilog("Viewed Word Files in " + @Id);
                return File(fileBytes, "application/pdf");
            }
            catch (Exception)
            {
                return Content("Cannot open file.Try downloading");
            }
        }



        public ActionResult AsPPT(string Id)
        {
            /*
            RCGRemoteFile bola = RimkusHelper.JustName(Id);
            if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
            FileShare = bola.FileShare;
            RootPath = "";
            var path = pretrail();
            //userwa = bola.tokencheck;

            ViewBag.rimkus = Id;
            string xfilex = path + bola.RootPath;
            */
            string xfilex = dbgetfilepath(Id);
            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");
            FileInfo Excelfile = new FileInfo(xfilex);
            TempData["PPT"] = Excelfile.Name;
            TempData["PPTPath"] = @xfilex;
            ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Excelfile.Name.Replace(".", "DDOOTT");
            ////userwa = bola.tokencheck;
            justapilog("Viewed PowerPoint Files in " + @xfilex);
            return View();
        }
        public ActionResult AsPPTPDF()
        {
            try
            {
                string Id = TempData["PPTPath"].ToString();
                if (String.IsNullOrEmpty(Id))
                {
                    return Content("");
                }
                if (!System.IO.File.Exists(Id)) return Content("");

                Stream stream = System.IO.File.OpenRead(@Id);
                Aspose.Slides.Presentation docbook = new Aspose.Slides.Presentation(stream);
                stream.Close();
                MemoryStream mss = new MemoryStream();
                docbook.Save(mss, Aspose.Slides.Export.SaveFormat.Pdf);   //System.IO.File.ReadAllBytes(@result2);    
                byte[] fileBytes = mss.ToArray();
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "ppt" + ".pdf");
                return File(fileBytes, "application/pdf");
            }
            catch (Exception)
            {
                return Content("Cannot open file.Try downloading");
            }
        }



        public ActionResult AsPSD(string Id)
        {
            /*
            RCGRemoteFile bola = RimkusHelper.JustName(Id);
            if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
            FileShare = bola.FileShare;
            RootPath = "";
            var path = pretrail();
            ViewBag.rimkus = Id;
            //userwa = bola.tokencheck;

            string xfilex = path + bola.RootPath;
            */
            string xfilex = dbgetfilepath(Id);
            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");
            FileInfo Excelfile = new FileInfo(xfilex);
            TempData["PSD"] = Excelfile.Name;
            TempData["PSDPath"] = @xfilex;
            ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Excelfile.Name.Replace(".", "DDOOTT");
            //userwa = "";// bola.tokencheck;
            justapilog("Viewed Photoshop Files in " + @xfilex);
            return View();
        }
        public ActionResult AsPSDPDF()
        {
            try
            {
                string Id = TempData["PSDPath"].ToString();
                if (String.IsNullOrEmpty(Id))
                {
                    return Content("");
                }
                if (!System.IO.File.Exists(Id)) return Content("");

                //Stream stream = System.IO.File.OpenRead(@Id);
                Aspose.Imaging.Image docimage = Aspose.Imaging.Image.Load(@Id);
                //var psdImage = (PsdImage)docimage;
                //Aspose.Imaging.Image docbook = new Aspose.Imaging.Image.Load(); // .Load(stream);
                //stream.Close();
                MemoryStream mss = new MemoryStream();
                docimage.Save(mss, new Aspose.Imaging.ImageOptions.JpegOptions());   //System.IO.File.ReadAllBytes(@result2);    
                byte[] fileBytes = mss.ToArray();
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "psdtiff" + ".jpg");
                return File(fileBytes, "image/jpeg");
            }
            catch (Exception eee)
            {
                return Content("Cannot open file.Try downloading");
            }
        }


        public ActionResult AsNote(string Id)
        {
            /*
            RCGRemoteFile bola = RimkusHelper.JustName(Id);
            if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
            FileShare = bola.FileShare;
            RootPath = "";
            var path = pretrail();
            ViewBag.rimkus = Id;
            string xfilex = path + bola.RootPath;
            */
            string xfilex = dbgetfilepath(Id);

            //userwa = "";// bola.tokencheck;
            ViewBag.rimkus = Id;
            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");
            FileInfo Excelfile = new FileInfo(xfilex);
            TempData["NOT"] = Excelfile.Name;
            TempData["NOTPath"] = @xfilex;
            ////userwa = bola.tokencheck;
            justapilog("Viewed Note Files in " + @xfilex);
            ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Excelfile.Name.Replace(".", "DDOOTT");
            return View();
        }
        public ActionResult AsNotePDF()
        {
            try
            {
                string Id = TempData["NOTPath"].ToString();
                if (String.IsNullOrEmpty(Id))
                {
                    return Content("");
                }
                if (!System.IO.File.Exists(Id)) return Content("");

                //Stream stream = System.IO.File.OpenRead(@Id);
                Aspose.Note.Document docnote = new Aspose.Note.Document(@Id);
                //var psdImage = (PsdImage)docimage;
                //Aspose.Imaging.Image docbook = new Aspose.Imaging.Image.Load(); // .Load(stream);
                //stream.Close();
                MemoryStream mss = new MemoryStream();
                docnote.Save(mss, Aspose.Note.SaveFormat.Pdf);   //System.IO.File.ReadAllBytes(@result2);    
                byte[] fileBytes = mss.ToArray();
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "onenote" + ".pdf");
                return File(fileBytes, "application/pdf");
            }
            catch (Exception eee)
            {
                return Content("Cannot open file.Try downloading");
            }
        }

        public ActionResult AsPDF(string Id)
        {
            /*            RCGRemoteFile bola = RimkusHelper.JustName(Id);
                        if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
                        FileShare = bola.FileShare;
                        RootPath = "";
                        var path = pretrail();
                        //userwa = bola.tokencheck;

                        ViewBag.rimkus = Id;
                        string xfilex = path + bola.RootPath;*/
            string xfilex = dbgetfilepath(Id);
            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");
            FileInfo Excelfile = new FileInfo(xfilex);
            TempData["PDF"] = Excelfile.Name;
            TempData["PDFPath"] = @xfilex;
            ViewBag.rimkus = Id;
            ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Excelfile.Name.Replace(".", "DDOOTT");
            //userwa = "";// bola.tokencheck;
            justapilog("Viewed files in " + @xfilex + " " + Excelfile.Name);
            return View();
        }

        public ActionResult AsPDFPDF()
        {
            try
            {
                string Id = TempData["PDFPath"].ToString();
                if (String.IsNullOrEmpty(Id))
                {
                    return Content("");
                }
                if (!System.IO.File.Exists(Id)) return Content("");

                //Stream stream = System.IO.File.OpenRead(@Id);
                Aspose.Pdf.Document docnote = new Aspose.Pdf.Document(@Id);
                //var psdImage = (PsdImage)docimage;
                //Aspose.Imaging.Image docbook = new Aspose.Imaging.Image.Load(); // .Load(stream);
                //stream.Close();
                MemoryStream mss = new MemoryStream();
                docnote.Save(mss);//, Aspose.Note.SaveFormat.Pdf);   //System.IO.File.ReadAllBytes(@result2);    
                byte[] fileBytes = mss.ToArray();
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "Pdf" + ".pdf");
                return File(fileBytes, "application/pdf");
            }
            catch (Exception eee)
            {
                return Content("Cannot open file.Try downloading");
            }
        }

        public string getjustfiles(string Id)
        {
            RCGRemoteFile bola = RimkusHelper.JustName(Id);
            if (bola.FileShare == "") return String.Empty;
            FileShare = bola.FileShare;
            ////userwa = bola.tokencheck;

            RootPath = "";// bola.RootPath;// .FileShare;// ""; // bola.RootPath;
                          // error if path is not found on allshare look
            var path = pretrail();
            // remmemebr rootpath contains the file name compltet
            //var path = pretrail();
            //string xfilex = Path.Combine(path, Param
            string xfilex = path + bola.RootPath;
            if (!System.IO.Directory.Exists(xfilex))
                xfilex = @Path.Combine(path, bola.RootPath);
            // see if the filename is in the rootpath
            if (!System.IO.Directory.Exists(xfilex))
                xfilex = @Path.Combine(path, bola.RootPath.Substring(0, bola.RootPath.LastIndexOf("\\")));
            if (!System.IO.Directory.Exists(xfilex)) return String.Empty;

            String[] allfiles = System.IO.Directory.GetFiles(xfilex, "*.*", System.IO.SearchOption.AllDirectories);
            string[] items2 = allfiles.Select(x => x.Replace(FileSharePath, "CHKSUM*FILE*" + FileShare + "*")).ToArray();
            ////userwa = bola.tokencheck;
            justapilog("Viewed Files in " + @path);
            return JsonConvert.SerializeObject(items2);
        }


        public ActionResult PDFReturn(string Id)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"c:\Rimkus\Claim441.pdf");
            return File(fileBytes, "application/pdf");
        }
        public ActionResult ExcelView(string Id)
        {
            try
            {
                RCGRemoteFile bola = RimkusHelper.JustName(Id);
                if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
                FileShare = bola.FileShare;
                RootPath = "";
                var path = pretrail();
                //userwa = bola.tokencheck;

                ViewBag.rimkus = Id;
                // remmemebr rootpath contains the file name compltet
                //string xfilex = Path.Combine(path, Parameter1);
                string xfilex = path + bola.RootPath;
                //string xfilex = @"C:\Testing RSE\testing.xlsx";// path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");

                FileInfo Excelfile = new FileInfo(xfilex);
                TempData["Excel"] = Excelfile.Name;

                Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
                workbook.LoadFromFile(@xfilex, ExcelVersion.Version2013);

                string myhtml = "";
                string result = Path.Combine(@Path.GetTempPath(), Guid.NewGuid().ToString()); // Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                List<string> mylist;

                string excelsheetbola = "<div class='container-fluid'>" +
                  "	<div class='row'>" +
                  "		<div class='col-md-12'>" +
                  "			<div class='tabbable' id='tabs-892045'>" +
                  "				<ul class='nav nav-tabs'>" +
                  "" + "tabboo" +

                  "				</ul>" +
                  "				<div class='tab-content'>" +
                  "" + "mattoo" +
                  "				</div>" +
                  "			</div>" +
                  "		</div>" +
                  "	</div>" +
                  "</div>";


                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {
                    myhtml += "<br/><br/><h1>" + string.Format("WORKSHEET " + "-{0}", i + 1) + "</h1><br/><br/>";
                    Spire.Xls.Worksheet sheet = workbook.Worksheets[i];
                    string htmlPath = string.Format(@result + "-{0}.html", i);


                    sheet.SaveToHtml(htmlPath);
                    FileInfo htmlfilex = new FileInfo(htmlPath);
                    var FilePath = htmlfilex.DirectoryName;
                    var RealFileName = htmlfilex.Name.Replace(".html", "_files");
                    var temphtml = System.IO.File.ReadAllText(htmlPath);
                    mylist = FetchLinksFromSource(temphtml);
                    foreach (var item in mylist)
                    {
                        string myfilename = item.Substring(item.IndexOf("\\") + 1);
                        myfilename = myfilename.Substring(0, myfilename.IndexOf("\""));
                        byte[] fileBBytes = System.IO.File.ReadAllBytes(@Path.Combine(FilePath, @RealFileName, myfilename));
                        System.IO.File.Delete(@Path.Combine(FilePath, @RealFileName, myfilename));
                        //var filebbstring = Convert.ToBase64String(fileBBytes);
                        var stringconvert = "data:image/png;base64," + Convert.ToBase64String(fileBBytes);
                        temphtml = temphtml.Replace(@Path.Combine(@RealFileName, myfilename), stringconvert);
                        // now lets replace the stuff
                    }

                    temphtml = temphtml.Replace("?", "");
                    temphtml = temphtml.GetBetween("<body>", "</body>");

                    myhtml += temphtml;// System.IO.File.ReadAllText(htmlPath);
                                       // create the sheet

                    var activa = i == 0 ? "active" : "";
                    var gid = Guid.NewGuid().ToString().Substring(0, 5);
                    var tabbo = "<li><a href='#" + gid + "' data-toggle='tab'>Sheet " + (i + 1).ToString() + "</a></li>";
                    var mattoo = "<div class='tab-pane " + activa + "' id='" + gid + "'><div> " + temphtml + "</div></div>";
                    excelsheetbola = excelsheetbola.Replace("tabboo", tabbo + "tabboo");
                    excelsheetbola = excelsheetbola.Replace("mattoo", mattoo + "mattoo");


                    System.IO.File.Delete(htmlPath);
                    try
                    {
                        System.IO.Directory.Delete(@Path.Combine(@FilePath, @RealFileName));
                    }
                    catch (Exception eee)
                    {

                        //throw;
                    }
                }
                //            myhtml = myhtml.Replace("Evaluation&nbsp;Warning:The&nbsp;Document&nbsp;was&nbsp;created&nbsp;with&nbsp;Spire.XLS&nbsp;for&nbsp;.NET", "");
                myhtml = myhtml.Replace("?", "");
                myhtml = myhtml.GetBetween("<body>", "</body>");

                excelsheetbola = excelsheetbola.Replace("tabboo", "");
                excelsheetbola = excelsheetbola.Replace("mattoo", "");

                //            mylist = FetchLinksFromSource(myhtml);

                // do the magic
                ViewBag.Matter = excelsheetbola;


                /*   
                            try
                {
                 //   var addheader = System.IO.File.ReadAllText(Server.MapPath("~/HeaderExcel.txt"));
                 //   excelsheetbola = addheader + excelsheetbola + "</body></html>";
                }
                catch (Exception rrr)
                {
                  //  excelsheetbola = rrr.Message;
                    //throw;
                }

                byte[] fileBytes = Encoding.ASCII.GetBytes(excelsheetbola);  //System.IO.File.ReadAllBytes(@result2);
                //return view
                Response.AppendHeader("Content-Disposition", "inline; filename=" + "excel" + ".html");
                return File(fileBytes, "text/html");
                */
                //userwa = bola.tokencheck;
                justapilog("Viewed Excel Files " + @path);
                ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Path.GetFileName(xfilex).Replace(".", "DDOOTT");
            }
            catch (Exception)
            {

                //throw;
            }
            return View();
        }


        public ActionResult ExcelViewold(string Id)
        {
            RCGRemoteFile bola = RimkusHelper.JustName(Id);
            if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
            FileShare = bola.FileShare;
            RootPath = "";
            var path = pretrail();
            //userwa = bola.tokencheck;

            // remmemebr rootpath contains the file name compltet
            //string xfilex = Path.Combine(path, Parameter1);
            string xfilex = path + bola.RootPath;
            //string xfilex = @"C:\Testing RSE\testing.xlsx";// path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");

            FileInfo Excelfile = new FileInfo(xfilex);
            TempData["Excel"] = Excelfile.Name;

            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.LoadFromFile(@xfilex, ExcelVersion.Version2013);

            string myhtml = "";
            string result = Path.Combine(@Path.GetTempPath(), Guid.NewGuid().ToString()); // Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            List<string> mylist;
            for (int i = 0; i < workbook.Worksheets.Count; i++)
            {
                myhtml += "<br/><br/><h1>" + string.Format("WORKSHEET " + "-{0}", i + 1) + "</h1><br/><br/>";
                Spire.Xls.Worksheet sheet = workbook.Worksheets[i];
                string htmlPath = string.Format(@result + "-{0}.html", i);


                sheet.SaveToHtml(htmlPath);
                FileInfo htmlfilex = new FileInfo(htmlPath);
                var FilePath = htmlfilex.DirectoryName;
                var RealFileName = htmlfilex.Name.Replace(".html", "_files");
                var temphtml = System.IO.File.ReadAllText(htmlPath);
                mylist = FetchLinksFromSource(temphtml);
                foreach (var item in mylist)
                {
                    string myfilename = item.Substring(item.IndexOf("\\") + 1);
                    myfilename = myfilename.Substring(0, myfilename.IndexOf("\""));
                    byte[] fileBBytes = System.IO.File.ReadAllBytes(@Path.Combine(FilePath, @RealFileName, myfilename));
                    System.IO.File.Delete(@Path.Combine(FilePath, @RealFileName, myfilename));
                    //var filebbstring = Convert.ToBase64String(fileBBytes);
                    var stringconvert = "data:image/png;base64," + Convert.ToBase64String(fileBBytes);
                    temphtml = temphtml.Replace(@Path.Combine(@RealFileName, myfilename), stringconvert);
                    // now lets replace the stuff
                }
                myhtml += temphtml;// System.IO.File.ReadAllText(htmlPath);
                System.IO.File.Delete(htmlPath);
                try
                {
                    System.IO.Directory.Delete(@Path.Combine(@FilePath, @RealFileName));
                }
                catch (Exception eee)
                {

                    //throw;
                }
            }
            myhtml = myhtml.Replace("Evaluation&nbsp;Warning:The&nbsp;Document&nbsp;was&nbsp;created&nbsp;with&nbsp;Spire.XLS&nbsp;for&nbsp;.NET", "");
            myhtml = myhtml.Replace("?", "");
            //            mylist = FetchLinksFromSource(myhtml);

            // do the magic


            byte[] fileBytes = Encoding.ASCII.GetBytes(myhtml);  //System.IO.File.ReadAllBytes(@result2);
            //return view
            Response.AppendHeader("Content-Disposition", "inline; filename=" + "excel" + ".html");
            return File(fileBytes, "text/html");
        }

        //        public List<Uri> FetchLinksFromSource(string htmlSource)
        public List<string> FetchLinksFromSource(string htmlSource)
        {
            //            List<Uri> links = new List<Uri>();
            List<string> links = new List<string>();

            string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            MatchCollection matchesImgSrc = Regex.Matches(htmlSource, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in matchesImgSrc)
            {
                // string href = m.Groups[1].Value;
                string href = m.Value;// Groups[1].Value;
                links.Add(href);// (new Uri(href));
            }
            return links;
        }

        public ActionResult ExcelPDFView(string Id)
        {

            RCGRemoteFile bola = RimkusHelper.JustName(Id);
            if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
            FileShare = bola.FileShare;
            RootPath = "";
            var path = pretrail();
            //userwa = bola.tokencheck;

            // remmemebr rootpath contains the file name compltet
            //string xfilex = Path.Combine(path, Parameter1);
            string xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");

            FileInfo Excelfile = new FileInfo(xfilex);
            TempData["Excel"] = Excelfile.Name;

            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.LoadFromFile(@xfilex, ExcelVersion.Version2013);


            for (int i = 0; i < workbook.Worksheets.Count; i++)

            {
                Spire.Xls.Worksheet sheet = workbook.Worksheets[i];

                string htmlPath = string.Format(@"C:\Testing RSE\resultFileName" + " -{0}.html", i++);

                sheet.SaveToHtml(htmlPath);
            }





            string result = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
            string result2 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
            workbook.SaveToFile(@result, Spire.Xls.FileFormat.PDF);
            // now work out the logo

            PdfDocument outputDocument = new PdfDocument();
            outputDocument = PdfReader.Open(@result, PdfDocumentOpenMode.Modify);
            for (int i = 0; i < outputDocument.Pages.Count; i++)
            {
                PdfPage page = outputDocument.Pages[i];
                page.Orientation = PageOrientation.Portrait;
                // XGraphics gfx = XGraphics.FromPdfPage(page, XPageDirection.Downwards);

                try
                {
                    XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend, XPageDirection.Downwards);
                    gfx.DrawImage(XImage.FromFile(@"C:\TestShareRoot\ftbytesxsl.jpg"), 0, 0);
                }
                catch (Exception e)
                {
                    return new FileContentResult(new byte[] { }, "PDF");
                    var fff = "sdsd";
                    //throw;
                }
            }

            // Draw background


            outputDocument.Save(@result2);

            System.IO.File.Delete(@result);
            TempData["TempFolder"] = @result2;
            TempData["ID"] = Id;
            //userwa = bola.tokencheck;
            justapilog("Viewed Excel Files in " + @path);
            return RedirectToAction("viewPDF");

            /*
            byte[] fileBytes = System.IO.File.ReadAllBytes(@result2);// "C:\rimkus\test.pdf");// System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Uploads/records.csv"));
            FileInfo fileinfo = new FileInfo(xfilex);
            string fileName = fileinfo.Name; // filepath.Substring(filepath.LastIndexOf("//"));// "testpdf.pdf";// CsvName;
            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            string mimeType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName+".pdf");
            return File(fileBytes, mimeType);
            //eturn View();
            */
        }
        public ActionResult OldExcelView()
        {
            string _XlsConnectionStringFormat = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=NO;IMEX=1\"";
            string xlsFilename = @"C:\TestShareRoot\test.xlsx";
            using (OleDbConnection conn = new OleDbConnection(string.Format(_XlsConnectionStringFormat, xlsFilename)))
            {
                try
                {
                    conn.Open();

                    string outputFilenameHeade = Path.GetFileNameWithoutExtension(xlsFilename);
                    string dir = Path.GetDirectoryName(xlsFilename);
                    string[] sheetNames = conn.GetSchema("Tables")
                                              .AsEnumerable()
                                              .Select(a => a["TABLE_NAME"].ToString())
                                              .ToArray();
                    foreach (string sheetName in sheetNames)
                    {
                        string outputFilename = Path.Combine(dir, string.Format("{0}_{1}.csv", outputFilenameHeade, sheetName));
                        using (StreamWriter sw = new StreamWriter(System.IO.File.Create(outputFilename), Encoding.Unicode))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                using (OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}]", sheetName), conn))
                                {
                                    adapter.Fill(ds);

                                    foreach (DataRow dr in ds.Tables[0].Rows)
                                    {
                                        string[] cells = dr.ItemArray.Select(a => a.ToString()).ToArray();
                                        sw.WriteLine("\"{0}\"", string.Join("\"\t\"", cells));
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exp)
                {

                    var ddd = exp.Message;
                    // handle exception
                }
                finally
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        try
                        {
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            // handle exception
                        }
                    }
                }
            }
            return View();
        }


        public ActionResult WordViewDoc(string Id)
        {
            RCGRemoteFile bola = RimkusHelper.JustName(Id);
            if (bola.FileShare == "") return Content("File Access Error");// new  FileContentResult(new byte[] { }, "PDF");
            FileShare = bola.FileShare;
            RootPath = "";
            var path = pretrail();
            //userwa = bola.tokencheck;

            ViewBag.rimkus = Id;
            // remmemebr rootpath contains the file name compltet
            //string xfilex = Path.Combine(path, Parameter1);
            string xfilex = path + bola.RootPath;
            //string xfilex = @"C:\Testing RSE\testing.xlsx";// path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
            if (!System.IO.File.Exists(xfilex)) return Content("File Access Error");// new FileContentResult(new byte[] { }, "PDF");

            FileInfo Excelfile = new FileInfo(xfilex);
            TempData["Excel"] = Excelfile.Name;
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(@xfilex);
            MemoryStream mss = new MemoryStream();
            workbook.Save(mss, Aspose.Cells.SaveFormat.Pdf);//  FileFormatType.Pdf);
                                                            //now i have the stream check 
            byte[] fileBytes = mss.ToArray();   //System.IO.File.ReadAllBytes(@result2);
            //return view
            Response.AppendHeader("Content-Disposition", "inline; filename=" + "word" + ".pdf");
            //userwa = bola.tokencheck;
            justapilog("Viewed Word Files in " + @path);
            return File(fileBytes, "application/pdf");

        }

        public ActionResult WordView(string Id)
        {
            RCGRemoteFile bola = RimkusHelper.JustName(Id);
            if (bola.FileShare == "") return Content("File cannot be read");
            FileShare = bola.FileShare;
            RootPath = "";
            //userwa = bola.tokencheck;

            var path = pretrail();

            string xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";

            // string xfilex = @"c:\testshareroot\Manual.docx";
            if (!System.IO.File.Exists(xfilex)) return Content("File access denied");

            ViewBag.rimkus = Id;
            ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Path.GetFileName(@xfilex).Replace(".", "DDOOTT");
            /* if (xfilex.ToLower().Contains(".rtf"))
             {
                 var rtfText = System.IO.File.ReadAllText(xfilex);
                 ViewBag.Content =rtfText HTMLFromRtf(rtfText);
                 return View();
             }  
             */
            try
            {
                byte[] byteArray = System.IO.File.ReadAllBytes(@xfilex);// "C:\Testing RSE\TestDoc.docx");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(byteArray, 0, byteArray.Length);
                    using (WordprocessingDocument doc =
                        WordprocessingDocument.Open(memoryStream, true))
                    {
                        HtmlConverterSettings settings = new HtmlConverterSettings()
                        {
                            PageTitle = "File on Server"
                        };
                        XElement html = HtmlConverter.ConvertToHtml(doc, settings);
                        ViewBag.Content = html.ToStringNewLineOnAttributes();
                        //return Content(bola);
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Content = "Document cannot be opened , you need to Drag and Save in order to view it locally"; // e.Message +"<br/> or file Access denied or reading error";// DateTime.Now.ToLongTimeString() + " " + Environment.NewLine + e.Message + Environment.NewLine + e.Source + Environment.NewLine + e.StackTrace + Environment.NewLine + e.TargetSite + Environment.NewLine;
                //throw;
            }




            return View();
        }
        public string getfileinfo()
        {
            FileInfo fileinfo = new FileInfo(@"c:\rimkus\img100");
            return JsonConvert.SerializeObject(fileinfo);
        }

        public ActionResult MsgView(string Id)
        {
            string greatfilex = "";

            if (String.IsNullOrEmpty(Id)) return RedirectToAction("welcome");

            var mypath = RimkusHelper.Base64Decode(Id);
            string mypresentpath = mypath.Substring(mypath.LastIndexOf("\\") + 1);
            var tola = mypresentpath.Split('^');
            tola[0] = tola[0].Trim();
            tola[1] = RimkusHelper.GetBetween(tola[1], "[", "]");


            /*            if (String.IsNullOrEmpty(Id)) return RedirectToAction("welcome");
                        RCGRemoteFile bola = RimkusHelper.JustName(Id);
                        if (bola.FileShare == "") return RedirectToAction("welcome");            
                        FileShare = bola.FileShare;
                        RootPath = bola.RootPath;
                        //userwa = bola.tokencheck;
                        */
            var path = dbgetdirpath(tola[1]) + tola[0];//  pretrail();
            //var path = pretrail();
            TempData["TempFolder"] = @path;// "C:\Rimkus\TestBigFile\";
            ViewBag.rimkus = Id;
            // get the list of all msg messages
            ViewBag.NoMessage = "";
            List<OutlookMsgStorage> mymsglist = new List<OutlookMsgStorage>();
            try
            {
                // Only get files that begin with the msg "msg"
                //string[] dirs = Directory.GetFiles(@"C:\TestShareRoot\Share 2\London Open Jobs\02212258 RVB\Emails", "*.msg");
                string[] dirs;
                try
                {
                    dirs = Directory.GetFiles(@path, "*.msg");// "C:\Rimkus\Share 2\London Open Jobs\zz02212258 RVB\Emails", "*.msg");

                }
                catch (Exception)
                {
                    mymsglist.Add(new OutlookMsgStorage
                    {
                        Id = 0,
                        From = "",
                        fileloc = "",
                        FromEmail = "",
                        ReceivedDate = DateTime.Now, //message.ReceivedDate==null ? DateTime.Now :message.ReceivedDate   ,
                        ShortFrom = "",// (message.From + "                            ").Substring(0, 20).Trim(),
                        Message = "",//message.BodyText,//.Replace("\r\n","<br/>"),
                        shortsubject = "", //(message.Subject + "                                 ").Substring(0, 30).Trim(),
                        subject = "", //message.Subject,
                        Attachments = new List<string>() { "" },// message.Attachments.Select(m => m.Filename).ToList(),
                        Recipient = new List<string>() { "" },// message.Recipients.Select(m => m.DisplayName + " <" + m.Email + ">").ToList(),
                        RecipientEmail = new List<string>() { "" },// message.Recipients.Select(m => m.Email).ToList(),
                        SubMessages = new List<string>() { "" }, // message.Messages.Select(m => m.From).ToList()
                        Attachment = "",// String.Join("<br/>", message.Attachments.Select(m => m.Filename).ToList()),
                        Recipients = "", // String.Join("<br/>", message.Recipients.Select(m => m.DisplayName + " <" + m.Email + ">").ToList()),
                        RecipientEmails = "", //String.Join("<br/>", message.Recipients.Select(m => m.Email).ToList()),
                        SubMessage = "" //String.Join("<br/>", message.Messages.Select(m => m.From).ToList())



                    });
                    ViewBag.NoMessage = "No Outlook Message(s) in this folder.";
                    ViewBag.Message = mymsglist[0];
                    return View(mymsglist);
                }
                if (dirs.Length < 1)
                {
                    ViewBag.NoMessage = "No Outlook Message(s) in this folder.";
                    mymsglist.Add(new OutlookMsgStorage
                    {
                        Id = 0,
                        From = "",
                        fileloc = "",
                        FromEmail = "",
                        ReceivedDate = DateTime.Now, //message.ReceivedDate==null ? DateTime.Now :message.ReceivedDate   ,
                        ShortFrom = "",// (message.From + "                            ").Substring(0, 20).Trim(),
                        Message = "",//message.BodyText,//.Replace("\r\n","<br/>"),
                        shortsubject = "", //(message.Subject + "                                 ").Substring(0, 30).Trim(),
                        subject = "", //message.Subject,
                        Attachments = new List<string>() { "" },// message.Attachments.Select(m => m.Filename).ToList(),
                        Recipient = new List<string>() { "" },// message.Recipients.Select(m => m.DisplayName + " <" + m.Email + ">").ToList(),
                        RecipientEmail = new List<string>() { "" },// message.Recipients.Select(m => m.Email).ToList(),
                        SubMessages = new List<string>() { "" },// message.Messages.Select(m => m.From).ToList()
                        Attachment = "",// String.Join("<br/>", message.Attachments.Select(m => m.Filename).ToList()),
                        Recipients = "", // String.Join("<br/>", message.Recipients.Select(m => m.DisplayName + " <" + m.Email + ">").ToList()),
                        RecipientEmails = "", //String.Join("<br/>", message.Recipients.Select(m => m.Email).ToList()),
                        SubMessage = "" //String.Join("<br/>", message.Messages.Select(m => m.From).ToList())

                    });
                    ViewBag.NoMessage = "No Outlook Message(s) in this folder.";
                    ViewBag.Message = mymsglist[0];
                    //userwa = " ";//bola.tokencheck;
                    justapilog("Viewed Mail Files in " + @path);
                    return View(mymsglist);
                }
                var a = 0;
                foreach (string dir in dirs)
                {
                    // now open them one by onec

                    Stream messageStream = System.IO.File.Open(dir, FileMode.Open, FileAccess.Read);
                    MsgStorage.Message message = new MsgStorage.Message(messageStream);
                    messageStream.Close();
                    // now we have all the stuff here 
                    DateTime mydate = DateTime.Now;

                    try
                    {
                        mydate = message.ReceivedDate;
                    }
                    catch (Exception)
                    {
                        //mydate = null;getdownicon
                    }
                    greatfilex = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Path.GetFileName(dir).Replace(".", "DDOOTT");

                    if (greatfilex.Contains(";"))
                        greatfilex = greatfilex.Replace(";", "SEMMICOLONN");

                    if (greatfilex.Contains("#"))
                        greatfilex = greatfilex.Replace("#", "HHASHTTAG");

                    mymsglist.Add(new OutlookMsgStorage
                    {
                        Id = a,
                        //fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Path.GetFileName(dir).Replace(".", "DDOOTT"),
                        fileloc = greatfilex,
                        From = message.From,
                        FromEmail = "NA",
                        ReceivedDate = mydate,// DateTime.Now , //message.ReceivedDate==null ? DateTime.Now :message.ReceivedDate   ,
                        ShortFrom = (message.From + "                            ").Substring(0, 20).Trim(),
                        Message = message.BodyText,//.Replace("\r\n","<br/>"),
                        shortsubject = (message.Subject + "                                 ").Substring(0, 30).Trim(),
                        subject = message.Subject,
                        Attachments = message.Attachments.Select(m => m.Filename).ToList(),
                        Recipient = message.Recipients.Select(m => m.DisplayName + " <" + m.Email + ">").ToList(),
                        RecipientEmail = message.Recipients.Select(m => m.Email).ToList(),
                        SubMessages = message.Messages.Select(m => m.From).ToList(),
                        Attachment = String.Join("<br/>", message.Attachments.Select(m => m.Filename).ToList()),
                        Recipients = String.Join("<br/>", message.Recipients.Select(m => m.DisplayName + " <" + m.Email + ">").ToList()),
                        RecipientEmails = String.Join("<br/>", message.Recipients.Select(m => m.Email).ToList()),
                        SubMessage = String.Join("<br/>", message.Messages.Select(m => m.From).ToList())
                    });
                    a++;
                }
                //ViewBag.Message = mymsglist[0];
            }
            catch (Exception e)
            {
                // work on it 
                ViewBag.NoMessage = "No Outlook Messages found at location";//  e.Message;
            }
            //ViewBag.NoMessage = "";
            if (mymsglist.Count > 0) ViewBag.Message = mymsglist[0];
            return View(mymsglist);
        }
        public string makenewfolder(string Id)
        {
            try
            {
                RCGRemoteFile bola = RimkusHelper.JustName(Id);

                if (bola.FileShare == "") return "No active token";
                FileShare = bola.FileShare;
                RootPath = bola.RootPath;
                // //userwa = bola.tokencheck;

                // error if path is not found on allshare look
                var path = pretrail();
                try
                {

                    Directory.CreateDirectory(Path.Combine(path, newfoldername("New Folder" + DateTime.Now.ToLongTimeString() + FileShare + RootPath)));
                    justapilog("Created New Folder in" + @path);
                }
                catch
                {
                    return "FAILED";
                }

                return "OK";
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }
        private string newfoldername(string input)
        {
            byte[] data = Encoding.ASCII.GetBytes(input);
            long longsum = data.Sum(X => (long)X);
            var checksum = unchecked(longsum);
            return (checksum.ToString() + "0000000000").Substring(0, 8);
            //return unchecked((byte)longsum);
        }
        public ActionResult EmptyLoad()
        {
            return View();
        }
        public string mobiledir(string Id)
        {
            string foldername = "";
            string assignmgr;
            string jobinfo = "";
            try
            {
                RCGRemoteFile bola = RimkusHelper.JustName(Id);

                if (bola.FileShare == "") return "No active token";
                FileShare = bola.FileShare;
                RootPath = bola.RootPath;
                // //userwa = bola.tokencheck;

                // error if path is not found on allshare look
                var path = pretrail();
                // get th ejob number
                try
                {
                    if (!RootPath.Contains("\\")) { foldername = RootPath; }
                    else
                    {
                        foldername = RootPath.Substring(0, RootPath.IndexOf("\\"));
                        foldername = foldername.Substring(0, foldername.IndexOf("\\"));
                    }
                }
                catch (Exception)
                {

                    // throw;
                }
                finally
                {
                    //  if (!String.IsNullOrEmpty(foldername))
                    //    { jobinfo = RimkusHelper.GetJobInfo(foldername); }
                }




                if (!IsRootPath)
                {
                    if (AccessType != "Manager")
                    {
                        // get the folder name
                        foldername = RootPath.Substring(RootPath.LastIndexOf("\\") + 1);
                        //string folderbola = RimkusHelper.CanAccess(foldername);
                        var folderbola = userwa.ToUpper();
                        if (RootPath.Count(c => c == '\\') == 1)
                        {
                            folderbola = RimkusHelper.CanAccess(foldername);
                            //   jobinfo = RimkusHelper.GetJobInfo(foldername);
                        }
                        if (!folderbola.ToUpper().Contains(userwa.ToUpper()))
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
                            justapilog("Access denied to folder " + @foldername);
                            return JsonConvert.SerializeObject(serverblankfiles);
                            //return String.Empty;
                        }
                    }
                }


                var myfolders = new List<string>();
                var serverfiles = new List<LocalFileStruc>() { };
                var directories = System.IO.Directory.EnumerateDirectories(@path);


                /*
                if (!String.IsNullOrEmpty(searchpatternwa))
                {
                   if (System.IO.Directory.EnumerateDirectories(@path,"*"+searchpatternwa+"*", SearchOption.AllDirectories).Count() > 0 )
                    {
                        directories = System.IO.Directory.EnumerateDirectories(@path,"*"+ searchpatternwa+"*", SearchOption.AllDirectories);
                        searchpatternwa = "";
                    }
                }
                  */

                //RimkusHelper.WriteLocalLog(//userwa, "Folder:"+@path);
                justapilog("Access approved to folder " + @path);

                // try this approach
                //directories = directories.Where(x => x == "sd");

                //if (!String.IsNullOrEmpty(searchpatternwa))
                //{
                //    directories = directories.Where(x => x.Contains(searchpatternwa));
                //}

                serverfiles.Add(new LocalFileStruc()
                {
                    //Selected = false,
                    filename = jobinfo,
                    filesize = 0,
                    filetype = "DIRR",
                    fullpath = "CHKSUM*DIR*DUMMY*",
                    attributes = "+R+E+D"
                });


                foreach (var directory in directories)
                {
                    var dirX = new DirectoryInfo(@directory);
                    serverfiles.Add(new LocalFileStruc()
                    {
                        //Selected = false,
                        filename = Path.GetFileName(directory),
                        filesize = 0,
                        filetype = "DIR",
                        fileicon = "directory.png",
                        creationtime = dirX.CreationTimeUtc,
                        lastwritetime = dirX.LastWriteTimeUtc,
                        //fullpath = directory.Replace(FileSharePath, FileShare),
                        fullpath = "CHKSUM*DIR*" + directory.Replace(FileSharePath, FileShare + "*"),
                        attributes = "+R+E+D"
                    });
                }

                if (IsRootPath) return JsonConvert.SerializeObject(serverfiles);



                var dir = new DirectoryInfo(@path);

                FileInfo[] files = dir.GetFiles();
                if (!String.IsNullOrEmpty(searchpatternwa))
                    files = dir.GetFiles(searchpatternwa);



                foreach (System.IO.FileInfo fileinfo in files)
                {
                    serverfiles.Add(new LocalFileStruc()
                    {
                        //Selected = false,
                        filename = fileinfo.Name,
                        filesize = fileinfo.Length,
                        filetype = fileinfo.Extension,
                        //fileicon = "file.png",
                        fileicon = fileinfo.Name,// makethumb(fileinfo),
                        //winfiletype = GetFileType(fileinfo.FullName),
                        //fullpath = fileinfo.FullName.Replace(FileSharePath,FileShare),
                        fullpath = "CHKSUM*FILE*" + fileinfo.FullName.Replace(FileSharePath, FileShare + "*"),
                        creationtime = fileinfo.CreationTimeUtc,
                        lastwritetime = fileinfo.LastWriteTimeUtc,
                        //attributes = "+R+E+D"
                    });
                }
                // now
                RimkusHelper.WriteLocalLog(userwa, "Folder Accessed " + @path);
                return JsonConvert.SerializeObject(serverfiles);
            }
            catch (Exception eee)
            {
                var bbb = eee.Message;
                return String.Empty;
            }
        }


        public string dbmobiledirsearch(string Id)
        {
            if (String.IsNullOrEmpty(searchpatternwa))
            {
                return "No search";
            }

            if (searchpatternwa == "ENCODED")
            {
                string datareturn1 = dbgetallfiles(RimkusHelper.Base64Decode(Id));
                return datareturn1;
            }
            else
            {
                string datareturn = dbgetallfiles("SELECT * FROM AbpDocuments WHERE DocumentName LIKE '%" + Id.Trim() + "%' FOR JSON AUTO");
                return datareturn;

            }
            //string datareturn =dbgetallfiles("SELECT * FROM AbpDocuments WHERE DocumentName LIKE '%"+Id.Trim()+"%' FOR JSON AUTO");
            //return datareturn;

            string foldername;
            string assignmgr;
            try
            {
                RCGRemoteFile bola = RimkusHelper.JustName(Id);

                if (bola.FileShare == "") return "No active token";
                FileShare = bola.FileShare;
                RootPath = bola.RootPath;
                // //userwa = bola.tokencheck;

                // error if path is not found on allshare look
                var path = pretrail();
                var myfolders = new List<string>();
                var serverfiles = new List<LocalFileStruc>() { };

                var directories = Directory.EnumerateFiles(@path, "*" + searchpatternwa + "*", SearchOption.AllDirectories);


                foreach (var file in directories)
                {
                    var fileinfo = new FileInfo(file.ToString());
                    //var fileinfo = System.IO.FileInfo(file);
                    serverfiles.Add(new LocalFileStruc()
                    {
                        //Selected = false,
                        filename = fileinfo.Name,
                        filesize = fileinfo.Length,
                        filetype = fileinfo.Extension,
                        //fileicon = "file.png",
                        fileicon = fileinfo.Name,// makethumb(fileinfo),
                        //winfiletype = GetFileType(fileinfo.FullName),
                        //fullpath = fileinfo.FullName.Replace(FileSharePath,FileShare),
                        fullpath = "CHKSUM*FILE*" + fileinfo.FullName.Replace(FileSharePath, FileShare + "*"),
                        creationtime = fileinfo.CreationTimeUtc,
                        lastwritetime = fileinfo.LastWriteTimeUtc,
                        //attributes = "+R+E+D"
                    });
                }
                // now
                RimkusHelper.WriteLocalLog(userwa, "Searched for " + @searchpatternwa);
                return JsonConvert.SerializeObject(serverfiles);
            }
            catch (Exception eee)
            {
                var bbb = eee.Message;
                return String.Empty;
            }
        }





        public string mobiledirsearch(string Id)
        {
            if (String.IsNullOrEmpty(searchpatternwa))
            {
                return "No search";
            }
            string foldername;
            string assignmgr;
            try
            {
                RCGRemoteFile bola = RimkusHelper.JustName(Id);

                if (bola.FileShare == "") return "No active token";
                FileShare = bola.FileShare;
                RootPath = bola.RootPath;
                // //userwa = bola.tokencheck;

                // error if path is not found on allshare look
                var path = pretrail();
                var myfolders = new List<string>();
                var serverfiles = new List<LocalFileStruc>() { };

                var directories = Directory.EnumerateFiles(@path, "*" + searchpatternwa + "*", SearchOption.AllDirectories);


                foreach (var file in directories)
                {
                    var fileinfo = new FileInfo(file.ToString());
                    //var fileinfo = System.IO.FileInfo(file);
                    serverfiles.Add(new LocalFileStruc()
                    {
                        //Selected = false,
                        filename = fileinfo.Name,
                        filesize = fileinfo.Length,
                        filetype = fileinfo.Extension,
                        //fileicon = "file.png",
                        fileicon = fileinfo.Name,// makethumb(fileinfo),
                        //winfiletype = GetFileType(fileinfo.FullName),
                        //fullpath = fileinfo.FullName.Replace(FileSharePath,FileShare),
                        fullpath = "CHKSUM*FILE*" + fileinfo.FullName.Replace(FileSharePath, FileShare + "*"),
                        creationtime = fileinfo.CreationTimeUtc,
                        lastwritetime = fileinfo.LastWriteTimeUtc,
                        //attributes = "+R+E+D"
                    });
                }
                // now
                RimkusHelper.WriteLocalLog(userwa, "Searched for " + @searchpatternwa);
                return JsonConvert.SerializeObject(serverfiles);
            }
            catch (Exception eee)
            {
                var bbb = eee.Message;
                return String.Empty;
            }
        }
        public string dbgetdirpath(string Id)
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
        public string dbgetfilepath(string Id)
        {
  var jsonResult = new StringBuilder();
            using (SqlConnection conn2 = new SqlConnection())
            {
                
          
            //SqlConnection conn2 = new SqlConnection();
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
        }


            return jsonResult.ToString();
        }

        public static string getToken(string Id)
        {
            var Authenticity = System.Web.HttpContext.Current.Request.Headers["Authorization"]; // string.Empty;
            var mysecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw";
            var bola = false;
            //var tokenrecddata = new TokenData();
            try
            {
                var secret = Encoding.ASCII.GetBytes(mysecret);//  new Buffer(mysecret, "base64").ToString();
                var claims = JWT.JsonWebToken.Decode(Authenticity, mysecret, bola);// false);
                var tokenrecddata = JsonConvert.DeserializeObject<TokenData>(claims);
                return claims;
            }
            catch
            {
                return String.Empty;
            }
        }


        //http://cr.rimkus.com/creport/JustJob/76400021
        private static async Task<string> GetAnyFromServer(string id)
        {
            //Todo Remove this hard coding and use soem other means
            string page = id;// + "/" + data;
            using (System.Net.Http.HttpClient client = new HttpClient())
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

        public string GetJobFolderInformation(string Id, string data)
        {
            if (Id == null) return "";
            if (data == null) return "";

            data = RimkusHelper.GetBetween(data, "[", "]");
            if (Id == "Job") return "";
            string[] myinfo =new string[6];
            myinfo[0]= Id; // GetAnyFromServer("Https://cr.rimkus.com/creport/JustJob/" + Id.Trim()).Result ;
           
            string bola = RimkusHelper.dbGetJson("Select * from AbpFolders where Id ='" +data + "' FOR JSON AUTO");
            if (bola.Length > 30)
            {
                List<AbpFolders> folderstemp = JsonConvert.DeserializeObject<List<AbpFolders>>(bola);
                myinfo[1] = folderstemp[0].FolderSubFolderCount.ToString();
                myinfo[2] = folderstemp[0].FolderFileCount.ToString();
                myinfo[3] = folderstemp[0].FolderCreatedDate.ToString();
                myinfo[4] = folderstemp[0].FolderUpdated.ToString();
                myinfo[5] = folderstemp[0].FolderOffice;
            }
            return JsonConvert.SerializeObject(myinfo); 
        }
        public string dbgetallfiles(string Id, string data = "")
        {
            var jsonResult = new StringBuilder();

            using (SqlConnection conn2 = new SqlConnection())
            {
            //SqlConnection conn2 = new SqlConnection();
            conn2.ConnectionString = "Data Source=RCGFTD01.rimkus.net;Initial Catalog=JobStructure;User ID=sa;Password=Star@1234";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn2;
            conn2.Open();
            if (Id.Substring(0, 6) != "SELECT")
                Id = "SELECT * FROM  AbpDocuments " + Id;

            cmd.CommandText = Id; // "select DocumentName , Id from AbpDocuments where FolderId='" + Id + "' FOR JSON AUTO";//+ data != "" ? " AND " + data : string.Empty+ "FOR JSON AUTO" ;
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
            };
            conn2.Close();
            }
               
            return jsonResult.ToString();
        }




        public string dbpretrail(long Id)
        {
            var jsonResult = new StringBuilder();
            SqlConnection conn2 = new SqlConnection();
            conn2.ConnectionString = "Data Source=RCGFTD01.rimkus.net;Initial Catalog=JobStructure;User ID=sa;Password=Star@1234";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn2;
            conn2.Open();
            cmd.CommandText = "SELECT * FROM AbpFolders WHERE ParentFolderId =" + Id.ToString() + " FOR JSON AUTO";
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
            return jsonResult.ToString();
        }
        public string dbmobileuserdironly(string Id)
        {
            // check if foder exists
            var Authenticity = System.Web.HttpContext.Current.Request.Headers["Authorization"]; // string.Empty;
            var mysecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw";
            var bola = false;
            //var tokenrecddata = new TokenData();
           
                var secret = Encoding.ASCII.GetBytes(mysecret);//  new Buffer(mysecret, "base64").ToString();
                var claims = JWT.JsonWebToken.Decode(Authenticity, mysecret, bola);// false);
                var tokenrecddata = JsonConvert.DeserializeObject<TokenData>(claims);
                //return claims;
           
            //var inString = RimkusHelper.Base64Decode(Id);
            var tester = RimkusHelper.dbScalar("Select Id from AbpFolders where FolderName='"+tokenrecddata.HomeDirectory+"'");
            if (tester.Trim()=="")
            {
                var botestar = RimkusHelper.dbScalar("INSERT INTO AbpFolders  ( Foldername,FolderRoot,FolderRank,FolderOffice)  OUTPUT Inserted.Id  VALUES('" + tokenrecddata.HomeDirectory+"','"+tokenrecddata.HomeDirectory+"',"+"0"+",'"+"Personal Folder')");
                //var botestar = RimkusHelper.dbScalar("Select Id from AbpFolders where FolderName='" + tokenrecddata.HomeDirectory + "'");

                return botestar;
                // make a new entry
            }


            return tester;
        }

        public string dbmobiledironly(string Id)
        {
            long folderid;
            var inString = RimkusHelper.Base64Decode(Id);
            //string foldername;
            var sfolderid = inString.Substring(inString.IndexOf("^") + 2);

            var bolahat = inString.Split('^');

            //var path = dbgetdirpath(bolahat[1]) + bolahat[0];//  pretrail();
            bolahat[0] = bolahat[0].Substring(bolahat[0].LastIndexOf('*') + 1);
           

            CheckBuildFolder(bolahat);
           



            //string assignmgr;
            try
            {
                folderid = Convert.ToInt64(sfolderid.Substring(0, sfolderid.IndexOf("]")));
                var dbpath = dbpretrail(folderid);
                var mFolders = JsonConvert.DeserializeObject<List<AbpFolders>>(dbpath).OrderBy(x=>x.FolderName);
                return JsonConvert.SerializeObject(mFolders);
            }
            catch (Exception eee)
            {
                var bbb = eee.Message;
                return String.Empty;
            }
        }
        public string mobiledironly(string Id)
        {
            string foldername;
            string assignmgr;
            try
            {
                RCGRemoteFile bola = RimkusHelper.JustName(Id);

                if (bola.FileShare == "") return "No active token";
                FileShare = bola.FileShare;
                RootPath = bola.RootPath;
                // //userwa = bola.tokencheck;

                // error if path is not found on allshare look
                var path = pretrail();

                if (!IsRootPath)
                {
                    if (AccessType != "Manager")
                    {
                        // get the folder name
                        foldername = RootPath.Substring(RootPath.LastIndexOf("\\") + 1);
                        //string folderbola = RimkusHelper.CanAccess(foldername);
                        var folderbola = userwa.ToUpper();
                        if (RootPath.Count(c => c == '\\') == 1)
                        {
                            folderbola = RimkusHelper.CanAccess(foldername);
                        }
                        if (!folderbola.ToUpper().Contains(userwa.ToUpper()))
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
                            justapilog("Access denied to folder " + @foldername);
                            return JsonConvert.SerializeObject(serverblankfiles);
                            //return String.Empty;
                        }
                    }
                }


                var myfolders = new List<string>();
                var serverfiles = new List<LocalFileStruc>() { };
                var directories = System.IO.Directory.EnumerateDirectories(@path);

                return JsonConvert.SerializeObject(directories);

                /*
                if (!String.IsNullOrEmpty(searchpatternwa))
                {
                   if (System.IO.Directory.EnumerateDirectories(@path,"*"+searchpatternwa+"*", SearchOption.AllDirectories).Count() > 0 )
                    {
                        directories = System.IO.Directory.EnumerateDirectories(@path,"*"+ searchpatternwa+"*", SearchOption.AllDirectories);
                        searchpatternwa = "";
                    }
                }
                  */

                //RimkusHelper.WriteLocalLog(//userwa, "Folder:"+@path);
                justapilog("Access approved to folder " + @path);

                // try this approach
                //directories = directories.Where(x => x == "sd");

                //if (!String.IsNullOrEmpty(searchpatternwa))
                //{
                //    directories = directories.Where(x => x.Contains(searchpatternwa));
                //}

                foreach (var directory in directories)
                {
                    var dirX = new DirectoryInfo(@directory);
                    serverfiles.Add(new LocalFileStruc()
                    {
                        //Selected = false,
                        filename = Path.GetFileName(directory),
                        filesize = 0,
                        filetype = "DIR",
                        fileicon = "directory.png",
                        creationtime = dirX.CreationTimeUtc,
                        lastwritetime = dirX.LastWriteTimeUtc,
                        //fullpath = directory.Replace(FileSharePath, FileShare),
                        fullpath = "CHKSUM*DIR*" + directory.Replace(FileSharePath, FileShare + "*"),
                        attributes = "+R+E+D"
                    });
                }

                if (IsRootPath) return JsonConvert.SerializeObject(serverfiles);



                var dir = new DirectoryInfo(@path);

                FileInfo[] files = dir.GetFiles();
                if (!String.IsNullOrEmpty(searchpatternwa))
                    files = dir.GetFiles(searchpatternwa);



                foreach (System.IO.FileInfo fileinfo in files)
                {
                    serverfiles.Add(new LocalFileStruc()
                    {
                        //Selected = false,
                        filename = fileinfo.Name,
                        filesize = fileinfo.Length,
                        filetype = fileinfo.Extension,
                        //fileicon = "file.png",
                        fileicon = fileinfo.Name,// makethumb(fileinfo),
                        //winfiletype = GetFileType(fileinfo.FullName),
                        //fullpath = fileinfo.FullName.Replace(FileSharePath,FileShare),
                        fullpath = "CHKSUM*FILE*" + fileinfo.FullName.Replace(FileSharePath, FileShare + "*"),
                        creationtime = fileinfo.CreationTimeUtc,
                        lastwritetime = fileinfo.LastWriteTimeUtc,
                        //attributes = "+R+E+D"
                    });
                }
                // now
                RimkusHelper.WriteLocalLog(userwa, "Folder Accessed " + @path);
                return JsonConvert.SerializeObject(serverfiles);
            }
            catch (Exception eee)
            {
                var bbb = eee.Message;
                return String.Empty;
            }
        }
        // GetManagerJob

        public async Task<string> GetJobInfo(string Id)
        {
            var testo = await RimkusHelper.GetJobInfo(Id);
            return testo;
        }
        public async Task<string> GetManagerJob(string Id)
        {
            var testo = await RimkusHelper.GetManagerJob(Id);
            return testo;
        }
        public string dbmobiledir(string Id)
        {
            string foldername = "";
            string assignmgr;
            string jobinfo = "";
            try
            {
                RCGRemoteFile bola = RimkusHelper.JustName(Id);

                if (bola.FileShare == "") return "No active token";

                var bolahat = bola.RootPath.Split('^');

                FileShare = bola.FileShare;
                RootPath = bolahat[0];// bola.RootPath;
                                      // //userwa = bola.tokencheck;
                var FolderID = RimkusHelper.GetBetween(bolahat[1], "[", "]");
                // 

                //var path = dbgetdirpath(bolahat[1]) + bolahat[0];//  pretrail();

                CheckBuildFiles(bolahat); 

                // error if path is not found on allshare look
                //var path = "fdfd";// pretrail();
                // get th ejob number
                try
                {
                    if (!RootPath.Contains("\\")) { foldername = RootPath; }
                    else
                    {
                        foldername = RootPath.Substring(0, RootPath.IndexOf("\\"));
                        foldername = foldername.Substring(0, foldername.IndexOf("\\"));
                    }
                }
                catch (Exception)
                {

                    // throw;
                }
                finally
                {
                    //  if (!String.IsNullOrEmpty(foldername))
                    /// { jobinfo = RimkusHelper.GetJobInfo(foldername); }
                }
                var folderbola = userwa.ToUpper();

                if (RootPath.Count(c => c == '\\') == 0)
                {
                    folderbola = RimkusHelper.CanAccess(foldername);


                    if (!folderbola.ToUpper().Contains(userwa.ToUpper()))
                    {
                        var serverblankfiles = new List<AbpDocuments>();
                        serverblankfiles.Add(new AbpDocuments()
                        {
                            //Selected = false,
                            DocumentName = jobinfo,
                            DocumentType = "DIRR",
                        });
                        serverblankfiles.Add(new AbpDocuments()
                        {
                            DocumentName = "Access Denied",
                            DocumentType = "DIR",
                            DocumentIcon = "directory.png",
                            DocumentJob = foldername,
                            FolderID = 0,
                            DocumentMetaData = "CHKSUM*DIR*ACCESSDENIED",
                        });
                        justapilog("Access denied to folder " + @foldername);
                        return JsonConvert.SerializeObject(serverblankfiles);
                        //return String.Empty;
                    }
                }

              

                // now get all the data here

                var jsonResult = new StringBuilder();
                SqlConnection conn2 = new SqlConnection();
                conn2.ConnectionString = "Data Source=RCGFTD01.rimkus.net;Initial Catalog=JobStructure;User ID=sa;Password=Star@1234";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn2;
                conn2.Open();
                cmd.CommandText = "SELECT * FROM AbpDocuments WHERE FolderID =" + FolderID.ToString() + " FOR JSON AUTO";
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
                };
                return jsonResult.ToString();
            }
            catch (Exception eee)
            {
                var bbb = eee.Message;
                return String.Empty;
            }
        }

        public void CheckBuildFolder(String[] Id ) //, bool isroot=false)
        {
            bool isroot =false;
            if (!Id[0].Contains("\\")) isroot = true;

            try
            {
                var FolderID = RimkusHelper.GetBetween(Id[1], "[", "]");
                var path = "";
                if (isroot)
                {
                     path = RimkusHelper.dbScalar("Select FolderName FROm AbpFolders Where Id=" + FolderID);
                }
                else
                {
                     path = dbgetdirpath(FolderID) + Id[0].Substring(Id[0].LastIndexOf("\\")+1) ;//  pretrail();
                }
               
              
                // get all the folder below this
                //ar parentfolder = RimkusHelper.dbScalar("Select ParentFolderID From AbpFolders WHERE Id=" + FolderID);

                var folders = System.IO.Directory.GetDirectories(@path, "*", System.IO.SearchOption.TopDirectoryOnly);//.Select(x => x.Remove(-1, x.LastIndexOf("\\")) + 1);
                var folderdrive = folders.Select(s => s.Substring(s.LastIndexOf("\\") + 1));
                // now get the foder from db
                var bola = RimkusHelper.dbGetJson("Select FolderName from AbpFolders where ParentFolderID ='" + FolderID + "' FOR JSON AUTO");
                var folderstemp = JsonConvert.DeserializeObject<List<FolderData>>(bola);
                var foldersdb = folderstemp.Select(x => x.FolderName);
                var intersect = folderdrive.Intersect(foldersdb);
                var foldersnew = folderdrive.Except(intersect);
                var folderdelete = foldersdb.Except(intersect);
                foreach (var item in foldersnew)
                {


                    //create a folder
                    //fill up all the fies in hthis folder
                    RimkusHelper.dbnonExecute("EXEC dbo.sp_createnewfolder " + FolderID + ",'" + item.Replace("'", "''").Replace("\a", "").Replace("\u0015", "").Replace("\"", "").Trim() + "'");
                }
                foreach (var item in folderdelete)
                {
                    //delete this folder and subydrectories
                    RimkusHelper.dbnonExecute("EXEC dbo.DeleteFTBFolders '" + item.Replace("'", "''").Replace("\a", "").Replace("\u0015", "").Replace("\"", "").Trim() + "'," + FolderID);
                }
                // now lets compare them
            }
            catch (Exception eee)
            {

                //throw;
            }
        }

        public void CheckBuildFiles(String[] Id ) //, bool isroot = false)
        {
            bool isroot =false;
            if (!Id[0].Contains("\\")) isroot = true;
            try
            {
                var FolderID = RimkusHelper.GetBetween(Id[1], "[", "]");
                var path = "";
               // if (isroot)
               // {
                //    path = RimkusHelper.dbScalar("Select FolderName FROm AbpFolders Where Id=" + FolderID);
               // }
               // else
               // {
                    path = dbgetdirpath(FolderID) + Id[0].Substring(Id[0].LastIndexOf("\\") + 1);//  pretrail();
                //}


                // get all the folder below this
                //ar parentfolder = RimkusHelper.dbScalar("Select ParentFolderID From AbpFolders WHERE Id=" + FolderID);

                var files = System.IO.Directory.GetFiles(@path, "*", System.IO.SearchOption.TopDirectoryOnly);//.Select(x => x.Remove(-1, x.LastIndexOf("\\")) + 1);
                var filename = files.Select(s => s.Substring(s.LastIndexOf("\\") + 1));
                // now get the foder from db

                var folderbola = RimkusHelper.dbGetJson("Select TOP 1 * from AbpFolders where Id ='" + FolderID + "' FOR JSON AUTO");
                var folderstemp = JsonConvert.DeserializeObject<List<AbpFolders>>(folderbola);
    

                // and the fiel information
                var bola = RimkusHelper.dbGetJson("Select DocumentName from AbpDocuments where FolderID ='" + FolderID + "' FOR JSON AUTO");
                var filetemp = JsonConvert.DeserializeObject<List<FileData>>(bola);
                var filedb = filetemp.Select(x => x.DocumentName);
                var intersect = filename.Intersect(filedb);
                var filesnew = filename.Except(intersect);
                var filesdelete = filedb.Except(intersect);
                foreach (var item in filesnew)
                {
                    //  INSERT into
                    // get fileinfo
                    FileInfo myfileinfo = new FileInfo(path + "//" + item);

                    //here rname the file n proper 
                   RimkusHelper.dbnonExecute("INSERT INTO AbpDocuments(FolderId ,DocumentName,DocumentSize,DocumentJob,DocumentDate,DocumentAccess,DocumentOffice,DocumentType)   VALUES("+FolderID+",'"+item.Replace("'","''").Replace("\a", "").Replace("\u0015", "").Replace("\"", "").Trim() + "',"+myfileinfo.Length.ToString()+",'"+folderstemp[0].FolderJob+"','"+myfileinfo.CreationTime.ToString()+"','"+"+R+W+D"+"','"+folderstemp[0].FolderOffice+"','"+myfileinfo.Extension+"')") ;

                    //create a folder
                    //fill up all the fies in hthis folder
                    //RimkusHelper.dbnonExecute("EXEC dbo.sp_createnewfolder " + FolderID + ",'" + item + "'");
                }
                foreach (var item in filesdelete)
                {
                    RimkusHelper.dbnonExecute("DELETE FROM AbpDocuments WHERE DocumentName='"+item.Replace("'", "''").Replace("\a", "").Replace("\u0015", "").Replace("\"", "").Trim() + "' AND FolderID="+FolderID);


                    //delete this folder and subydrectories
                    //RimkusHelper.dbnonExecute("EXEC dbo.sp_DeleteFTBFolders '" + item + "'," + FolderID);
                }
                // now lets compare them
            }
            catch (Exception eee)
            {

                //throw;
            }
        }



        public ActionResult NoViewer(string Id)
        {

            RCGRemoteFile bola = RimkusHelper.JustName(Id);
            if (bola.FileShare == "") return Content("File cannot be read");
            FileShare = bola.FileShare;
            RootPath = "";
            var path = pretrail();
            string xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                                                 // string xfilex = @"c:\testshareroot\Manual.docx";
            if (!System.IO.File.Exists(xfilex)) return Content("File access denied");

            ViewBag.rimkus = Id;
            ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + Path.GetFileName(@xfilex).Replace(".", "DDOOTT");

            return View();
        }

        public async Task<string> createconnections()
        {
            int randomNumber = 0;
            try
            {
                //var tempo = RimkusHelper.RimTerm(randomNumber.ToString());
                Random random = new Random();

                var testUser = new Application.RimSQL();
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
                var temptoken = "JUSTTOKEN";
                temptoken = temptoken.Replace("'", "''");
                testUser.ExecuteNonQuery("INSERT INTO PortTrack (UserName, PortNumber , TokenKey, RecdAt) VALUES ('" + "rimuser" + "','" + randomNumber.ToString() + "','" + temptoken + "','" + DateTime.UtcNow.ToString() + "')");
                System.Threading.Thread.Sleep((int)System.TimeSpan.FromSeconds(1).TotalMilliseconds);

                var opennewport = await RimkusHelper.RimTerm(randomNumber.ToString(), temptoken, "rimuser", "Houston");

                //                if (RimkusHelper.RimTerm(randomNumber.ToString(), temptoken, tokendata.User, tokendata.Company) == "OK")

                if (opennewport == "OK")
                {
                    System.Threading.Thread.Sleep((int)System.TimeSpan.FromSeconds(1).TotalMilliseconds);
                    return "220 Service ready for user rimuser " + " on Port " + randomNumber;
                }
                else
                {
                    return "500 Error in opening port for user " + "rimuser" + " on Port " + randomNumber;
                }

                //return Content("220 Service ready for user " + tokendata.User + " on Port " + randomNumber);
            }
            catch (Exception rrr)
            {
                return "500 Port Error  " + rrr.Message;
            }
            //Todo: verify user now
        }


        public string dbfiledownload(string Id)
        {
            try
            {
                //RCGRemoteFile bola = RimkusHelper.JustName(Id);
                // if (bola.FileShare == "") return "File uploaded error";
                //FileShare = bola.FileShare;
                // RootPath = "";// bola.RootPath;// .FileShare;// ""; // bola.RootPath;
                // error if path is not found on allshare look
                //var path = pretrail();
                // remmemebr rootpath contains the file name compltet
                //var path = pretrail();
                //string xfilex = Path.Combine(path, Parameter1);


                if (Id == "NOHAVEGETVALUE")
                {
                    var myfolder = RimkusHelper.GetBetween(System.Web.HttpContext.Current.Request.Headers["RootPath"], "[", "]");
                    var myfilenamer = System.Web.HttpContext.Current.Request.Headers["Parameter1"].ToString();
                    myfilenamer = WebUtility.HtmlDecode(myfilenamer);
                    Id = RimkusHelper.dbGetScalarCommand("SELECT Id from AbpDocuments WHERE FolderID=" + myfolder + " AND DocumentName ='" + myfilenamer + "'");
                    //Id = tola.Id;
                }


                //if(bola.RootPath.Substring(0,1)="\\")
                string xfilex = dbgetfilepath(Id);// path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                                                  //if (!System.IO.File.Exists(xfilex))
                                                  //   xfilex = @Path.Combine(path, bola.RootPath);

                // see if the filename is in the rootpath
                // if (!System.IO.File.Exists(xfilex))
                //    xfilex = @Path.Combine(path, bola.RootPath.Substring(0, bola.RootPath.LastIndexOf("\\")));

                xfilex = xfilex.Replace("\\\\", "\\");

                if (xfilex.Substring(0, 1) == "\\")
                {
                    xfilex = "\\" + xfilex;
                }

                if (!System.IO.File.Exists(xfilex))

                {
                    RimkusHelper.dbCommandExecute("DELETE FROM AbpDocuments WHERE ID=" + Id);
                    return "Could not find a part of the path";
                }
                // return "Could not find a part of the path";


                FileInfo tempfileupload = new FileInfo(@xfilex);

                if (tempfileupload.Length > 200000)
                {
                    if (tempfileupload.Extension.ToLower() == ".pdf" || tempfileupload.Extension.ToLower() == ".xlsx" || tempfileupload.Extension.ToLower() == ".xls" || tempfileupload.Extension.ToLower() == ".docx" || tempfileupload.Extension.ToLower() == ".doc" || tempfileupload.Extension.ToLower() == ".pptx" || tempfileupload.Extension.ToLower() == ".ppt")
                    {
                        string comfile = fmcompress.compMain(@xfilex);
                        if (comfile != "")
                        {
                            xfilex = comfile;
                        }
                    }

                    if (tempfileupload.Extension.ToLower() == ".jpg" || tempfileupload.Extension.ToLower() == ".png" || tempfileupload.Extension.ToLower() == ".gif" || tempfileupload.Extension.ToLower() == ".bmp" || tempfileupload.Extension.ToLower() == ".jpeg")
                    {
                        bool iscompressed = false;
                        try
                        {
                            // is it already compressed
                            var shellFile = ShellFile.FromFilePath(@xfilex);
                            var tags = (string[])shellFile.Properties.System.Keywords.ValueAsObject;
                            tags = tags ?? new string[0];
                            if (tags.Length != 0)
                            {
                                foreach (string str in tags)
                                {
                                    if (str.Contains("ftbytescompressed"))
                                    {
                                        iscompressed = true; break;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            //throw;
                        }

                        if (!iscompressed)
                        {
                            System.Drawing.Image i = System.Drawing.Image.FromFile(@xfilex);// @"C:\Users\jmredeni\Desktop\Image Compression Test Pictures\IMG_9622.JPG");
                            ImageProcessor.Imaging.Quantizers.WuQuantizer.WuQuantizer q = new ImageProcessor.Imaging.Quantizers.WuQuantizer.WuQuantizer();
                            //System.Drawing.Image i2 = q.Quantize(i, 0, 1, null, 255);
                            System.Drawing.Image i2 = q.Quantize(i, 0, 1, null, 256);

                            using (MemoryStream ms = new MemoryStream())
                            {
                                switch (tempfileupload.Extension.ToLower())
                                {
                                    case ".jpg":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        break;
                                    case ".png":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                        break;
                                    case ".gif":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                                        break;
                                    case ".bmp":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                                        break;
                                    case ".jpeg":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        break;
                                }

                                //string ccontentType = MimeMapping.GetMimeMapping(fullpath);
                                //return File(ms.ToArray(), ccontentType); // mimeType);
                                string ttoEncode = Convert.ToBase64String(ms.ToArray());
                                //byte[] temp_backToBytes = Encoding.UTF8.GetBytes(temp_inBase64);
                                //var toEncode = Encoding.ASCII.GetString(fileinbytes);
                                return ttoEncode;
                            }
                        }

                    }

                }


                if (!System.IO.File.Exists(@xfilex))
                {
                    return String.Empty;
                    //return "Error";
                }

                // string comfile = fmcompress.compMain(xfilex);
                // if (comfile != "")
                // {
                //     xfilex = comfile;
                // }
                //string contentType = MimeMapping.GetMimeMapping(fullpath);
                // contentType = "application/pdf"
                //byte[] fileBytes = System.IO.File.ReadAllBytes(@fullpath);
                //return File(fileBytes, contentType); // mimeType);

                byte[] fileBytes = System.IO.File.ReadAllBytes(@xfilex);
                //string fileName = Parameter1;
                //return fileBytes; // File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                string toEncode = Convert.ToBase64String(fileBytes);

                justapilog("File downloaded " + @xfilex, 0, fileBytes.Length);

                //byte[] temp_backToBytes = Encoding.UTF8.GetBytes(temp_inBase64);
                //var toEncode = Encoding.ASCII.GetString(fileinbytes);
                return toEncode;
            }
            catch (Exception eee)
            {

                return "Could not find a part of the path: Out of Memory";
                // throw;
            }
        }


        public string filedownload(string Id)
        {
            try
            {
                RCGRemoteFile bola = RimkusHelper.JustName(Id);
                if (bola.FileShare == "") return "File uploaded error";
                FileShare = bola.FileShare;
                RootPath = "";// bola.RootPath;// .FileShare;// ""; // bola.RootPath;
                              // error if path is not found on allshare look
                var path = pretrail();
                // remmemebr rootpath contains the file name compltet
                //var path = pretrail();
                //string xfilex = Path.Combine(path, Parameter1);

                //if(bola.RootPath.Substring(0,1)="\\")
                string xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
                if (!System.IO.File.Exists(xfilex))
                    xfilex = @Path.Combine(path, bola.RootPath);

                // see if the filename is in the rootpath
                if (!System.IO.File.Exists(xfilex))
                    xfilex = @Path.Combine(path, bola.RootPath.Substring(0, bola.RootPath.LastIndexOf("\\")));



                if (!System.IO.File.Exists(xfilex)) return "Error";


                FileInfo tempfileupload = new FileInfo(@xfilex);

                if (tempfileupload.Length > 200000)
                {
                    if (tempfileupload.Extension.ToLower() == ".pdf" || tempfileupload.Extension.ToLower() == ".xlsx" || tempfileupload.Extension.ToLower() == ".xls" || tempfileupload.Extension.ToLower() == ".docx" || tempfileupload.Extension.ToLower() == ".doc" || tempfileupload.Extension.ToLower() == ".pptx" || tempfileupload.Extension.ToLower() == ".ppt")
                    {
                        string comfile = fmcompress.compMain(@xfilex);
                        if (comfile != "")
                        {
                            xfilex = comfile;
                        }
                    }

                    if (tempfileupload.Extension.ToLower() == ".jpg" || tempfileupload.Extension.ToLower() == ".png" || tempfileupload.Extension.ToLower() == ".gif" || tempfileupload.Extension.ToLower() == ".bmp" || tempfileupload.Extension.ToLower() == ".jpeg")
                    {
                        bool iscompressed = false;
                        try
                        {
                            // is it already compressed
                            var shellFile = ShellFile.FromFilePath(@xfilex);
                            var tags = (string[])shellFile.Properties.System.Keywords.ValueAsObject;
                            tags = tags ?? new string[0];
                            if (tags.Length != 0)
                            {
                                foreach (string str in tags)
                                {
                                    if (str.Contains("ftbytescompressed"))
                                    {
                                        iscompressed = true; break;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            //throw;
                        }

                        if (!iscompressed)
                        {
                            System.Drawing.Image i = System.Drawing.Image.FromFile(@xfilex);// @"C:\Users\jmredeni\Desktop\Image Compression Test Pictures\IMG_9622.JPG");
                            ImageProcessor.Imaging.Quantizers.WuQuantizer.WuQuantizer q = new ImageProcessor.Imaging.Quantizers.WuQuantizer.WuQuantizer();
                            //System.Drawing.Image i2 = q.Quantize(i, 0, 1, null, 255);
                            System.Drawing.Image i2 = q.Quantize(i, 0, 1, null, 256);

                            using (MemoryStream ms = new MemoryStream())
                            {
                                switch (tempfileupload.Extension.ToLower())
                                {
                                    case ".jpg":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        break;
                                    case ".png":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                        break;
                                    case ".gif":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                                        break;
                                    case ".bmp":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                                        break;
                                    case ".jpeg":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        break;
                                }

                                //string ccontentType = MimeMapping.GetMimeMapping(fullpath);
                                //return File(ms.ToArray(), ccontentType); // mimeType);
                                string ttoEncode = Convert.ToBase64String(ms.ToArray());
                                //byte[] temp_backToBytes = Encoding.UTF8.GetBytes(temp_inBase64);
                                //var toEncode = Encoding.ASCII.GetString(fileinbytes);
                                return ttoEncode;
                            }
                        }

                    }

                }


                if (!System.IO.File.Exists(@xfilex))
                {
                    return String.Empty;
                    //return "Error";
                }

                // string comfile = fmcompress.compMain(xfilex);
                // if (comfile != "")
                // {
                //     xfilex = comfile;
                // }
                //string contentType = MimeMapping.GetMimeMapping(fullpath);
                // contentType = "application/pdf"
                //byte[] fileBytes = System.IO.File.ReadAllBytes(@fullpath);
                //return File(fileBytes, contentType); // mimeType);

                byte[] fileBytes = System.IO.File.ReadAllBytes(@xfilex);
                //string fileName = Parameter1;
                //return fileBytes; // File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                string toEncode = Convert.ToBase64String(fileBytes);

                justapilog("File downloaded " + @xfilex, 0, fileBytes.Length);

                //byte[] temp_backToBytes = Encoding.UTF8.GetBytes(temp_inBase64);
                //var toEncode = Encoding.ASCII.GetString(fileinbytes);
                return toEncode;
            }
            catch (Exception eee)
            {

                return "Could not find a part of the path: Out of Memory";
                // throw;
            }
        }

        public FileContentResult filestreamdownload(string Id)
        {
            RCGRemoteFile bola = RimkusHelper.JustName(Id);
            if (bola.FileShare == "") return new FileContentResult(new byte[] { }, "msg");
            FileShare = bola.FileShare;
            RootPath = ""; // bola.RootPath;
            // error if path is not found on allshare look
            var path = pretrail();
            // remmemebr rootpath contains the file name compltet
            //var path = pretrail();
            //string xfilex = Path.Combine(path, Parameter1);

            //if(bola.RootPath.Substring(0,1)="\\")
            string xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
            if (!System.IO.File.Exists(xfilex))
                xfilex = @Path.Combine(path, bola.RootPath);

            byte[] fileBytes = System.IO.File.ReadAllBytes(@xfilex);
            return File(fileBytes, "application/msg"); // mimeType);
            //string fileName = Parameter1;
            //return fileBytes; // File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //string toEncode = Convert.ToBase64String(fileBytes);
            //byte[] temp_backToBytes = Encoding.UTF8.GetBytes(temp_inBase64);
            //var toEncode = Encoding.ASCII.GetString(fileinbytes);
            //return toEncode;
        }
        public FileContentResult ViewAsPDF(string Id)
        {
            string filepath = "";
            if (TempData["TempFolder"] != null)
            { filepath = TempData["TempFolder"].ToString(); }

            if (filepath == "") return new FileContentResult(new byte[] { }, "PDF");

            byte[] fileBytes = System.IO.File.ReadAllBytes(@filepath);// "C:\rimkus\test.pdf");// System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Uploads/records.csv"));
            FileInfo fileinfo = new FileInfo(filepath);
            string fileName = fileinfo.Name; // filepath.Substring(filepath.LastIndexOf("//"));// "testpdf.pdf";// CsvName;

            if (filepath.Contains(@Path.GetTempPath())) System.IO.File.Delete(filepath);
            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            string mimeType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);
            return File(fileBytes, mimeType);
            //eturn View();
        }

        public void justapilog(string Msg, Int64 rx = 0, Int64 tx = 0)
        {
            if (userwa == "zfaruqi") return;
            Msg = Msg.Replace("//", "-").Replace("'", " ").Replace("\\", "-");
            LogData logdata = new LogData()
            {

                UserName = userwa,
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

        public ActionResult viewPDF()
        {
            string filepath = "";
            if (TempData["TempFolder"] != null)
            { filepath = TempData["TempFolder"].ToString(); }
            TempData["TempFolder"] = filepath;
            FileInfo tempo = new FileInfo(filepath);
            var filer = tempo.Name;

            if (TempData["Excel"] != null)
                filer = TempData["Excel"].ToString();


            ViewBag.fileloc = "/dbrimkus/getdownicon/_" + "JpbUJpZ0ZpbGVz" + "_" + filer.Replace(".", "DDOOTT");
            var inString = RimkusHelper.Base64Decode(TempData["ID"].ToString());
            ViewBag.rimkus = RimkusHelper.Base64Encode(inString.Substring(0, inString.LastIndexOf("\\")));
            //ViewBag.rimkus = ViewBag.rimkus = RimkusHelper.Base64Encode(tempo.DirectoryName); ;
            // return View();


            Aspose.Pdf.Document document = new Aspose.Pdf.Document(filepath);
            justapilog("Viewed Files in " + @ViewBag.fileloc + " " + filer.Replace(".", "DDOOTT"));
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms); //.Bmp);
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(ms.ToArray());
                return Content("OK");
            }


        }
        /// <summary>
        /// here you will getthe file when you do a double click . make sure you know the file type before opening 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult getrimpic(string Id)
        {
            /*            RCGRemoteFile bola = RimkusHelper.JustName(Id);
                        if (bola.FileShare == "") return Content("File cannot be read");
                        FileShare = bola.FileShare;
                        //userwa = bola.tokencheck;
                        RootPath = "";
                        var path = pretrail();
                        // remmemebr rootpath contains the file name compltet
                        //string xfilex = Path.Combine(path, Parameter1);
                        string xfilex = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";*/
            string xfilex = dbgetfilepath(Id);
            if (!System.IO.File.Exists(xfilex)) return Content("File access denied");

            System.IO.FileInfo fileinfo = new System.IO.FileInfo(xfilex);

            if (fileinfo.Extension.ToLower() == ".jpg" || fileinfo.Extension.ToLower() == ".png" || fileinfo.Extension.ToLower() == ".gif" || fileinfo.Extension.ToLower() == ".bmp" || fileinfo.Extension.ToLower() == ".jpeg")
            {
                Bitmap myBitmap = new Bitmap(@xfilex);
                using (MemoryStream ms = new MemoryStream())
                {
                    myBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); //.Bmp);
                    Response.ContentType = "image/jpg";
                    Response.BinaryWrite(ms.ToArray());
                    return Content("OK");
                }
            }
            else
                if (fileinfo.Extension.ToLower() == ".pdf")
            {
                TempData["TempFolder"] = xfilex;
                TempData["ID"] = Id;
                return RedirectToAction("viewPDF");
            }





            return Content("OK");
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

        public string getthumb(string Id)
        {
            RCGRemoteFile bola = RimkusHelper.JustName(Id);
            if (bola.FileShare == "") return "File cannot be read";
            FileShare = bola.FileShare;
            RootPath = "";
            var path = pretrail();
            //userwa = bola.tokencheck;

            // remmemebr rootpath contains the file name compltet
            //string xfilex = Path.Combine(path, Parameter1);
            string xfilename = path + bola.RootPath;// @Path.Combine(path, bola.RootPath);// @"D:\RimKus\ftb2017\FTBAPISERVER\images\fullscreen\DSC02699.JPG";
            if (!System.IO.File.Exists(xfilename)) return "File access denied";
            FileInfo filename = new FileInfo(xfilename);
            if (filename.Extension.ToLower() == ".jpg" || filename.Extension.ToLower() == ".png" || filename.Extension.ToLower() == ".gif" || filename.Extension.ToLower() == ".bmp" || filename.Extension.ToLower() == ".jpeg")
            {
                try
                {
                    Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    Bitmap myBitmap = new Bitmap(@filename.FullName);// "c:\rimkus\testfile.jpg");
                    Image myThumbnail = myBitmap.GetThumbnailImage(140, 140, myCallBack, IntPtr.Zero);
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

        private string oldmakethumb(FileInfo filename)
        {
            if (filename.Extension.ToLower() == ".jpg" || filename.Extension.ToLower() == ".png" || filename.Extension.ToLower() == ".gif" || filename.Extension.ToLower() == ".bmp" || filename.Extension.ToLower() == ".jpeg")
            {
                try
                {
                    Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    Bitmap myBitmap = new Bitmap(@filename.FullName);// "c:\rimkus\testfile.jpg");
                    //todo: create icon from bitmap

                    Bitmap bm = new Bitmap(myBitmap, new Size(32, 32));
                    //notify.Icon = Icon.FromHandle(bm.GetHicon());

                    IntPtr Hicon = bm.GetHicon();
                    Icon newIcon = Icon.FromHandle(Hicon);

                    //myBitmap.He
                    //Image myThumbnail = myBitmap.GetThumbnailImage(30, 30, myCallBack, IntPtr.Zero);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        newIcon.Save(ms);// System.Drawing.Imaging.ImageFormat.Icon);

                        // myThumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Icon);//.Bmp);
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


        public ActionResult getfile(string Id, string data = "0")
        {
            try
            {

                string filepath = "";
                if (TempData["TempFolder"] != null)
                { filepath = TempData["TempFolder"].ToString(); }
                TempData["TempFolder"] = filepath;
                if (Id.Contains("_JpbUJpZ0ZpbGVz_")) Id = Id.Replace("_JpbUJpZ0ZpbGVz_", "");
                if (Id.Contains("DDOOTT")) Id = Id.Replace("DDOOTT", ".");
                var fullpath = Path.Combine(filepath, Id);


                FileInfo tempfileupload = new FileInfo(@fullpath);

                if (tempfileupload.Length > 200000)
                {
                    if (tempfileupload.Extension.ToLower() == ".pdf" || tempfileupload.Extension.ToLower() == ".xlsx" || tempfileupload.Extension.ToLower() == ".xls" || tempfileupload.Extension.ToLower() == ".docx" || tempfileupload.Extension.ToLower() == ".doc" || tempfileupload.Extension.ToLower() == ".pptx" || tempfileupload.Extension.ToLower() == ".ppt")
                    {
                        string comfile = fmcompress.compMain(fullpath);
                        if (comfile != "")
                        {
                            fullpath = comfile;
                        }
                    }
                    if (tempfileupload.Extension.ToLower() == ".jpg" || tempfileupload.Extension.ToLower() == ".png" || tempfileupload.Extension.ToLower() == ".gif" || tempfileupload.Extension.ToLower() == ".bmp" || tempfileupload.Extension.ToLower() == ".jpeg")
                    {
                        bool iscompressed = false;
                        try
                        {
                            // is it already compressed
                            var shellFile = ShellFile.FromFilePath(@fullpath);
                            var tags = (string[])shellFile.Properties.System.Keywords.ValueAsObject;
                            tags = tags ?? new string[0];

                            if (tags.Length != 0)
                            {
                                foreach (string str in tags)
                                {
                                    if (str.Contains("ftbytescompressed"))
                                    {
                                        iscompressed = true; break;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            //throw;
                        }


                        if (!iscompressed)
                        {
                            System.Drawing.Image i = System.Drawing.Image.FromFile(@fullpath);// @"C:\Users\jmredeni\Desktop\Image Compression Test Pictures\IMG_9622.JPG");
                            ImageProcessor.Imaging.Quantizers.WuQuantizer.WuQuantizer q = new ImageProcessor.Imaging.Quantizers.WuQuantizer.WuQuantizer();
                            //System.Drawing.Image i2 = q.Quantize(i, 0, 1, null, 255);
                            System.Drawing.Image i2 = q.Quantize(i, 0, 1, null, 256);

                            using (MemoryStream ms = new MemoryStream())
                            {
                                switch (tempfileupload.Extension.ToLower())
                                {
                                    case ".jpg":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        break;
                                    case ".png":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                        break;
                                    case ".gif":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                                        break;
                                    case ".bmp":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                                        break;
                                    case ".jpeg":
                                        i2.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        break;
                                }

                                string ccontentType = MimeMapping.GetMimeMapping(fullpath);
                                return File(ms.ToArray(), ccontentType); // mimeType);
                            }
                        }

                    }
                }

                /*            string comfile =fmcompress.compMain(fullpath);
                            if (comfile !="")
                            {
                                fullpath = comfile;
                            }
                            */


                string contentType = MimeMapping.GetMimeMapping(fullpath);
                // contentType = "application/pdf"
                byte[] fileBytes = System.IO.File.ReadAllBytes(@fullpath);
                return File(fileBytes, contentType); // mimeType);
            }
            catch (Exception eee)
            {
                return File(new byte[0], "application/pdf"); // mimeType);

                //throw;
            }
        }


        public ActionResult getdownicon(string Id)
        {
            string filepath = "";

            if (TempData["TempFolder"] != null)
            { filepath = TempData["TempFolder"].ToString(); }
            TempData["TempFolder"] = filepath;

            var dir = Server.MapPath("/Images");
            var path = Path.Combine(dir, "dragsave.png"); //validate the path for security or use other means to generate the path.
            return base.File(path, "image/png");
        }
        /*    try
            {
                Bitmap myBitmap = new Bitmap("~/images/dragsave.png");
                using (MemoryStream ms = new MemoryStream())
                {
                    myBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);//, myEncoderParameters); //.Bmp);
                    Response.ContentType = "image/png";
                    Response.BinaryWrite(ms.ToArray());
                    return Content("OK");
                }

            }
            catch (Exception eee)
            {
            }
        return Content("OK");
        */


        public ActionResult GoodView()
        {
            return View();
        }
        public ActionResult geticon(string Id, string data = "0")
        {
            try
            {
                string filepath = "";
                int width = 150;
                int height = 150;
                if (data == "1")
                {
                    width = 900;
                    height = 540;
                }

                if (TempData["TempFolder"] != null)
                { filepath = TempData["TempFolder"].ToString(); }

                TempData["TempFolder"] = filepath;

                if (Id.Contains("_JpbUJpZ0ZpbGVz_")) Id = Id.Replace("_JpbUJpZ0ZpbGVz_", "");

                if (Id.Contains("DDOOTT")) Id = Id.Replace("DDOOTT", ".");
                var fullpath = Path.Combine(filepath, Id);

                if (fullpath.ToLower().Contains(".jpg") || fullpath.ToLower().Contains(".png") || fullpath.ToLower().Contains(".gif") || fullpath.ToLower().Contains(".bmp") || fullpath.ToLower().Contains(".ico"))
                {
                    try
                    {
                        Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);


                        using (Bitmap myBitmap = new Bitmap(fullpath))
                        {
                            Image myThumbnail = myBitmap.GetThumbnailImage(width, height, myCallBack, IntPtr.Zero);
                            using (MemoryStream ms = new MemoryStream())
                            {
                                if (data == "1")
                                {
                                    myBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);//, myEncoderParameters); //.Bmp);
                                }
                                else
                                {
                                    myThumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); //.Bmp);
                                }
                                Response.ContentType = "image/jpg";
                                Response.BinaryWrite(ms.ToArray());
                                return Content("OK");
                            }
                        }

                    }
                    catch (Exception eee)
                    {
                        try
                        {
                            var b = new Bitmap(1, 1);
                            b.SetPixel(0, 0, Color.White);
                            Bitmap bmp = new Bitmap(b, 200, 200);

                            RectangleF rectf = new RectangleF(05, 05, 195, 195);
                            Graphics g = Graphics.FromImage(bmp);
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            g.DrawString(eee.Message, new System.Drawing.Font("Tahoma", 8), Brushes.Red, rectf);
                            g.Flush();
                            Image myImage = bmp;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                myImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); //.Bmp);
                                Response.ContentType = "image/jpg";
                                Response.BinaryWrite(ms.ToArray());
                                return Content("OK");
                            }
                        }
                        catch (Exception ecee)
                        {
                            var bbb = ecee.Message;
                            return Content("OK");
                            //throw;
                        }
                    }
                }



                //todo else has been removed for other routine which shows all
                // removing else gave lot of error

                else
                {
                    try
                    {
                        var b = new Bitmap(1, 1);
                        b.SetPixel(0, 0, Color.White);
                        Bitmap bmp = new Bitmap(b, 200, 200);

                        RectangleF rectf = new RectangleF(05, 05, 195, 195);
                        Graphics g = Graphics.FromImage(bmp);
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.DrawString(Id, new System.Drawing.Font("Tahoma", 16), Brushes.Red, rectf);
                        g.Flush();
                        Image myImage = bmp;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            myImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); //.Bmp);
                            Response.ContentType = "image/jpg";
                            Response.BinaryWrite(ms.ToArray());
                            return Content("OK");
                        }
                    }
                    catch (Exception eee)
                    {
                        var bbb = eee.Message;
                        return Content("OK");
                        //throw;
                    }
                }




                // try this first beofre using the lib for rimkus compresion
                // Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                // Bitmap myBitmap = new Bitmap(fullpath); // "c:\rimkus\testfile.jpg");
                // Image myThumbnail = myBitmap.GetThumbnailImage(width, height, myCallBack, IntPtr.Zero);

                //Todo this code worked for compressing the icon but it create too many files , shuld avoid , try using memory
                //var tempfilename = @"c:\temp\" + Guid.NewGuid().ToString() + ".jpg";
                //myThumbnail.Save(tempfilename, System.Drawing.Imaging.ImageFormat.Jpeg);//
                // compress it
                // string comfile =fmcompress.compMain(tempfilename);
                //if (System.IO.File.Exists(tempfilename)) System.IO.File.Delete(tempfilename);
                //if (comfile !="")
                //{
                //    myThumbnail = new Bitmap(comfile);
                //    System.IO.File.Delete(comfile);
                //}
                /*
                using (MemoryStream ms = new MemoryStream())
                {
                    if (data == "1")
                    {
                        //System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        //EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                        //EncoderParameters myEncoderParameters = new EncoderParameters(1);
                        //myEncoderParameters.Param[0] = myEncoderParameter;
                        myBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);//, myEncoderParameters); //.Bmp);

                //i have used this reference 
                        //http://stackoverflow.com/questions/1484759/quality-of-a-saved-jpg-in-c-sharp
                    }
                    else
                    {
                        myThumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); //.Bmp);
                    }
                    //myThumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); //.Bmp);
                    Response.ContentType = "image/jpg";
                    Response.BinaryWrite(ms.ToArray());
                    return Content("OK");
                }*/
                return Content("OK");
            }
            catch (Exception eee)
            {
                return Content("OK");
                //throw;
            }
        }

        public ActionResult getpic(string Id)
        {
            Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            Bitmap myBitmap = new Bitmap(@"C:\KristenLee\1.jpg"); // "c:\rimkus\testfile.jpg");
            Image myThumbnail = myBitmap.GetThumbnailImage(900, 540, myCallBack, IntPtr.Zero);

            using (MemoryStream ms = new MemoryStream())
            {
                myThumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); //.Bmp);
                Response.ContentType = "image/jpg";
                Response.BinaryWrite(ms.ToArray());
                return Content("OK");
            }
            return Content("OK");
        }

        public string GetFileType(string fileName)
        {
            SHFILEINFO shFileInfo = new SHFILEINFO();

            SHGetFileInfo(fileName, 0, ref shFileInfo, (uint)Marshal.SizeOf(shFileInfo), SHGFI_TYPENAME);

            return shFileInfo.szTypeName;
        }

        private string pretrail()
        {
            RootPath = RootPath.Trim();
            IsRootPath = true;
            if (RootPath.Length > 1)
            {
                if (RootPath.Contains("\\")) RootPath = RootPath.Replace("\\\\", "\\");
                if (RootPath.Contains("\\")) RootPath = RootPath.Replace("\\\\", "\\");
                RootPath = @RootPath.Substring(0, 1) == @"/" ? @RootPath.Substring(1, @RootPath.Length - 1) : @RootPath;
            }

            if (FileShare.Contains("PERSONALFOLDEROF"))
            {
                //FileSharePath = @RimkusHelper.DataParser("[AbpUsers.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim(); FileSharePath = "\\\\houserver\\users2$\\ZFaruqi";
                FileSharePath = "\\\\houserver\\users\\" + userwa;
                if (!Directory.Exists(@FileSharePath))
                {
                    FileSharePath = "\\\\houserver\\users2$\\" + userwa;
                }
            }
            else
            {
                FileSharePath = @RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim();
            }

            //            FileSharePath = @RimkusHelper.DataParser("[AbpFileShares.SharedFriendlyName]{SharedPath}", FileShare.Trim()).Trim();
            var tempsharepath = "";
            try
            {
                if (RootPath.Length > 1)
                {
                    tempsharepath = Path.Combine(FileSharePath, @RootPath.Substring(0, 1) == @"\" ? @RootPath.Substring(1, @RootPath.Length - 1) : @RootPath);
                    IsRootPath = false;
                }
                else
                { tempsharepath = FileSharePath; }
            }
            catch (Exception eee)
            {
                var bola = eee.Message;
                IsRootPath = true;
                throw;
            }
            if (Directory.Exists(@tempsharepath))
            {
                return tempsharepath;
            }

            return String.Empty;
        }

        public ActionResult fileView()
        {
            return View();
        }

        public string ZipUp()
        {

            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();
            List<string> mystringlist = JsonConvert.DeserializeObject<List<string>>(json);
            List<string> mystring = new List<string>();
            foreach (var item in mystringlist)
            {
                mystring.Add(dbgetfilepath(item));
            }


            //            RCGRemoteFile bola = RimkusHelper.JustName(mystring[0]);
            //            FileShare = bola.FileShare;
            //            RootPath = "";// bola.RootPath;
            //            // error if path is not found on allshare look
            //            var path = pretrail();
            var zipfile = Guid.NewGuid().ToString().Substring(0, 8) + ".zip";
            string havefile = "";
            if (mystring.Count > 0)
            {
                if (mystring[0].IndexOf(".") == 0)
                {
                    mystring[0] += ".dat";
                }
                zipfile = mystring[0].Substring(0, mystring[0].LastIndexOf(".") + 1) + "zip";




                for (int i = 1; i < 100; i++)
                {
                    if (!System.IO.File.Exists(zipfile))
                    {
                        break;
                    }
                    zipfile = zipfile.Replace(".", "(" + i.ToString() + ").");

                }
            }
            using (ZipFile zip = new ZipFile())
            {
                foreach (var item in mystring)
                {


                    if (System.IO.File.Exists(@item)) zip.AddFile(@item, "FTBFiles");

                    ;
                }

                if (zip.Count > 0) zip.Save(zipfile);


                justapilog("Zipped Files in " + json);

                LogData logdata = new LogData()
                {
                    UserName = userwa,
                    PortNumber = "443",
                    LogEntry = "Zipped the following Files" + json,
                    StartedAt = DateTime.UtcNow.ToString(),
                    EndsAt = DateTime.UtcNow.ToString(),
                    Company = "Houston", // RimkusHelper.APICompany,
                    DataRx = 0, // Convert.ToInt64(DataTx),
                    DataTx = 0// Convert.ToInt64(DataTx)
                };
                //System.IO.File.WriteAllText(@"c:\rimapi\testolog.txt", JsonConvert.SerializeObject(logdata));
                RimkusHelper.SendDataLogtoServer(logdata);

            }
            return "Done";

        }

        public void SendMailOverUse()
        {
            var message = new MailMessage();
            message.From = new MailAddress("ftbytes@rimkus.com"); // "cr@rimkus.com");
            message.To.Add(new MailAddress("bfaruqi@rimkus.com"));
            //message.Body = "User zfaruqi has viewed these files within the threahold limit. "+Environment.NewLine +"Test local Share\\1234567\\ftbxls.jpg"+ Environment.NewLine +

            message.Body = "User zfaruqi has downloaded these files within the threahold limit. " + Environment.NewLine + "Test local Share\\1234567\\ftbxls.jpg" + Environment.NewLine +

                "Test local Share\\1234567\\fw: application testing - FIle Management.pdf" + Environment.NewLine +
                "Test local Share\\1234567\\083746423.jpg" + Environment.NewLine +
                "Test local Share\\1234567\\manual.docx.jpg" + Environment.NewLine +
                "Test local Share\\1234567\\\"RE:Questionnaire checklist updates.msg" + Environment.NewLine +
                "Test local Share\\1234567\\987623242.jpg" + Environment.NewLine +
                "Test local Share\\1234567\\testfolder\\87832323.jpg" + Environment.NewLine +
                "Test local Share\\1234567\\testfolder\\mytest.jpg" + Environment.NewLine +
                "Test local Share\\1234567\\testfolder\\DSC99572.jpg" + Environment.NewLine +
                "Test local Share\\1234567\\testfolder\\DSC876832.jpg" + Environment.NewLine + "You can change the threshold value from the Admin Website for this user."
               ;
            message.Subject = "Over usage Report - Downloaded Files";

            using (var client = new SmtpClient("mail.rimkus.com", 25))
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = true;
                client.Timeout = 20000;
                client.Send(message);
            }
        }

        public ActionResult PictureGallery(string Id = "OTI2NjkqRElSKlJpbUJpZ0ZpbGVz")
        {
            ViewBag.LocalPath = Id;
            var mypath = RimkusHelper.Base64Decode(Id);
            string mypresentpath = mypath.Substring(mypath.LastIndexOf("\\") + 1);
            var tola = mypresentpath.Split('^');
            tola[0] = tola[0].Trim();
            tola[1] = RimkusHelper.GetBetween(tola[1], "[", "]");

            /* RCGRemoteFile bola = RimkusHelper.JustName(Id);
             if (bola.FileShare == "") return View(new ViewPics());
             FileShare = bola.FileShare;
             RootPath = bola.RootPath;
             // error if path is not found on allshare look
             */
            var path = dbgetdirpath(tola[1]) + tola[0];//  pretrail();

            TempData["TempFolder"] = @path;// "C:\Rimkus\TestBigFile\";
                                           // string[] exten = { ".jpg", ".bmp", ".gif", ".png", ".ico", ".jpeg" };
            try
            {

                /* Dictionary<int, string>  getalllist = JsonConvert.DeserializeObject<Dictionary<int, string>>(dbgetallfiles(tola[1]));
                 List<string> myImages =new List<string>();

                  foreach (var item in getalllist)
                     {
                     myImages.Add("["+ item.Key.ToString()+"]" +item.Value.ToString());
                     };
                 //  var model = new ViewPics() { };
                 */
                var model = new ViewPics()
                {
                    // Images = Directory.EnumerateFiles(Server.MapPath("~/images/fullscreen"))
                    //          .Select(fn => "~/images/fullscreen/" + Path.GetFileName(fn))
                    //Images = Directory.EnumerateFiles(@"\\HOUDMXL40305B8\TestShareRoot\Image Compression Test Pictures")
                    //         .Select(fn => "data:image/jpeg:base64," + ImageToBase64(@"\\HOUDMXL40305B8\TestShareRoot\Image Compression Test Pictures\" + Path.GetFileName(fn)))
                    //Images = Directory.EnumerateFiles(path,"*.jpg|*.bmp|*.gif|*.png|*.ico|*.jpeg")
                    // Images = Directory.EnumerateFiles(path)
                    Images = Directory.EnumerateFiles(path).Select(fn => "~/dbrimkus/geticon/_" + "JpbUJpZ0ZpbGVz" + "_" + Path.GetFileName(fn).Replace(".", "DDOOTT"))
                    //  Images=getalllist.AsEnumerable<int>
                };
                // //userwa = bola.tokencheck;
                justapilog("Viewed Image Files in " + @path);
                return View(model);
            }
            catch (Exception eee)
            {
                return View(new ViewPics());
                //throw;
            }



        }

        private static IEnumerable<string> GetFiles(string sourceFolder, string filters)
        {
            return filters.Split('|').SelectMany(filter => System.IO.Directory.EnumerateFiles(sourceFolder, filter));
        }

        public string ImageToBase64(string Id)  // Image image, System.Drawing.Imaging.ImageFormat format)
        {
            if (Id.Substring(Id.LastIndexOf('.')).ToLower() == ".jpeg" || Id.Substring(Id.LastIndexOf('.')).ToLower() == ".jpg" || Id.Substring(Id.LastIndexOf('.')).ToLower() == ".png" || Id.Substring(Id.LastIndexOf('.')).ToLower() == ".gif" || Id.Substring(Id.LastIndexOf('.')).ToLower() == ".bmp" || Id.Substring(Id.LastIndexOf('.')).ToLower() == ".ico")
            { }
            else
                Id = "~/image/nopicfile.jpg";
            int newSize = 60;
            Image image = Image.FromFile(Id);

            if (image.Width <= newSize)
                newSize = image.Width;

            var newHeight = image.Height * newSize / image.Width;

            if (newHeight > newSize)
            {
                // Resize with height instead
                newSize = image.Width * newSize / image.Height;
                newHeight = newSize;
            }

            Image.GetThumbnailImageAbort myCallBack = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            Bitmap myBitmap = new Bitmap(@Id);// "c:\rimkus\testfile.jpg");
            Image myThumbnail = myBitmap.GetThumbnailImage(newSize, newHeight, myCallBack, IntPtr.Zero);
            using (MemoryStream ms = new MemoryStream())
            {
                myThumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);//.Bmp);
                return Convert.ToBase64String(ms.ToArray());
            }

            /*Image image = Image.FromFile(Id);
            //var format =image.Form
            var format = System.Drawing.Imaging.ImageFormat.Jpeg;  //later try memory
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }*/

            //Id = @"C:\RCGProjects\noanimage.jpg";
        }
    }
}