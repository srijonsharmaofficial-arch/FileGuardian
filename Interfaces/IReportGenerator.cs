namespace FileGuardian.Interfaces
{
    public interface IReportGenerator
    {
        string GenerateReport(string title, string content);
        void SaveToFile(string filePath, string reportText);
    }
}