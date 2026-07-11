using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FileGuardian.Interfaces;
using FileGuardian.Models;

namespace FileGuardian.Services
{
    public class DefenderVirusScanner : IVirusScanner
    {
        public async Task<VirusScanResult> ScanAsync(string path)
        {
            string defenderPath = FindDefenderExecutable();

            if (defenderPath == null)
            {
                return new VirusScanResult
                {
                    ScanSucceeded = false,
                    ThreatsFound = false,
                    Summary = "Windows Defender was not found on this system.",
                    RawOutput = ""
                };
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = defenderPath,
                Arguments = $"-Scan -ScanType 3 -File \"{path}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();

                    await Task.Run(() => process.WaitForExit());

                    bool threatsFound = output.IndexOf("Threat", StringComparison.OrdinalIgnoreCase) >= 0
                                         && output.IndexOf("found", StringComparison.OrdinalIgnoreCase) >= 0;

                    return new VirusScanResult
                    {
                        ScanSucceeded = true,
                        ThreatsFound = threatsFound,
                        Summary = threatsFound
                            ? "Windows Defender flagged one or more threats."
                            : "No threats detected.",
                        RawOutput = output + error
                    };
                }
            }
            catch (Exception ex)
            {
                return new VirusScanResult
                {
                    ScanSucceeded = false,
                    ThreatsFound = false,
                    Summary = $"Scan could not run: {ex.Message}",
                    RawOutput = ""
                };
            }
        }

        private string FindDefenderExecutable()
        {
            string classicPath = Environment.ExpandEnvironmentVariables(
                @"%ProgramFiles%\Windows Defender\MpCmdRun.exe");

            if (File.Exists(classicPath))
                return classicPath;

            string platformRoot = Environment.ExpandEnvironmentVariables(
                @"%ProgramData%\Microsoft\Windows Defender\Platform");

            if (Directory.Exists(platformRoot))
            {
                foreach (string versionFolder in Directory.GetDirectories(platformRoot))
                {
                    string candidate = Path.Combine(versionFolder, "MpCmdRun.exe");
                    if (File.Exists(candidate))
                        return candidate;
                }
            }

            return null;
        }
    }
}