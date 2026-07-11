namespace FileGuardian.Interfaces
{
    public interface IHashGenerator
    {
        string ComputeHash(string filePath);
    }
}