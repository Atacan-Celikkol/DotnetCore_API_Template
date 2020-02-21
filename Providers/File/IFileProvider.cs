using System;
using System.Threading.Tasks;

namespace Providers.File
{
    public interface IFileProvider
    {
        Task<Uri> SaveFileAsync(string folder, string path, byte[] bytes);

        Task<bool> DeleteFileAsync(string folder, string path);

        Task<byte[]> GetFileBytesAsync(string folder, string path);

        Task<bool> FileExistsAsync(string folder, string path);

        Task<Uri> CopyFileAsync(string sourceFolder, string sourcePath, string targetFoler, string targetPath);
    }
}