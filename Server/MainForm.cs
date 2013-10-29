using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Server
{
    public partial class MainForm : Form
    {
        System.Threading.Timer timer;
        List<Common.Message> log = new List<Common.Message>();
        
        ServerManager server;
        public MainForm()
        {
            InitializeComponent();
            timer = new System.Threading.Timer(UpdateLog, this, 1000, 250);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            server = new ServerManager(Port.Value);
            server.Start();
            server.OnClientConnect += ConnectClient;
            server.OnReviceData += ReviceData;
            ServerMsg("Server start");
        }

        private void ServerMsg(string p)
        {
            Common.Message m = new Common.Message() { User = "Server", Text = p };
            AddToLog(m);
        }

        private void AddToLog(Common.Message m)
        {
            log.Add(m);
        }

        void ReviceData(int clientId, byte[] data)
        {
            string msg = Tools.GetString(data);
            AddToLog(new Common.Message(msg));
            server.SendDataToAllClientExpect(clientId, data);
        }

        private static void UpdateLog(Object stateInfo)
        {
            MainForm form = (MainForm)stateInfo;
            form.UpdateLog();
        }

        public void UpdateLog()
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(UpdateLog), new object[] { });
                return;
            }
            textBox1.Clear();
            String s = "";
            foreach (Common.Message item in log)
            {
                s += item.ToString();
            }
            textBox1.Text = s;
        }

        void ConnectClient(int clientId)
        {
            String msg = clientId.ToString();
            Common.Message m = new Common.Message() { User = "Server", Text = "New user add" };
            AddToLog(m);
            foreach (Common.Message item in log)
            {
                server.SendDataToClient(clientId, Tools.GetBytes(item.GetJSONString()));
            }
            server.SendDataToAllClientExpect(clientId, Tools.GetBytes(m.GetJSONString()));
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                server.Stop();
            }
            catch (Exception)
            {
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Common.Message m = new Common.Message() { User = "Server", Text = textBox2.Text };
            ServerMsg(textBox2.Text);
            server.SendDataToAllClient(Tools.GetBytes(m.GetJSONString()));
        }

    }
}
