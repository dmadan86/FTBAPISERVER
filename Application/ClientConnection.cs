using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;

namespace FTBAPISERVER.Application
{
    public class ClientConnection
    {
        private System.ComponentModel.IContainer components;
        private System.Diagnostics.EventLog eventLog1;
        private FileStream tempfile;
        private bool m_started = false;
        //private ServiceMode m_mode = 0;
        private ManualResetEvent m_stop;
        private IPEndPoint m_endPoint;
        private Thread m_workingThread;
        private UdpClient m_client;
        private const int PORT_NUMBER = 32000;
        private string fullFilePathName;

        public ClientConnection(int PortNumber, String filedata)
        {
            Start();
        }
        private void Start()
        {
            fullFilePathName = @"C:\OpenJobs\Houston Open Jobs\12345678\this is test.txt"; //  @"C:\" + @_FileData.RootFolder + @"\" + _FileData.SubFolder + @"\" + _FileData.FileName;
            var directoryname = Path.GetDirectoryName(fullFilePathName);
            System.IO.Directory.CreateDirectory(directoryname);
            tempfile = new FileStream(@fullFilePathName, FileMode.Create, FileAccess.Write);
            m_started = true;
            m_stop = new ManualResetEvent(false);
            InitializeUdpClient();
            InitializeWorkingThread();
        }
        private void InitializeUdpClient()
        {
            m_endPoint = new IPEndPoint(IPAddress.Any, PORT_NUMBER);
            m_client = new UdpClient(m_endPoint);

        }
        private void InitializeWorkingThread()
        {
            m_workingThread = new Thread(WorkerFunction);
            m_workingThread.Name = "ClientThread";
            m_workingThread.Start();
        }
        private void WorkerFunction()
        {
            //InitialData RecdData;
            bool? UserAuthenticated = false;
            while (m_started)
            {
                var res = m_client.BeginReceive(iar =>
                {
                    if (iar.IsCompleted)
                    {
                        byte[] receivedBytes = m_client.EndReceive(iar, ref m_endPoint);
                        string receivedPacket = Encoding.ASCII.GetString(receivedBytes);
                        if (receivedPacket == "\r\n.\r\n")
                        {
                            tempfile.Close();
                            //m_mode = ServiceMode.Terminating;
                        }
                        tempfile.Write(receivedBytes, 0, receivedBytes.Length);
                        tempfile.Close();
                        tempfile = new FileStream(@fullFilePathName, FileMode.Append, FileAccess.Write);
                    }
                }, null);

                if (WaitHandle.WaitAny(new[] { m_stop, res.AsyncWaitHandle }) == 0)
                {
                    break;
                }
            }
        }

    }
}