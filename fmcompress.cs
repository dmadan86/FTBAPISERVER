using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace FTBAPISERVER
{
    public static class fmcompress
    {
        // Function prototypes of the FILEminimizer SDK
        [DllImport("fileminimizersdk.dll", EntryPoint = "InitFILEminimizerSDK", CallingConvention = CallingConvention.StdCall)]
        private static extern int InitFILEminimizerSDK(string InitParams);

        [DllImport("fileminimizersdk.dll", EntryPoint = "DoneFILEminimizerSDK", CallingConvention = CallingConvention.StdCall)]
        private static extern void DoneFILEminimizerSDK();

        [DllImport("fileminimizersdk.dll", EntryPoint = "StartOptimizationJob", CallingConvention = CallingConvention.StdCall)]
        private static extern int StartOptimizationJob(string InputFileName, string OutputFileName);

        [DllImport("fileminimizersdk.dll", EntryPoint = "IsOptimizationJobCompleted", CallingConvention = CallingConvention.StdCall)]
        private static extern bool IsOptimizationJobCompleted();

        [DllImport("fileminimizersdk.dll", EntryPoint = "GetCurrentJobProgress", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetCurrentJobProgress();

        [DllImport("fileminimizersdk.dll", EntryPoint = "GetOptimizationJobResult", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetOptimizationJobResult();

        [DllImport("fileminimizersdk.dll", EntryPoint = "GetImageJobResult", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetImageJobResult();

        [DllImport("fileminimizersdk.dll", EntryPoint = "SetOptimizationLevelTo", CallingConvention = CallingConvention.StdCall)]
        private static extern void SetOptimizationLevelTo(int nLevel);

        [DllImport("fileminimizersdk.dll", EntryPoint = "SetJobScreenResolution", CallingConvention = CallingConvention.StdCall)]
        private static extern void SetJobScreenResolution(int nWidth, int nHeight);

        [DllImport("fileminimizersdk.dll", EntryPoint = "SetJobJPEGQuality", CallingConvention = CallingConvention.StdCall)]
        private static extern void SetJobJPEGQuality(int nCompression);

        [DllImport("fileminimizersdk.dll", EntryPoint = "SetJobPPIQuality", CallingConvention = CallingConvention.StdCall)]
        private static extern void SetJobPPIQuality(int nCompression);

        [DllImport("fileminimizersdk.dll", EntryPoint = "SetJobImageOptions", CallingConvention = CallingConvention.StdCall)]
        private static extern void SetJobImageOptions(bool AllowChangeFormat, bool UseLosslessOutput, bool AllowImageResize, bool KeepExif);

        [DllImport("fileminimizersdk.dll", EntryPoint = "SetOLEObjectsProcessingType", CallingConvention = CallingConvention.StdCall)]
        private static extern void SetOLEObjectsProcessingType(bool Flatten);

        [DllImport("fileminimizersdk.dll", EntryPoint = "BreakOptimizationJob", CallingConvention = CallingConvention.StdCall)]
        private static extern bool BreakOptimizationJob();

        [DllImport("fileminimizersdk.dll", EntryPoint = "GetInputSizeInMB", CallingConvention = CallingConvention.StdCall)]
        private static extern double GetInputSizeInMB();

        [DllImport("fileminimizersdk.dll", EntryPoint = "GetOutputSizeInMB", CallingConvention = CallingConvention.StdCall)]
        private static extern double GetOutputSizeInMB();

        [DllImport("fileminimizersdk.dll", EntryPoint = "GetSavedSpaceInMB", CallingConvention = CallingConvention.StdCall)]
        private static extern double GetSavedSpaceInMB();

        [DllImport("fileminimizersdk.dll", EntryPoint = "GetOptimizationRatio", CallingConvention = CallingConvention.StdCall)]
        private static extern double GetOptimizationRatio();

        [DllImport("fileminimizersdk.dll", EntryPoint = "GenerateOutputFileName", CallingConvention = CallingConvention.StdCall)]
        private static extern void GenerateOutputFileName(string OptimizedTextID, bool AppendTextID,
                                                          string InputFileName, StringBuilder OutputFileName);
        public const
           int OP_Error = -1,
               OP_Success = 0,
               OP_Break = 1,
               OP_FileInUse = 2,
               OP_NoPowerPointFormat = 3,
               OP_PowerPoint95Format = 4,
               OP_EncryptedPowerPoint = 5,
               OP_IdenticalFileNames = 6;


        public static string compMain(string filename)
        {
            try
            {
                bool iscompressed = false;
                try
                {
                    // is it already compressed
                    var shellFile = ShellFile.FromFilePath(@filename);
                    var tags = (string[])shellFile.Properties.System.Keywords.ValueAsObject;
                    tags = tags ?? new string[0];

                    if (tags.Length != 0)
                    {
                        foreach (string str in tags)
                        {
                            if (str.Contains("ftbytescompressed"))
                            { iscompressed = true; break; }
                        }
                    }

                    if (iscompressed)
                        return filename;

                }
                catch (Exception)
                {
                    //throw;
                }
                string realfile = "";
                string outputfile = "";
                const
                   int OP_Error = -1,
                       OP_Success = 0,
                       OP_Break = 1,
                       OP_FileInUse = 2,
                       OP_NoPowerPointFormat = 3,
                       OP_PowerPoint95Format = 4,
                       OP_EncryptedPowerPoint = 5,
                       OP_IdenticalFileNames = 6;


                // Initialize the SDK and check if there is an error
                try
                {
                    if (InitFILEminimizerSDK("") != OP_Success) goto Finish;
                }
                catch (Exception eee)
                {

                    return "";
                    //throw;
                }

                outputfile = Path.GetTempFileName();
                string InputFileName = filename;// @"c:\rimkus\img100.jpg";//) "c:\myfile.ppt";
                StringBuilder OutputFileName = new StringBuilder(500);
                //GenerateOutputFileName("(FMSDK)", true, InputFileName, OutputFileName);
                //OutputFileName.Append(outputfile);
                //GenerateOutputFileName("(FMSDK)", true, InputFileName, OutputFileName);
                //SetJobScreenResolution(1024, 768);
                //SetJobJPEGQuality(50);
                SetOptimizationLevelTo(1);

                SetOLEObjectsProcessingType(true);
                int Progress;
                int JobResult;
                bool JobDone = false;
                JobResult = StartOptimizationJob(InputFileName, outputfile); // OutputFileName.ToString());
                switch (JobResult)
                {
                    case OP_Success:
                        break;

                    case OP_FileInUse:
                        break;
                }
                if (JobResult != OP_Success) goto Finish;

                int OldProgress = 0;
                do
                {
                    Progress = GetCurrentJobProgress();
                    if ((Progress % 5 == 0) && (OldProgress != Progress))
                    {
                        OldProgress = Progress;
                        //myProgress(counter, "CONVERT", Progress, datestarted, Progress, "Compress");
                    }
                    else
                        JobDone = IsOptimizationJobCompleted();
                } while (!JobDone);

                Finish:

                try
                {


                    JobResult = GetOptimizationJobResult();


                    // We check whether there had been any errors upon start or not
                    switch (JobResult)
                    {
                        case OP_Error:
                            break;

                        case OP_Success:
                            realfile = outputfile;
                            break;

                        case OP_Break:
                            break;

                        case OP_FileInUse:
                            break;

                        case OP_NoPowerPointFormat:
                            break;

                        case OP_PowerPoint95Format:
                            break;

                        case OP_EncryptedPowerPoint:
                            break;

                        case OP_IdenticalFileNames:
                            break;
                    }
                    // Finalize the FILEminimizer SDK
                    DoneFILEminimizerSDK();
                    if (!File.Exists(realfile))
                        return "";

                    if (File.Exists(realfile) && new FileInfo(realfile).Length > 0)
                    {
                        return realfile;
                    }
                    else
                        if (File.Exists(realfile) && new FileInfo(realfile).Length < 1)
                    {
                        File.Delete(realfile);
                        realfile = realfile.Substring(0, realfile.LastIndexOf(".")) + filename.Substring(filename.LastIndexOf("."));

                        if (!File.Exists(realfile))
                            return "";

                        try
                        {
                            if (File.Exists(realfile) && new FileInfo(realfile).Length > 0)
                            {
                                try
                                {
                                    string[] keywords = new string[] { "ftbytescompressed" };
                                    var realshellFile = ShellFile.FromFilePath(realfile);
                                    realshellFile.Properties.System.Keywords.Value = keywords;
                                    return realfile;
                                }
                                catch (Exception)
                                {
                                    return realfile;
                                    // throw;
                                }
                               
                            }
                        }
                        catch (Exception)
                        {
                            return "";
                        }
                    }
                }
                catch (Exception e)
                {

                    return "";
                }
                try
                {
                    string[] keywordss = new string[] { "ftbytescompressed" };
                    var realshellFilee = ShellFile.FromFilePath(realfile);
                    realshellFilee.Properties.System.Keywords.Value = keywordss;
                }
                catch (Exception)
                {
                    //throw;
                }
                return realfile;
            }
            catch (Exception eee)
            {

                return "";
            }
        }

    }
}