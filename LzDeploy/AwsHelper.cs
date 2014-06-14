using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Amazon;
using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using Amazon.S3;
using Amazon.S3.Model;

namespace LzDeploy
{
    public class AwsHelper : IDisposable
    {
        private readonly string _awsAccessKey;
        private readonly string _awsSecretKey;

        public AwsHelper(string awsAccessKey, string awsSecretKey)
        {
            _awsAccessKey = awsAccessKey;
            _awsSecretKey = awsSecretKey;

            _s3Client = new Lazy<AmazonS3>(() => AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey), true);
            _cloudFront = new Lazy<AmazonCloudFront>(() => AWSClientFactory.CreateAmazonCloudFrontClient(_awsAccessKey, _awsSecretKey), true);

        }

        private readonly Lazy<AmazonS3> _s3Client;
        private readonly Lazy<AmazonCloudFront> _cloudFront;

        public AmazonS3 S3 { get { return _s3Client.Value; } }
        public AmazonCloudFront CloudFront { get { return _cloudFront.Value; } }

        public bool TryToGetObjectContent(string bucketName, string key, out string content)
        {
            content = "";
            try
            {
                var filesObject = S3.GetObject(new GetObjectRequest
                    {
                        BucketName = bucketName,
                        Key = key
                    });

                using (var reader = new StreamReader(filesObject.ResponseStream))
                {
                    content = reader.ReadToEnd();
                    return true;
                }
            }
            catch (AmazonS3Exception exception)
            {
                if (exception.StatusCode == HttpStatusCode.NotFound)
                    return false;

                throw;
            }
        }

        public void Dispose()
        {
            if (_s3Client.IsValueCreated)
                _s3Client.Value.Dispose();

            if (_cloudFront.IsValueCreated)
                _cloudFront.Value.Dispose();
        }

        public void UploadFile(string bucketName, string localPath, string cloudPath)
        {
            var putObjectRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    FilePath = localPath,
                    Key = cloudPath,
                    Timeout = -1,
                    ReadWriteTimeout = 300000
                };

            // make public
            putObjectRequest.AddHeader("x-amz-acl", "public-read");

            S3.PutObject(putObjectRequest);
        }

        public void DeleteFile(string bucketName, string cloudPath)
        {
            S3.DeleteObject(new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = cloudPath,
                    Timeout = -1
                });
        }

        public void InvalidationCloudFront(string distributionId, IEnumerable<string> cloudPaths)
        {
            var cloudPathList = cloudPaths as List<string> ?? cloudPaths.ToList();
            CloudFront.CreateInvalidation(new CreateInvalidationRequest
                {
                    DistributionId = distributionId,
                    InvalidationBatch = new InvalidationBatch
                    {
                        CallerReference = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss ", System.Globalization.CultureInfo.InvariantCulture) + "GMT",
                        Paths = new Paths
                        {
                            Quantity = cloudPathList.Count,
                            Items = cloudPathList.Select(e => e.StartsWith("/")? e : "/" + e).ToList()
                        }
                    }
                });
        }

        public static string ToCloudPath(string localPath)
        {
            return localPath.Replace(Path.DirectorySeparatorChar.ToString(), "_$FOLDER$_");
        }

        public static string ToLocalPath(string cloudPath)
        {
            return cloudPath.Replace("_$FOLDER$_", Path.DirectorySeparatorChar.ToString());
        }
    }
}
