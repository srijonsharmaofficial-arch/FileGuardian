using System;
using System.Collections.Generic;
using System.IO;

namespace FileGuardian.Services
{
    public class OrganizerService
    {
        private static readonly Dictionary<string, string> CategoryMap = new Dictionary<string, string>
        {
            { ".pdf", "PDF" },

            { ".doc", "Word" }, { ".docx", "Word" },
            { ".xls", "Excel" }, { ".xlsx", "Excel" },
            { ".ppt", "PowerPoint" }, { ".pptx", "PowerPoint" },

            { ".jpg", "Images" }, { ".jpeg", "Images" }, { ".png", "Images" }, { ".gif", "Images" }, { ".bmp", "Images" },

            { ".mp4", "Videos" }, { ".mov", "Videos" }, { ".avi", "Videos" }, { ".mkv", "Videos" },

            { ".mp3", "Music" }, { ".wav", "Music" }, { ".flac", "Music" },

            { ".exe", "Application" }, { ".msi", "Application" },

            { ".zip", "Archive" }, { ".rar", "Archive" }, { ".7z", "Archive" },

            { ".txt", "Text" }
        };

        public string GetCategory(string extension)
        {
            string ext = extension.ToLowerInvariant();
            return CategoryMap.ContainsKey(ext) ? CategoryMap[ext] : "Other";
        }

        public int OrganizeFiles(string targetBaseFolder, List<string> filePaths, out List<string> errors)
        {
            errors = new List<string>();
            int movedCount = 0;

            foreach (string filePath in filePaths)
            {
                try
                {
                    string ext = Path.GetExtension(filePath);
                    string category = GetCategory(ext);
                    string categoryFolder = Path.Combine(targetBaseFolder, category);

                    if (!Directory.Exists(categoryFolder))
                        Directory.CreateDirectory(categoryFolder);

                    string fileName = Path.GetFileName(filePath);
                    string destinationPath = GetUniqueDestination(Path.Combine(categoryFolder, fileName));

                    File.Move(filePath, destinationPath);
                    movedCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"{Path.GetFileName(filePath)}: {ex.Message}");
                }
            }

            return movedCount;
        }

        private string GetUniqueDestination(string path)
        {
            if (!File.Exists(path))
                return path;

            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            int counter = 1;
            string newPath;

            do
            {
                newPath = Path.Combine(dir, $"{name} ({counter}){ext}");
                counter++;
            } while (File.Exists(newPath));

            return newPath;
        }
    }
}