using FTBAPISERVER.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

//using System.Transactions.TransactionScope;

namespace FTBAPISERVER.Application
{
    public class FTBConnections : IDisposable
    {
        MemoryStream ftbstream = new MemoryStream();
        private bool m_started = false;

        //private ServiceMode m_mode = 0;
        private ManualResetEvent m_stop;

        private System.Timers.Timer _timer;
        private IPEndPoint m_endPoint;
        private Thread m_workingThread;
        private UdpClient m_client;
        private int m_portNumber;
        private string FTBCommands = "";
        private string FTBParameter = "";
        private string FTBPreviousParameter = "";
        private FileTxInfo _myToken;
        private string _prevCommand = "";
        private byte[] _lastBlock;
        private bool Validated = true;// false;
        public byte[] mDataGram;//= FTBResponse.NOOP(new TokenData());
        private StringBuilder stringer;

        public delegate void DisposeConnection();

        public event DisposeConnection DonewithIt;

        public FTBConnections(int portNumber, FileTxInfo mytokenwa)
        {
            m_portNumber = portNumber;
            m_started = true;
            m_stop = new ManualResetEvent(false);

            _myToken = mytokenwa;
            _timer = new System.Timers.Timer();
            _timer.Interval = 360000;

            _timer.Elapsed += OnTimerEvent;
            _timer.Start();

            InitializeUdpClient();
            InitializeWorkingThread();
            stringer = new StringBuilder(); 
        }

        private void OnTimerEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            closeConnection();
            /*            _timer.Stop();
                        _timer.Dispose();
                        m_workingThread.Abort();
                        m_started = false;
                        m_client.Close();
                        m_client = null;
                        //Dispose();
                        DonewithIt.Invoke();*/
        }

        private void closeConnection()
        {
            //mDataGram = Encoding.ASCII.GetBytes("221 Service closing control connection");
            //m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
            try
            {
                if (_prevCommand == "STOR")
                {
                    if (!String.IsNullOrEmpty(FTBPreviousParameter))
                    {
                        FileInfo f = new FileInfo(FTBPreviousParameter);
                        long s1 = f.Length;
                        if (s1 == 0)
                        {
                            File.Delete(@FTBPreviousParameter);

                            try
                            {
                                byte[] mgDataGram = Encoding.ASCII.GetBytes(CloseFile("200 Last File disregarded as no incoming data."));
                                m_client.Send(mgDataGram, mgDataGram.Length, m_endPoint);
                            }
                            catch { }
                        }
                    }
                }
                //            File.Delete(@FTBPreviousParameter);
                _prevCommand = "";
                byte[] lastBlock;
                Validated = true;// false;
                byte[] mDataGram;//= FTBResponse.NOOP(new TokenData());
                _timer.Stop();
                _timer.Dispose();
                m_workingThread.Abort();
                m_started = false;
                m_client.Close();
                m_client.Client = null;
                m_client = null;
                //Dispose();
                DonewithIt.Invoke();
            }
            catch
            {
            }
        }

        private void InitializeUdpClient()
        {
            try
            {
                m_endPoint = new IPEndPoint(IPAddress.Any, m_portNumber);// PORT_NUMBER);
                m_client = new UdpClient(m_endPoint);
                //_client.Client.SendTimeout = 360000;
                //m_client.Client.ReceiveTimeout = 360000;
            }
            catch (Exception ttt)
            {
                var yyy = ttt.Message;
                //closeConnection();
            }
        }

        private void InitializeWorkingThread()
        {
            try
            {
                m_workingThread = new Thread(WorkerFunction);
                m_workingThread.Name = "WorkingThread";
                m_workingThread.Start();
            }
            catch (Exception)
            {
                // throw;
            }
        }

        private string WriteBytes(string fullFilePathName)  //,byte[] checkData)
        {
            bool openfile = false;
            if (_lastBlock == null || _lastBlock.Length == 0)
            {
               // ftbstream = new MemoryStream(); 
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + "No Old data to write");
                return "00000000";
            }
            //ftbstream.Write(_lastBlock, 0, _lastBlock.Length);
            //Thread.Sleep(500);
            //return "OK";

