namespace FileGuardian.Models
{
    public class VirusScanResult
    {
        public bool ScanSucceeded { get; set; }
        public bool ThreatsFound { get; set; }
        public string Summary { get; set; }
        public string RawOutput { get; set; }
    }
}