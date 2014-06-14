using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LzDeploy
{
    public partial class FormDeploy : Form
    {
        public FormDeploy()
        {
            InitializeComponent();
        }

        private void buttonDeploy_Click(object sender, EventArgs e)
        {
            var history = textHistory.Text.Trim();
            buttonDeployClient.Enabled = false;
            Task.Factory.StartNew(() =>
            {
                var deploy = new ClientDeploy();

                deploy.OnLogReceived += deploy_OnLogReceived;
                return deploy.Execute(history);
            }).ContinueWith(_ =>
            {
                buttonDeployClient.Enabled = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void deploy_OnLogReceived(Color color, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => deploy_OnLogReceived(color, message)));
            }
            else
            {
                var timestamp = string.Format("[{0}] ", DateTime.Now.ToString("MM-dd HH:mm:ss"));
                textLog.AppendText(timestamp, Color.WhiteSmoke);
                textLog.AppendText(message + "\r\n", color);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textLog.Clear();
        }

        private void FormDeploy_Load(object sender, EventArgs e)
        {
            if (File.Exists("UpdateHistory.txt"))
                textHistory.Text = File.ReadAllText("UpdateHistory.txt", Encoding.UTF8);
        }
    }

    internal static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}