            /*
            using (TransactionScope tran = new TransactionScope())
            {
                tran.Complete();
            }
            */
            // for (int i = 0; i < 100; i++)
            // {
            //        if (IsFileReady(@fullFilePathName))
            //        {
            try
            {
                var tempfile = new FileStream(@fullFilePathName, FileMode.Append, FileAccess.Write);
                tempfile.Write(_lastBlock, 0, _lastBlock.Length);
                tempfile.Close();
                if (_lastBlock.Length > 55000) { Thread.Sleep(1500); }else { Thread.Sleep(150); }
                do
                {
                    try
                    {
                        using (Stream stream = System.IO.File.Open(fullFilePathName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            openfile = true;
                            stream.Close();
                        };
                    }
                    catch (System.NullReferenceException vvv)
                    {
                        openfile = false;
                        System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " " + "Exception Null Write " + vvv.Message);
                        Thread.Sleep(5000);
                    }
                    catch (UnauthorizedAccessException fff)
                    {
                        openfile = false;
                        System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " " + "Exception Access Write " + fff.Message);
                        Thread.Sleep(5000);
                        //throw;
                    }

                    catch (Exception eee)
                    {
                        openfile = false;
                        System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " " + "Exception Inner Write " + eee.Message);
                        Thread.Sleep(3000);
                        //throw;
                    }
                } while (!openfile) ;
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " " + "Wrote Bytes Successfully");
                return "OK";// RimkusHelper.ComputeCheckSum(_lastBlock);
            }
            catch (Exception ddd)
            {
                openfile = false;
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " " + "Exception Outer Write " + ddd.Message);
                Thread.Sleep(1000);
                return "ERROR";
                //throw;
            }
            //      }
            //    System.Threading.Thread.Sleep(1000);
            //}

            //return "OK";
        }

        private string WriteBytesPast(string fullFilePathName, byte[] checkData)
        {
            /*
            using (TransactionScope tran = new TransactionScope())
            {
                tran.Complete();
            }
            */

            var tempfile = new FileStream(@fullFilePathName, FileMode.Append, FileAccess.Write);
            tempfile.Write(checkData, 0, checkData.Length);
            tempfile.Close();
            return RimkusHelper.ComputeCheckSum(checkData);
        }

        public static bool IsFileReady(String sFilename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    if (inputStream.Length > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string CloseFile(string fullFilePathName)
        {

           // long totalbytes = 0;
           // using (FileStream file = new FileStream(@fullFilePathName, FileMode.Create, System.IO.FileAccess.Write))
           // {
           //     totalbytes = ftbstream.Length;
           //     byte[] bytes = new byte[ftbstream.Length];
           //     ftbstream.Read(bytes, 0, (int)ftbstream.Length);
           //     file.Write(bytes, 0, bytes.Length);
           //     ftbstream.Close();
           // }

            //return "220 OK FileSize On file is " + totalbytes.ToString();


            try
            {
                FileInfo f = new FileInfo(fullFilePathName);
                long s1 = f.Length;
                var filesize = s1.ToString();
                // remove last block
                _lastBlock = null;
                return "220 OK FileSize On file is " + filesize; // RimkusHelper.ComputeCheckSum(checkData);
            }
            catch (Exception)
            {
                return "220 OK FileSize cannot be ascertained" ;// On file is 1234567890";
            }
        }

        private void WorkerFunction()
        {
            if (m_client == null) return;
            if (m_client.Client == null) return;
            //InitialData RecdData;

            try
            {
                while (m_started)
                {
                    var res = m_client.BeginReceive(iar =>
                    {
                        if (iar.IsCompleted)
                            // check if the Toek time is good
                            _timer.Stop();
                        if (_myToken.ValidTime < DateTime.Now)
                        {
                            //todo
                            //bring it back mDataGram = Encoding.ASCII.GetBytes("500 Time Out Error . You need to get a new Token");
                            //m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                            //closeConnection();
                            // close connection using the delegate method
                            //return;
                        }
                        {
                            if (m_client == null) return;
                            if (m_client.Client == null) return;
                            byte[] receivedBytes;
                            try
                            {
                                receivedBytes = m_client.EndReceive(iar, ref m_endPoint);
                            }
                            catch
                            {
                                _timer.Stop();
                                closeConnection();
                                return;
                            }

                            if (receivedBytes.Length == 0) return;
                            string receivedPacket;
                            if (receivedBytes.Length < 300) { receivedPacket = Encoding.ASCII.GetString(receivedBytes); } else { receivedPacket = "NOOP"; }
                            //string receivedPacket = Encoding.ASCII.GetString(receivedBytes);
                            //FTBCommands = (receivedPacket.Length == 4) ?  receivedPacket.Substring(0, 4).ToUpper() : receivedPacket;
                            // Make sure you write the block after receiving confirmation from the sender

                            if (_prevCommand == "NOOP")
                            {
                                FTBPreviousParameter = String.Empty;
                            }
                            if (_prevCommand == "STOR")
                            {
                                if (receivedPacket == "ABOR")
                                {
                                    mDataGram = Encoding.ASCII.GetBytes("200 File Upload Aborted. File removed");
                                    File.Delete(@FTBPreviousParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append(Encoding.ASCII.GetString(mDataGram)+"\r\n" );
                                    FTBPreviousParameter = String.Empty;
                                    _prevCommand = "NOOP";
                                    return;
                                }
                                if (receivedPacket == "DRGD")
                                {
                                    _lastBlock = null;
                                    mDataGram = Encoding.ASCII.GetBytes(CloseFile("200 Last block disregarded. You can send the block again."));
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    return;
                                }
                                if (receivedPacket == "\r\n.\r\n")
                                {
                                    //mDataGram = Encoding.ASCII.GetBytes(WriteBytes(@FTBPreviousParameter, receivedBytes));
                                    //m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    WriteBytes(@FTBPreviousParameter);
                                    mDataGram = Encoding.ASCII.GetBytes(CloseFile(@FTBPreviousParameter));
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    FTBPreviousParameter = String.Empty;
                                    _prevCommand = "NOOP";
                                    return;
                                    // do end of file
                                }



                                try
                                {
                                    WriteBytes(@FTBPreviousParameter);
                                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + "Write Old data if any" );
                                    _lastBlock = receivedBytes;
                                    mDataGram = Encoding.ASCII.GetBytes(RimkusHelper.ComputeCheckSum(receivedBytes));
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + "Send to Client and waiting. " + Encoding.ASCII.GetString(mDataGram));
                                    stringer.Append(DateTime.Now.ToString() + " " + Encoding.ASCII.GetString(mDataGram) + "\r\n");
                                }
                                catch (Exception eee ) 
                                {
                                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " " + "FromMain "+eee.Message);
                                    stringer.Append(DateTime.Now.ToString() + " " + "FromMain " + eee.Message + "\r\n");
                                    //throw;
                                }

                                //mDataGram = Encoding.ASCII.GetBytes(WriteBytes(@FTBPreviousParameter) ;//, receivedBytes));
                                //m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                return;
                            }

                            //_lastBlock = receivedBytes;
                            FTBCommands = (receivedPacket.Length > 3) ? receivedPacket.Substring(0, 4).ToUpper() : receivedPacket;
                            FTBParameter = (receivedPacket.Length > 5 && receivedPacket.Substring(4, 1) == " ") ? receivedPacket.Substring(5) : String.Empty;
                            //_prevCommand = (FTBCommands + "NOOP").Substring(0, 4);
                            switch (FTBCommands)
                            {
                                case "AFFR":// affirmative
                                    mDataGram = FTBResponse.AFFR(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "NEGT":// negate
                                    mDataGram = FTBResponse.NEGT(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "USER":// user name
                                    mDataGram = FTBResponse.USER(_myToken, FTBParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "PASS": // password
                                    mDataGram = FTBResponse.PASS(_myToken);

                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "ACCT"://account AD or Windows
                                    mDataGram = FTBResponse.ACCT(_myToken, FTBParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "CWDY": // chaneg orking directory
                                    mDataGram = FTBResponse.CWDY(FTBParameter, ref _myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "CDUP": //change to Parent
                                    _myToken.RootPath = "";
                                    mDataGram = Encoding.ASCII.GetBytes("220 You are now on Root Folder :" + _myToken.FileShare);
                                    //mDataGram = FTBResponse.CDUP(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "SMNT": // structure mount
                                    mDataGram = FTBResponse.SMNT(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "QUIT": // logout close connection
                                    _prevCommand = "NOOP";
                                    mDataGram = FTBResponse.QUIT(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");


                                    _prevCommand = FTBCommands;
                                    _timer.Stop();
                                    closeConnection();
                                    return;

                                case "REIN": // reinitialize action , deleting evything
                                    mDataGram = FTBResponse.REIN(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "PORT": // data port
                                             //m_portNumber
                                    mDataGram = Encoding.ASCII.GetBytes("220 You are using Port Number :" + m_portNumber.ToString());
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "PASV": // passive
                                    mDataGram = FTBResponse.PASV(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "TYPE": // representation type
                                    mDataGram = FTBResponse.TYPE(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");


                                    _prevCommand = FTBCommands;
                                    break;

                                case "STRU": // file structure F R P
                                    mDataGram = FTBResponse.STRU(_myToken, FTBParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "MODE": // transfer mode stream block comressed
                                    mDataGram = FTBResponse.MODE(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "RETR": // retrieve file
                                    mDataGram = FTBResponse.RETR(_myToken, FTBParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    _prevCommand = FTBCommands;
                                    break;

                                case "STOR": //stor file or save file
                                    if (String.IsNullOrEmpty(FTBParameter.Trim()))
                                    {
                                        mDataGram = Encoding.ASCII.GetBytes("450 No file name found");
                                        m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                        stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                        _prevCommand = "NOOP";
                                        break;
                                    }
                                    string filepathname = "";
                                    mDataGram = FTBResponse.STOR(_myToken, FTBParameter, ref filepathname);
                                    if (!String.IsNullOrEmpty(filepathname)) FTBPreviousParameter = filepathname;
                                    _prevCommand = FTBCommands;
                                    if (Encoding.ASCII.GetString(mDataGram).Substring(0, 3) != "125") _prevCommand = "NOOP";
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    stringer.Append( DateTime.Now.ToString()+" "+Encoding.ASCII.GetString(mDataGram) + "\r\n");

                                    break;

                                case "SYCH": //stor file or save file
                                    if (String.IsNullOrEmpty(FTBParameter.Trim()))
                                    {
                                        mDataGram = Encoding.ASCII.GetBytes("450 No file name found");
                                        m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                        _prevCommand = "NOOP";
                                        break;
                                    }
                                    string filepathnamer = "";
                                    mDataGram = FTBResponse.STOR(_myToken, FTBParameter, ref filepathnamer);
                                    if (!String.IsNullOrEmpty(filepathnamer)) FTBPreviousParameter = filepathnamer;
                                    _prevCommand = "STOR";
                                    if (Encoding.ASCII.GetString(mDataGram).Substring(0, 3) != "125") _prevCommand = "NOOP";

                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    break;

                                case "STOU": //store as unique
                                    if (String.IsNullOrEmpty(FTBParameter.Trim()))
                                    {
                                        mDataGram = Encoding.ASCII.GetBytes("450 No file name found");
                                        m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                        _prevCommand = "NOOP";
                                        break;
                                    }
                                    string filepathname3 = "";
                                    mDataGram = FTBResponse.STOU(_myToken, FTBParameter, ref filepathname3);
                                    if (!String.IsNullOrEmpty(filepathname3)) FTBPreviousParameter = filepathname3;
                                    _prevCommand = "STOR";// FTBCommands;
                                    if (Encoding.ASCII.GetString(mDataGram).Substring(0, 3) != "125") _prevCommand = "NOOP";
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    break;

                                case "APPE": // apppend to file
                                    if (String.IsNullOrEmpty(FTBParameter.Trim()))
                                    {
                                        mDataGram = Encoding.ASCII.GetBytes("450 No file name found");
                                        m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                        _prevCommand = "NOOP";
                                        break;
                                    }
                                    string filepathname2 = "";
                                    mDataGram = FTBResponse.APPE(_myToken, FTBParameter, ref filepathname2);
                                    if (!String.IsNullOrEmpty(filepathname2)) FTBPreviousParameter = filepathname2;

                                    _prevCommand = "STOR";// FTBCommands;

                                    if (Encoding.ASCII.GetString(mDataGram).Substring(0, 3) != "125") _prevCommand = "NOOP";
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);

                                    break;

                                case "ALLO": //allocate enugh sapce
                                    mDataGram = FTBResponse.ALLO(_myToken, FTBParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "REST": //restart action
                                    mDataGram = FTBResponse.REST(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "RNFR": // rename from
                                    FTBPreviousParameter = FTBParameter;
                                    mDataGram = FTBResponse.RNFR(_myToken, FTBParameter);
                                    if (Encoding.ASCII.GetString(mDataGram).Substring(0, 3) == "450") FTBPreviousParameter = "";
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "RNTO":// rename to
                                    if (FTBPreviousParameter == "")
                                    {
                                        mDataGram = Encoding.ASCII.GetBytes("553 Requested action not taken. No rename from folder name");
                                        m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                        _prevCommand = "NOOP";
                                        break;
                                    }
                                    mDataGram = FTBResponse.RNTO(_myToken, FTBPreviousParameter + "," + FTBParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    if (Encoding.ASCII.GetString(mDataGram).Substring(0, 3) == "220") FTBPreviousParameter = "";
                                    _prevCommand = FTBCommands;
                                    break;

                                case "RFFR": // rename file from
                                    FTBPreviousParameter = FTBParameter;
                                    mDataGram = FTBResponse.RFFR(_myToken, FTBParameter);
                                    if (Encoding.ASCII.GetString(mDataGram).Substring(0, 3) == "450") FTBPreviousParameter = "";
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "RFTO":// rename file to
                                    if (FTBPreviousParameter == "")
                                    {
                                        mDataGram = Encoding.ASCII.GetBytes("553 Requested action not taken. No rename from file name");
                                        m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                        break;
                                    }
                                    mDataGram = FTBResponse.RFTO(_myToken, FTBParameter + "," + FTBPreviousParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "ABOR": // abort action delete file send
                                    mDataGram = FTBResponse.ABOR(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "RVOK": // abort action delete file send
                                    mDataGram = FTBResponse.RVOK(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    closeConnection();
                                    return;
                                //break;
                                case "RMDY": // remove directory
                                    mDataGram = FTBResponse.RMDY(_myToken, FTBParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "MKDY": // make directory
                                    mDataGram = FTBResponse.MKDY(_myToken, FTBParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "PWDR": // print directory
                                    mDataGram = FTBResponse.PWDR(_myToken, FTBParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "DELE": // delete file
                                    mDataGram = FTBResponse.DELE(_myToken, FTBParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "LIST": // file or directory
                                    mDataGram = FTBResponse.LIST(_myToken, FTBParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "NLST": // jsut file name and folder name
                                    mDataGram = FTBResponse.NLST(_myToken, FTBParameter);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "SITE": // site information
                                    mDataGram = FTBResponse.SITE(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "SYST": // operating system for client and server
                                    mDataGram = FTBResponse.SYST(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "STAT": //status of connection
                                    mDataGram = FTBResponse.STAT(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "HELP": // this help file
                                    mDataGram = FTBResponse.HELP(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "FIND":
                                    // this help file
                                    //DataGram = FTBResponse.HELP(_myToken);
                                    //m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                case "NOOP": // no operation
                                    mDataGram = FTBResponse.NOOP(_myToken);
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = FTBCommands;
                                    break;

                                default:
                                    mDataGram = Encoding.ASCII.GetBytes("500 COMMAND ERROR");
                                    m_client.Send(mDataGram, mDataGram.Length, m_endPoint);
                                    _prevCommand = "NOOP";
                                    break;
                            }
                        }
                        _timer.Start();
                    }, null);
                    if (WaitHandle.WaitAny(new[] { m_stop, res.AsyncWaitHandle }) == 0)
                    {
                        break;
                    }
                }
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            //Dispose(true);
            //this.Dispose;
            //throw new NotImplementedException();
        }
    }
}