using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace TadrousManassa.Areas.Teacher.Models
{
    public class S3Uploader
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Uploader(IAmazonS3 s3Client, string bucketName)
        {
            _s3Client = s3Client;
            _bucketName = bucketName;
        }

        public async Task<string> UploadToS3Async(Stream fileStream, string objectName)
        {
            // Ensure the stream is at the beginning
            if (fileStream.CanSeek)
            {
                fileStream.Seek(0, SeekOrigin.Begin);
            }

            var fileTransferUtility = new TransferUtility(_s3Client);

            // Upload the file stream to the specified bucket and key (objectName)
            await fileTransferUtility.UploadAsync(fileStream, _bucketName, objectName);

            // Construct the public URL for the uploaded object
            return $"https://{_bucketName}.s3.amazonaws.com/{objectName}";
        }
    }

}
