using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace FTBAPISERVER.Application
{
    class ClientFileTransfer

    {
        private int _PortNumber;
        private string _FileData;
        private FileStream tempfile;
        private string fullFilePathName;
        private bool m_started = false;
        IPEndPoint m_endPoint;
        //private ServiceMode m_mode = 0;
        public ClientFileTransfer(int PortNumber, String filedata)
        {
            _PortNumber = PortNumber;
            _FileData = filedata;
            fullFilePathName = @"C:\OpenJobs\Houston Open Jobs\12345678\this is test.txt"; //  @"C:\" + @_FileData.RootFolder + @"\" + _FileData.SubFolder + @"\" + _FileData.FileName;
            var directoryname = Path.GetDirectoryName(fullFilePathName);
            System.IO.Directory.CreateDirectory(directoryname);
            //tempfile = new FileStream(@fullFilePathName, FileMode.Create, FileAccess.Write);
            m_endPoint = new IPEndPoint(IPAddress.Any, 32000);
            OpenConnections();
        }

        private void OpenAndWriteFIle()
        {

        }
        private void OpenConnections()
        {
            int localPort = _PortNumber;
            IPAddress tempAddress;
            IPAddress.TryParse("OUT_GOING_IP/HOST_GOES_HERE", out tempAddress);
            // Create UDP client
            UdpClient client = new UdpClient(localPort);
            tempfile = new FileStream(@fullFilePathName, FileMode.Create, FileAccess.Write);
            tempfile.Close();
            while (true)
            {
                IPEndPoint remoteSender = new IPEndPoint(IPAddress.Any, localPort);
                Byte[] receiveBytes = client.Receive(ref remoteSender);

                udpState state = new udpState(client, remoteSender);
                state.setRemote(remoteSender);
                client.BeginReceive(new AsyncCallback(DataReceivedClient), state);
            }
            client.Close();
            tempfile.Close();
            //Dispose();
        }
        private void DataReceivedClient(IAsyncResult ar)
        {

            UdpClient c = (UdpClient)((udpState)ar.AsyncState).c;
            IPEndPoint ipEndPoint = (IPEndPoint)((udpState)(ar.AsyncState)).e; //local ip and random port.
            IPEndPoint remoteIPEndPoint = (IPEndPoint)((udpState)(ar.AsyncState)).remote;
            byte[] receiveBytes = c.EndReceive(ar, ref ipEndPoint);
            //string receivedText = BitConverter.ToString(receiveBytes);
            string receivedPacket = System.Text.Encoding.UTF8.GetString(receiveBytes);
            var DataGram = Encoding.ASCII.GetBytes("OK");
            c.Send(DataGram, DataGram.Length, ipEndPoint);
            if (receivedPacket == "\r\n.\r\n")
            {
                tempfile.Close();
                return;
            }
            tempfile.Write(receiveBytes, 0, receiveBytes.Length);
            tempfile.Close();
            tempfile = new FileStream(@fullFilePathName, FileMode.Append, FileAccess.Write);
            //System.IO.MemoryStream ms = new MemoryStream();// receiveBytes);
            // ms.Write(receiveBytes, 0, receiveBytes.Length);
            //if (tempfile.CanWrite)
            //ms.WriteTo(tempfile);
            //file.Close();
            //ms.Close();
            c.BeginReceive(new AsyncCallback(DataReceivedClient), ar.AsyncState);
        }

    }
    public class udpState
    {
        public udpState(UdpClient c, IPEndPoint e)
        {
            this.c = c;
            this.e = e;
        }

        internal void setRemote(IPEndPoint remote)
        {
            this.remote = remote;
        }

        public UdpClient c;
        public IPEndPoint e;
        public IPEndPoint remote;
    }
}