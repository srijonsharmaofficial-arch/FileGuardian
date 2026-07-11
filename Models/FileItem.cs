using System;

namespace FileGuardian.Models
{
    public class FileItem : FileSystemItem
    {
        public string Extension { get; private set; }
        public long SizeInBytes { get; private set; }
        public string Hash { get; set; }

        public FileItem(
            string name,
            string fullPath,
            string extension,
            long sizeInBytes,
            DateTime dateCreated,
            DateTime dateModified)
            : base(name, fullPath, dateCreated, dateModified)
        {
            Extension = extension;
            SizeInBytes = sizeInBytes;
        }

        public double SizeInMB => Math.Round(SizeInBytes / (1024.0 * 1024.0), 2);

        public override string GetSummary()
        {
            return $"{Name} ({Extension}) - {SizeInMB} MB - Modified: {DateModified:yyyy-MM-dd}";
        }
    }
}