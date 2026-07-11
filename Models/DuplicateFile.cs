using System.Collections.Generic;

namespace FileGuardian.Models
{
    public class DuplicateFile
    {
        public string Hash { get; private set; }
        public List<FileItem> Matches { get; private set; }

        public DuplicateFile(string hash)
        {
            Hash = hash;
            Matches = new List<FileItem>();
        }

        public long WastedBytes()
        {
            if (Matches.Count <= 1) return 0;
            return Matches[0].SizeInBytes * (Matches.Count - 1);
        }
    }
}