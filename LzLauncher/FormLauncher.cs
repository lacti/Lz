using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LzDeploy;
using System.Diagnostics;

namespace LzLauncher
{
    public partial class FormLauncher : Form
    {
        private const string UrlBase = "http://lzclient.s3-ap-northeast-1.amazonaws.com/";

        public FormLauncher()
        {
            InitializeComponent();
        }

        private void FormLauncher_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => ReadUpdateHistory())
                .ContinueWith(task => textHistory.Text = task.Result, TaskScheduler.FromCurrentSynchronizationContext());

            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        PatchClient();
                    }
                    catch (Exception exception)
                    {
                        return exception;
                    }
                    return null;

                }).ContinueWith(task =>
                    {
                        var exception = task.Result;
                        if (exception == null)
                            buttonStart.Enabled = true;
                        else
                        {
                            labelStatus.Text = exception.Message;
                            buttonStart.Enabled = false;

                            MessageBox.Show(this, exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private string ReadUpdateHistory()
        {
            try
            {
                var webClient = new WebClient();
                var history = webClient.DownloadString(UrlBase + "UpdateHistory.txt");
                return history;
            }
            catch
            {
            }
            return "";
        }

        private string GetLocalRootPath()
        {
            var localRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                         "LzClient");
            if (!Directory.Exists(localRoot))
                Directory.CreateDirectory(localRoot);

            return localRoot;
        }

        private void PatchClient()
        {
            var localRoot = GetLocalRootPath();

            var localHashFilePath = Path.Combine(localRoot, HashHelper.HashFileName);
            var localHashContent = File.Exists(localHashFilePath)
                                       ? File.ReadAllText(localHashFilePath, Encoding.UTF8)
                                       : "";

            var webClient = new WebClient();
            var cloudHashContent = webClient.DownloadString(UrlBase + HashHelper.HashFileName);
            var diff = HashHelper.MakeDiff(HashHelper.ParseFilesHashTable(cloudHashContent),
                                           HashHelper.ParseFilesHashTable(localHashContent));

            var totalUpdateCount = diff.ChangedFileList.Count + diff.DeletedFileList.Count;
            if (totalUpdateCount == 0)
            {
                UpdateMaxValueOfProcessBar(1);
                UpdateCurrentValueOfProcessBar(1);
            }
            else
            {
                UpdateMaxValueOfProcessBar(diff.ChangedFileList.Count + diff.DeletedFileList.Count);

                Func<string, string> toCloudPath = localPath =>
                                                   localPath.Replace(Path.DirectorySeparatorChar.ToString(), "_$FOLDER$_");

                var updateCount = 0;
                foreach (var each in diff.ChangedFileList)
                {
                    var url = UrlBase + toCloudPath(each);
                    var localPath = Path.Combine(localRoot, each);
                    CreateDirectory(Path.GetDirectoryName(localPath));

                    webClient.DownloadFile(url, localPath);
                    UpdateStatus("Update: " + each);
                    UpdateCurrentValueOfProcessBar(++updateCount);
                }

                foreach (var each in diff.DeletedFileList)
                {
                    File.Delete(Path.Combine(localRoot, each));
                    UpdateStatus("Delete: " + each);
                    UpdateCurrentValueOfProcessBar(++updateCount);
                }
            }

            UpdateStatus("Complete.");
        }

        private void CreateDirectory(string directory)
        {
            CreateDirectory(new DirectoryInfo(directory));
        }

        private void CreateDirectory(DirectoryInfo dirInfo)
        {
            if (dirInfo.Parent != null && !dirInfo.Parent.Exists) CreateDirectory(dirInfo.Parent);
            if (!dirInfo.Exists) dirInfo.Create();
        }

        private void UpdateUi(Action action)
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(() => UpdateUi(action)));
            else action();
        }

        private void UpdateStatus(string message)
        {
            UpdateUi(() => labelStatus.Text = message);
        }

        private void UpdateMaxValueOfProcessBar(int maxValue)
        {
            UpdateUi(() => progressUpdate.Maximum = maxValue);
        }

        private void UpdateCurrentValueOfProcessBar(int currentValue)
        {
            UpdateUi(() => progressUpdate.Value = currentValue);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            var localRoot = GetLocalRootPath();
            var executablePath = Path.Combine(localRoot, "LzClient.exe");

            if (!File.Exists(executablePath))
            {
                MessageBox.Show(this, "Cannot find a client binary.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonStart.Enabled = false;
                return;
            }

            Process.Start(executablePath);
            Close();
        }
    }
}
