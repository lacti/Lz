using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace LzDeploy
{
    public abstract class Deploy
    {
        public delegate void LogReceived(Color color, string message);

        public event LogReceived OnLogReceived;

        private readonly Dictionary<string, string> _macroMap = new Dictionary<string, string>();

        protected Deploy()
        {
            BuildMacroMap();
        }

        #region Macro

        private void BuildMacroMap()
        {
            _macroMap.Add("PROJECT_ROOT", FindProjectRoot());
        }

        private static string FindProjectRoot()
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            while (currentPath != null)
            {
                if (Directory.GetFiles(currentPath, "*.sln").Length > 0)
                    return currentPath;
                currentPath = Path.GetDirectoryName(currentPath);
            }
            return ".";
        }

        protected string ReplaceMacro(string source)
        {
            foreach (var pair in _macroMap)
            {
                var key = string.Format("$({0})", pair.Key);
                if (source.Contains(key))
                    source = source.Replace(key, pair.Value);
            }
            return source;
        }

        #endregion

        public bool Execute(string history)
        {
            try
            {
                ExecuteInternal(history);
                Log(Color.White, " * Complete.");
                return true;
            }
            catch (ProcessException exception)
            {
                Log(Color.Red, exception.Message);
                Log(Color.Red, exception.StackTrace);
                Log(Color.Gray, exception.Result.Stdout);
                Log(Color.Gray, exception.Result.Stderr);
            }
            catch (Exception exception)
            {
                Log(Color.Red, exception.Message);
                Log(Color.Red, exception.StackTrace);
            }
            Log(Color.Red, " * Error.");
            return false;
        }

        protected abstract void ExecuteInternal(string history);

        protected void Log(Color color, string format, params object[] args)
        {
            Log(color, 0, format, args);
        }

        protected void Log(Color color, int level, string format, params object[] args)
        {
            var message = args != null && args.Length > 0 ? string.Format(format, args) : format;
            if (level == 0)
                FireLogReceived(color, message);
            else
            {
                var space = new string(' ', level);
                foreach (var line in message.Split('\r', '\n').Where(line => !string.IsNullOrWhiteSpace(line)))
                {
                    FireLogReceived(color, space + line.Trim());
                }
            }
        }

        protected void FireLogReceived(Color color, string message)
        {
            var handler = OnLogReceived;
            if (handler != null) handler(color, message);
        }

        protected static ProcessResult BuildProject(string solutionPath, string configuration)
        {
            const string defaultDevenvPath = @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.com";
            var devenvPath = defaultDevenvPath;
            var devenvPathInEnv = Environment.GetEnvironmentVariable("VS110COMNTOOLS");
            if (!string.IsNullOrWhiteSpace(devenvPathInEnv))
                devenvPath = Path.GetFullPath(Path.Combine(devenvPathInEnv, "..", "IDE", "devenv.com"));

            if (!File.Exists(devenvPath))
                throw new InvalidOperationException("Cannot find VS110 devenv.com");

            return ExecuteProcess(devenvPath, string.Format(@"{0} /build ""{1}""", solutionPath, configuration), "Build Error");
        }

        protected static void CopyFiles(string src, string destPath)
        {
            var srcPath = Path.GetDirectoryName(src);
            var srcPattern = Path.GetFileName(src);

            if (srcPath == null || srcPattern == null)
                throw new InvalidOperationException("Invalid Source: " + src);

            if (!string.IsNullOrEmpty(srcPath) && !Directory.Exists(srcPath))
                throw new InvalidOperationException("No Source Directory: " + srcPath);

            CreateDirectory(new DirectoryInfo(destPath));
            var srcDirectory = srcPath.Length > 0 ? srcPath : ".";
            foreach (var srcFile in Directory.GetFiles(srcDirectory, srcPattern))
            {
                var destFile = srcPath.Length > 0
                                   ? srcFile.Replace(srcPath, destPath)
                                   : Path.Combine(destPath, srcFile);
                CopyFile(srcFile, destFile);
            }
        }

        protected static void CopyFile(string srcFile, string destFile)
        {
            var srcFileInfo = new FileInfo(srcFile);
            var destFileInfo = new FileInfo(destFile);
            if (!srcFileInfo.Exists)
                throw new InvalidOperationException("Cannot find the file: " + srcFile);

            if (destFileInfo.Exists
                && srcFileInfo.Length == destFileInfo.Length
                && Math.Abs(srcFileInfo.LastWriteTime.ToFileTime() - destFileInfo.LastWriteTime.ToFileTime()) < 1000)
                return;

            File.Copy(srcFile, destFile, true);
        }

        protected static ProcessResult ExecuteProcessRedirect(string fileName, string argument, string errorMessage = "")
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = argument;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                var result = new ProcessResult { Stdout = process.StandardOutput.ReadToEnd() };
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    result.Stderr = process.StandardError.ReadToEnd();
                    throw new ProcessException(errorMessage, result);
                }
                return result;
            }
        }

        protected static ProcessResult ExecuteProcess(string fileName, string argument, string errorMessage = "")
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = argument;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                process.Start();

                var result = new ProcessResult { Stdout = "Ok." };
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    result.Stderr = "Error!";
                    throw new ProcessException(errorMessage, result);
                }
                return result;
            }
        }

        protected static void CreateDirectory(DirectoryInfo directory)
        {
            if (directory.Parent != null) CreateDirectory(directory.Parent);
            if (!directory.Exists) directory.Create();
        }

        protected static void ModifyXml(string xmlPath, string xpath, string newValue)
        {
            var doc = new XmlDocument();
            doc.Load(xmlPath);

            var node = doc.SelectSingleNode(xpath) as XmlAttribute;
            if (node != null)
            {
                node.Value = newValue;
                doc.Save(xmlPath);
            }
        }

        protected static string FindFile(string fileName)
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            while (currentPath != null)
            {
                var filePath = Path.Combine(currentPath, fileName);
                if (File.Exists(filePath))
                    return filePath;

                currentPath = Path.GetDirectoryName(currentPath);
            }
            throw new FileNotFoundException(fileName);
        }

        protected static IEnumerable<string> UploadFile(string uploadPath, string bucketName, string awsAccessKey, string awsSecretKey, string cloudFrontDistributionId = null)
        {
            if (!uploadPath.EndsWith("\\"))
                uploadPath += "\\";

            using (var aws = new AwsHelper(awsAccessKey, awsSecretKey))
            {
                var uploadFileList = Directory.GetFiles(uploadPath, "*.*", SearchOption.AllDirectories).Select(e => e.Substring(uploadPath.Length)).ToList();
                var deleteFileList = new List<string>();

                var uploadFilesHashTableFile = Path.Combine(uploadPath, HashHelper.HashFileName);
                if (File.Exists(uploadFilesHashTableFile))
                {
                    var localHashTable = HashHelper.ParseFilesHashTable(File.ReadAllText(uploadFilesHashTableFile));
                    string cloudHashTableContent;
                    if (aws.TryToGetObjectContent(bucketName, HashHelper.HashFileName, out cloudHashTableContent))
                    {
                        var cloudHashTable = HashHelper.ParseFilesHashTable(cloudHashTableContent);
                        var diff = HashHelper.MakeDiff(localHashTable, cloudHashTable);
                        uploadFileList.RemoveAll(e => diff.SameFileList.Contains(e));
                        deleteFileList.AddRange(diff.DeletedFileList);
                    }
                }

                foreach (var file in uploadFileList)
                {
                    aws.UploadFile(bucketName, Path.Combine(uploadPath, file), AwsHelper.ToCloudPath(file));
                    yield return file;
                }

                foreach (var file in deleteFileList)
                {
                    aws.DeleteFile(bucketName, AwsHelper.ToCloudPath(file));
                    yield return file;
                }

                if (!string.IsNullOrWhiteSpace(cloudFrontDistributionId))
                {
                    aws.InvalidationCloudFront(cloudFrontDistributionId, uploadFileList.Concat(deleteFileList).Select(AwsHelper.ToCloudPath).ToList());
                }
            }
        }
    }

    public class ProcessException : InvalidOperationException
    {
        public ProcessResult Result { get; private set; }

        public ProcessException(string message, ProcessResult result)
            : base(message)
        {
            Result = result;
        }
    }

    public class ProcessResult
    {
        public string Stdout { get; set; }
        public string Stderr { get; set; }
    }
}
