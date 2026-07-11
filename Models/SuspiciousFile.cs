using System;

namespace FileGuardian.Models
{
    public class SuspiciousFile : FileItem
    {
        public string Reason { get; set; }

        public SuspiciousFile(
            string name,
            string fullPath,
            string extension,
            long sizeInBytes,
            DateTime dateCreated,
            DateTime dateModified,
            string reason)
            : base(name, fullPath, extension, sizeInBytes, dateCreated, dateModified)
        {
            Reason = reason;
        }

        public override string GetSummary()
        {
            return base.GetSummary() + $" ⚠ Flagged: {Reason}";
        }
    }
}