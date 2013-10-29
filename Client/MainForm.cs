using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;

namespace Client
{
    public partial class MainForm : Form
    {
        System.Threading.Timer timer;
        ClientManager client;
        List<Common.Message> log = new List<Common.Message>();
        public MainForm()
        {
            InitializeComponent();
            timer = new System.Threading.Timer(UpdateLog, this, 1000, 250);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client = new ClientManager(textBox1.Text, Port.Value);
            client.OnReviceData += ReviceData;
            client.Connect();
        }

        void ReviceData(byte[] data)
        {
            string msg = Tools.GetString(data);
            char[] p = { '}' };
            string[] s = msg.Split(p);
            for (int i = 0; i < s.Length-1; i++)
            {
                AddToLog(new Common.Message(s[i]+"}"));                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Common.Message m = new Common.Message() { Text = textBox2.Text, User = textBox3.Text };
            string s = m.GetJSONString();
            client.Send(Tools.GetBytes(s));
            MyMsg(textBox2.Text);
            
        }

        private void MyMsg(string p)
        {
            Common.Message m = new Common.Message() { User = textBox3.Text, Text = p };
            AddToLog(m);
        }

        private void AddToLog(Common.Message m)
        {
            log.Add(m);
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
            textBox4.Clear();
            String s = "";
            foreach (Common.Message item in log)
            {
                s += item.ToString();
            }
            textBox4.Text = s;
        }

    }
}
