using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace Common
{
    public class ClientManager
    {
        public string IP { get; set; }
        public int Port { get; set; }
        private TcpClient client;
        private NetworkStream stream;
        public delegate void ReceiveDataEventHandler(byte[] data);
        public event ReceiveDataEventHandler OnReviceData;

        public ClientManager(string ip, int port)
        {
            Port = port;
            IP = ip;
            client = new TcpClient();
        }

        public void Connect()
        {
            try
            {
                client.Connect(new IPEndPoint(IPAddress.Parse(IP), Port));
                stream = client.GetStream();
                Thread thread = new Thread(new ThreadStart(ReciveServerData));
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error: {0}", ex.Message));
            }
        }

        private void ReciveServerData()
        {
            try
            {
                byte[] dataBuffer;
                int bufferSize;
                while (true)
                {
                    dataBuffer = new byte[1048];
                    bufferSize = stream.Read(dataBuffer, 0, dataBuffer.Length);
                    if (OnReviceData != null) OnReviceData(dataBuffer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error: {0}", ex.Message));
            }
        }

        public void Send(byte[] data)
        {
            stream.Write(data,0, data.Length);
            stream.Flush();
        }
    }
}
