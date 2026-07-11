using System;
using System.Collections.Generic;

namespace FileGuardian.Models
{
    public class FolderItem : FileSystemItem
    {
        public List<FileItem> Files { get; private set; }
        public List<FolderItem> SubFolders { get; private set; }

        public FolderItem(string name, string fullPath, DateTime dateCreated, DateTime dateModified)
            : base(name, fullPath, dateCreated, dateModified)
        {
            Files = new List<FileItem>();
            SubFolders = new List<FolderItem>();
        }

        public long GetTotalSize()
        {
            long total = 0;
            foreach (var file in Files)
                total += file.SizeInBytes;

            foreach (var subFolder in SubFolders)
                total += subFolder.GetTotalSize();

            return total;
        }

        public override string GetSummary()
        {
            return $"{Name} - {Files.Count} files, {SubFolders.Count} subfolders";
        }
    }
}