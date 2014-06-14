using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace LzDeploy
{
    public class ClientDeploy : Deploy
    {
        private string _awsAccessKey;
        private string _awsSecretKey;
        private string _awsCloudFrontDistributionId;

        public ClientDeploy()
        {
            LoadAwsKeys();
        }

        private void LoadAwsKeys()
        {
            if (!File.Exists("AwsKey.txt"))
                return;

            var keyMap = File.ReadAllLines("AwsKey.txt")
                .Where(e => e.Contains('='))
                .Select(e => e.Split('='))
                .ToDictionary(e => e[0].Trim(), e => e[1].Trim());

            keyMap.TryGetValue("AccessKey", out _awsAccessKey);
            keyMap.TryGetValue("SecretKey", out _awsSecretKey);
            keyMap.TryGetValue("DistributionId", out _awsCloudFrontDistributionId);
        }

        protected override void ExecuteInternal(string history)
        {
            Log(Color.Yellow, " * Build Start.");
            var buildResult = BuildProject(FindFile("Lz.sln"), "Client|Any CPU");

            Log(Color.Cyan, 4, buildResult.Stdout.Trim());
            Log(Color.Yellow, " * Build Ok.");

            Log(Color.Yellow, " * Write History.");
            File.WriteAllText("UpdateHistory.txt", history, Encoding.UTF8);
            Log(Color.Yellow, " * Write History Ok.");

            foreach (var each in File.ReadAllLines("deploy_client.list").Where(e => !string.IsNullOrWhiteSpace(e)))
            {
                var pair = each.Split(',');
                var srcPath = ReplaceMacro(pair[0]);
                var destPath = ReplaceMacro(pair[1]);
                
                CopyFiles(srcPath, destPath);
            }
            Log(Color.Yellow, " * Copy Ok.");

            HashHelper.MakeHash("client");

            Log(Color.Yellow, " * Upload Start.");
            foreach (var progress in UploadFile("client", "lzclient", _awsAccessKey, _awsSecretKey))
                Log(Color.Cyan, 4, progress);
            Log(Color.Yellow, " * Upload Ok.");
        }
    }
}
