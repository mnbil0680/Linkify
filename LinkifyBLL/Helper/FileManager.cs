using Microsoft.AspNetCore.Http;

namespace SempaBLL.Helper
{
    public static class FileManager
    {
        public static string UploadFile(string FolderName, IFormFile File)
        {
            try
            {
                // Restrict to specific file types
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" , ".mp4"};
                var extension = Path.GetExtension(File.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    return "Only PDF, JPG, PNG and MP4 files are allowed.";
                }

                // Limit file size to 10MB
                if (File.Length > 80 * 1024 * 1024)
                {
                    return "File size must be under 80MB.";
                }

                // Create folder (same as original)
                string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FolderName);
                Directory.CreateDirectory(FolderPath);

                // Generate safe filename
                string safeFileName = Path.GetFileName(File.FileName);
                string FileName = Guid.NewGuid() + Path.GetExtension(safeFileName);
                string FinalPath = Path.Combine(FolderPath, FileName);

                using (var Stream = new FileStream(FinalPath, FileMode.Create))
                {
                    File.CopyTo(Stream);
                }

                return $"/{FolderName}/{FileName}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static string RemoveFile(string FolderName, string fileName)
        {
            try
            {
                // Remove leading / if present
                if (fileName.StartsWith("/"))
                {
                    fileName = fileName.Substring(1);
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return "File Deleted";
                }
                return "File Not Found";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
