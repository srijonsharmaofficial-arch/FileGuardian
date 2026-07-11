using System;

namespace FileGuardian.Models
{
    public abstract class FileSystemItem
    {
        private string _name;
        private string _fullPath;

        public string Name
        {
            get => _name;
            protected set => _name = value;
        }

        public string FullPath
        {
            get => _fullPath;
            protected set => _fullPath = value;
        }

        public DateTime DateCreated { get; protected set; }
        public DateTime DateModified { get; protected set; }

        protected FileSystemItem(string name, string fullPath, DateTime dateCreated, DateTime dateModified)
        {
            Name = name;
            FullPath = fullPath;
            DateCreated = dateCreated;
            DateModified = dateModified;
        }

        public abstract string GetSummary();
    }
}