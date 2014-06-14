using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LzDeploy
{
    public static class HashHelper
    {
        public const string HashFileName = "files.hash";

        public class HashInfo : IEquatable<HashInfo>
        {
            public string FilePath { get; set; }
            public long Length { get; set; }
            public string HashValue { get; set; }

            public bool Equals(HashInfo other)
            {
                return FilePath == other.FilePath && Length == other.Length && HashValue == other.HashValue;
            }

            public override string ToString()
            {
                return string.Join(",", FilePath, Length, HashValue);
            }

            public static HashInfo Parse(string line)
            {
                var triple = line.Split(',');
                return new HashInfo
                    {
                        FilePath = triple[0],
                        Length = long.Parse(triple[1]),
                        HashValue = triple[2]
                    };
            }
        }

        public class HashDiff
        {
            public readonly List<string> SameFileList = new List<string>();
            public readonly List<string> ChangedFileList = new List<string>();
            public readonly List<string> DeletedFileList = new List<string>();
        }

        public static void MakeHash(string targetPath)
        {
            if (!targetPath.EndsWith("\\"))
                targetPath += "\\";

            var result = Directory.GetFiles(targetPath, "*.*", SearchOption.AllDirectories)
                                  .Select(each => new HashInfo
                                      {
                                          FilePath = each.Substring(targetPath.Length),
                                          Length = new FileInfo(each).Length,
                                          HashValue = Hash(each)
                                      }).ToList();
            File.WriteAllLines(Path.Combine(targetPath, HashFileName), result.Select(e => e.ToString()).ToArray(), Encoding.UTF8);
        }

        public static string Hash(string filePath)
        {
            var result = new StringBuilder();
            var fileBytes = File.ReadAllBytes(filePath);
            var hashBytes = (new MD5CryptoServiceProvider()).ComputeHash(fileBytes);

            foreach (var each in hashBytes)
                result.Append(each.ToString("X2"));

            return result.ToString();
        }

        public static IEnumerable<HashInfo> ParseFilesHashTable(string filesContent)
        {
            return filesContent.Split('\r', '\n').Where(e => !string.IsNullOrWhiteSpace(e)).Select(HashInfo.Parse);
        }

        public static HashDiff MakeDiff(IEnumerable<HashInfo> leftTable, IEnumerable<HashInfo> rightTable)
        {
            var diff = new HashDiff();
            var leftTableMap = leftTable.ToDictionary(e => e.FilePath);
            diff.ChangedFileList.AddRange(leftTableMap.Keys);

            foreach (var rightEntry in rightTable)
            {
                if (leftTableMap.ContainsKey(rightEntry.FilePath))
                {
                    var localEntry = leftTableMap[rightEntry.FilePath];
                    if (localEntry.Equals(rightEntry))
                    {
                        diff.SameFileList.Add(localEntry.FilePath);
                        diff.ChangedFileList.Remove(localEntry.FilePath);
                    }
                    else diff.ChangedFileList.Add(localEntry.FilePath);
                }
                else diff.DeletedFileList.Add(rightEntry.FilePath);
            }
            return diff;
        }
    }
}
