using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace WalkChat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.thread = new Thread(new ThreadStart(this.UdpRead));
            this.thread.Start();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                // Send datagram
                UdpClient udpSendClient = new UdpClient();
                udpSendClient.Connect(IPAddress.Parse("6.6.6.255"), 6661);
//                udpSendClient.Connect(IPAddress.Parse("127.0.0.1"), 6661);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(tbMsg.Text);
                udpSendClient.Send(sendBytes, sendBytes.Length);
                udpSendClient.Close();
            }
            catch (Exception ex)
            {
                AppendText("E:" + ex.ToString());
            }

            // Add to log
            tbLog.Text += "\r\n" + "T:" + tbMsg.Text;
            tbMsg.Text = "";
        }

        private Thread thread = null;
        private UdpClient udpReadClient = null;
        private void UdpRead()
        {
            try
            {
                udpReadClient = new UdpClient(6661);
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 6661);
                while (true)
                {
                    Byte[] receiveBytes = udpReadClient.Receive(ref RemoteIpEndPoint);
                    this.AppendText("R:" + Encoding.ASCII.GetString(receiveBytes));
                }
            }
            catch (Exception ex)
            {
                AppendText("E:" + ex.ToString());
            }
        }
        delegate void SetTextCallback(string text);
        private void AppendText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.tbLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(AppendText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.tbLog.Text += "\r\n" + text;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.udpReadClient.Close();
        }
    }
}
