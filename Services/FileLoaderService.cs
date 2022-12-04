using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace DataAccess.Services
{
    public class FileLoaderService
    {
        string uploadPath;

        EncryptionService encryptionService;

        public FileLoaderService(IWebHostEnvironment env, EncryptionService encryption)
        {
            uploadPath = Path.Combine(env.WebRootPath, "Manuals");
            encryptionService = encryption;
        }
        
        /*
         * Using memory stream and filestream to write file
         * Requires IFormFile (file) as parameter
         */
        public async Task SaveFile(IFormFile file)
        {
            byte[] fileContents;

            using (MemoryStream stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                fileContents = stream.ToArray();
            }

            byte[] encryptedContents = encryptionService.EncryptByteArray(fileContents); 
            using (MemoryStream dataStream = new MemoryStream(encryptedContents))
            {
                string targetFile = Path.Combine(uploadPath, file.FileName);

                using (FileStream fileStream = new FileStream(targetFile, FileMode.Create))
                {
                    dataStream.WriteTo(fileStream);
                }
            }
        }

        /**
         * Loading file from directory, requires filename as parameter
         */
        public async Task<FileInfo> LoadFile(string fileName)
        {
            DirectoryInfo directory = new DirectoryInfo(uploadPath);
            if (directory.EnumerateFiles().Any(c=>c.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)) == false)
            {
                return null;
            }

            return directory.EnumerateFiles().Where(c => c.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        /**
         * Returning byte array containing file information
         */
        public async Task<byte[]> ReadFileIntoMemory(string fileName)
        {
            var file = await LoadFile(fileName);

            if (file==null)
            {
                return null;
            }

            using (var memStream = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(file.FullName))
                {
                    fileStream.CopyTo(memStream);
                    return memStream.ToArray();
                }
            }
        }

        /**
         * Loading encrypted file as byte array
         */
        public async Task<byte[]> LoadEncryptedFile(string fileName)
        {
            var encryptedFile = await ReadFileIntoMemory(fileName);

            if (encryptedFile == null || encryptedFile.Length == 0)
            {
                return null;
            }

            var decryptedData = encryptionService.DecryptByteArray(encryptedFile);

            return decryptedData;
        }
    }
}
