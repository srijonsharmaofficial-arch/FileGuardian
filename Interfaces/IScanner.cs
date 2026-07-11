using FileGuardian.Models;

namespace FileGuardian.Interfaces
{
    public interface IScanner
    {
        FolderItem Scan(string rootPath);
    }
}