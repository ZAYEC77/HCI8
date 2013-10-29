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
    public class ServerManager
    {
        public int Port { get; set; }
        TcpListener listener;
        List<TcpClient> clients;
        List<NetworkStream> streams;
        public delegate void ReceiveDataEventHandler(int clientId, byte[] data);
        public delegate void ConnectEventHandler(int clientId);
        public event ReceiveDataEventHandler OnReviceData;
        public event ConnectEventHandler OnClientConnect;

        public ServerManager(int port)
        {
            Port = port;
            listener = new TcpListener(new IPEndPoint(IPAddress.Any, Port));
            clients = new List<TcpClient>();
            streams = new List<NetworkStream>();
        }

        public void Start()
        {
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(ClientConnect), null);
        }

        public void Stop()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
                streams[i].Close();
            }
            listener.Stop();
        }

        public void SendDataToAllClient(byte[] data)
        {
            for (int i = 1; i <= clients.Count; i++)
            {
                SendDataToClient(i, data);
            }
        }

        public void SendDataToAllClientExpect(int clientId, byte[] data)
        {
            for (int i = 1; i <= clients.Count; i++)
            {
                if (i == clientId) continue;
                SendDataToClient(i, data);
            }
        }

        public void SendDataToClient(int clientId, byte[] data)
        {
            int clientIdx = clientId - 1;
            streams[clientIdx].Write(data, 0, data.Length);
            streams[clientIdx].Flush();
        }

        private void ClientConnect(IAsyncResult ar)
        {
            try
            {
                TcpClient client = listener.EndAcceptTcpClient(ar);
                clients.Add(client);
                streams.Add(client.GetStream());

                if (OnClientConnect != null) OnClientConnect(clients.Count);
                Thread thread = new Thread(new ParameterizedThreadStart(ClientReviseData));
                thread.Start(clients.Count);
                listener.BeginAcceptTcpClient(new AsyncCallback(ClientConnect), null);

            }
            catch (Exception)
            {
            }
        }

        private void ClientReviseData(object client)
        {
            int clientId = (int)client;
            int clientIdx = clientId - 1;
            byte[] dataBuffer;
            int bufferSize;
            while (true)
            {
                dataBuffer = new byte[1048];
                bufferSize = streams[clientIdx].Read(dataBuffer, 0, dataBuffer.Length);
                Array.Resize(ref dataBuffer, bufferSize);
                if (OnReviceData != null) OnReviceData(clientId, dataBuffer);
            }
        }
    }
}
