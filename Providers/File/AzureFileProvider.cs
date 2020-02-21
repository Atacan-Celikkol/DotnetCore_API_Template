using Core.Extensions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Providers.File
{
    public class AzureFileProvider : IFileProvider
    {
        protected static CloudBlobClient BlobClient { get; set; }

        public AzureFileProvider(string connString)
        {
            if (string.IsNullOrEmpty(connString))
            {
                throw new ArgumentNullException(nameof(connString));
            }
            var storageAccount = CloudStorageAccount.Parse(connString
                );

            BlobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<Uri> SaveFileAsync(string folder, string path, byte[] bytes)
        {
            var extension = Path.GetExtension(path);
            var mimeType = MimeTypeMap.GetMimeType(extension);
            var containerBlob = BlobClient.GetContainerReference(folder);

            var blockBlob = containerBlob.GetBlockBlobReference(path);
            await blockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
            if (!string.IsNullOrEmpty(mimeType))
            {
                blockBlob.Properties.ContentType = mimeType;
                await blockBlob.SetPropertiesAsync();
            }
            return blockBlob.Uri;
        }

        public async Task<bool> DeleteFileAsync(string folder, string path)
        {
            var contReference = BlobClient.GetContainerReference(folder);
            var blob = contReference.GetBlockBlobReference(path);

            return await blob.DeleteIfExistsAsync();
        }

        public async Task<byte[]> GetFileBytesAsync(string folder, string path)
        {
            var containerBlob = BlobClient.GetContainerReference(folder);
            var blob = containerBlob.GetBlockBlobReference(path);

            using (var stream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(stream);

                return stream.ToArray();
            }
        }

        public async Task<bool> FileExistsAsync(string folder, string path)
        {
            return await BlobClient.GetContainerReference(folder).GetBlockBlobReference(path).ExistsAsync();
        }

        public async Task<Uri> CopyFileAsync(string sourceFolder, string sourcePath, string targetFolder, string targetPath)
        {
            var sourceContainer = BlobClient.GetContainerReference(sourceFolder);
            var targetContainer = BlobClient.GetContainerReference(targetFolder);
            var sourceBlob = sourceContainer.GetBlockBlobReference(sourcePath);
            var targetBlob = targetContainer.GetBlockBlobReference(targetPath);
            await targetBlob.StartCopyAsync(sourceBlob);
            return targetBlob.CopyState.Status == CopyStatus.Success ? targetBlob.Uri : null;
        }
    }
}