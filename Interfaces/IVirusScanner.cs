using System.Threading.Tasks;
using FileGuardian.Models;

namespace FileGuardian.Interfaces
{
    public interface IVirusScanner
    {
        Task<VirusScanResult> ScanAsync(string path);
    }
}