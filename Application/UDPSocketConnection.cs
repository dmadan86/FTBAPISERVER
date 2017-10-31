using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;

namespace FTBAPISERVER.Application
{
    public class UDPSocketConnection
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
        private int m_portNumber;
        private const int PORT_NUMBER = 5000;
        private const string PRIVATE_PORT_NUMBER = "32000";


        public UDPSocketConnection(int portNumber)
        {
            m_portNumber = portNumber;
            m_started = true;
            m_stop = new ManualResetEvent(false);
            InitializeUdpClient();
            InitializeWorkingThread();
        }
        private void InitializeUdpClient()
        {
            m_endPoint = new IPEndPoint(IPAddress.Any, m_portNumber);// PORT_NUMBER);
            m_client = new UdpClient(m_endPoint);
        }

        private void InitializeWorkingThread()
        {
            m_workingThread = new Thread(WorkerFunction);
            m_workingThread.Name = "WorkingThread";
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
                        if (receivedPacket.Substring(0, 4) == "HELO")
                        {
                            string UserData = receivedPacket.Substring(4);
                            //RecdData = JsonConvert.DeserializeObject<InitialData>(UserData);
                            if (VerifyUserAccessToWrite("RecdData"))
                            {
                                UserAuthenticated = true;
                                // getport number
                                Random rnd = new Random();
                                int portnum = rnd.Next(20000, 40000);
                                var DataGram = Encoding.ASCII.GetBytes(portnum.ToString());// PRIVATE_PORT_NUMBER);
                                m_client.Send(DataGram, DataGram.Length, m_endPoint);
                                //m_mode = ServiceMode.Authenticated;
                                NewClientConnects(portnum);
                                //ClientConnection newClient = new ClientConnection(32000, "RecdData");
                                //Clients.Add(new ClientFileTransfer(32000, RecdData));
                            }
                            else
                            {
                                UserAuthenticated = false;
                                var DataGram = Encoding.ASCII.GetBytes("ACCESS DENIED");
                                m_client.Send(DataGram, DataGram.Length, m_endPoint);
                            }
                        }

                    }
                }, null);

                // if (UserAuthenticated == false ) break;

                if (WaitHandle.WaitAny(new[] { m_stop, res.AsyncWaitHandle }) == 0)
                {
                    break;
                }
            }
        }


        private bool VerifyUserAccessToWrite(string data)
        {
            return true;
        }


        private void NewClientConnects(int PortNumber)
        {
            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, PortNumber);
            UdpClient listenClient = new UdpClient(listenEndPoint);

            Random _r = new Random();
            int n = _r.Next();

            string fullFilePathName = @"C:\OpenJobs\Houston Open Jobs\12345678\" + n.ToString() + "thisistest2.txt"; //  @"C:\" + @_FileData.RootFolder + @"\" + _FileData.SubFolder + @"\" + _FileData.FileName;
            var directoryname = Path.GetDirectoryName(fullFilePathName);
            System.IO.Directory.CreateDirectory(directoryname);
            FileStream temfile = new FileStream(@fullFilePathName, FileMode.Create, FileAccess.Write);

            while (true)
            {
                try
                {
                    Byte[] data = listenClient.Receive(ref listenEndPoint);
                    string message = Encoding.ASCII.GetString(data);

                    Debug.Write(message);
                    temfile.Write(data, 0, data.Length);
                    temfile.Close();
                    temfile = new FileStream(@fullFilePathName, FileMode.Append, FileAccess.Write);
                }
                catch (Exception c)
                {

                    Debug.Write(c.Message);
                }
            }
        }


        private string GetOpenPort()
        {
            int PortStartIndex = 2000;
            int PortEndIndex = 64000;
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();

            List<int> usedPorts = tcpEndPoints.Select(p => p.Port).ToList<int>();
            int unusedPort = 0;

            for (int port = PortStartIndex; port < PortEndIndex; port++)
            {
                if (!usedPorts.Contains(port))
                {
                    unusedPort = port;
                    break;
                }
            }
            return unusedPort.ToString();
        }

        private string GetOpenPort(int higherlevel)
        {
            const int PortStartIndex = 1000;
            const int PortEndIndex = 2000;
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
            IEnumerable<int> query =
                  (from n in tcpEndPoints.OrderBy(n => n.Port)
                   where (n.Port >= PortStartIndex) && (n.Port <= PortEndIndex)
                   select n.Port).ToArray().Distinct();

            int i = PortStartIndex;
            foreach (int p in query)
            {
                if (p != i)
                {
                    break;
                }
                i++;
            }
            return i > PortEndIndex ? "0" : i.ToString();
        }

    }
}