using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using FileGuardian.Interfaces;
using FileGuardian.Models;
using FileGuardian.Services;

namespace FileGuardian
{
    public partial class ForensicsWindow : Window
    {
        private readonly IVirusScanner _virusScanner = new DefenderVirusScanner();

        private static readonly string[] RiskyExtensions =
            { ".exe", ".bat", ".cmd", ".ps1", ".vbs" };

        public ForensicsWindow()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select a file to investigate",
                Filter = "All Files (*.*)|*.*"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                SelectedPathTextBox.Text = dialog.FileName;
                ShowFileDetails(dialog.FileName);
                ScanVirusButton.IsEnabled = true;
            }
        }

        private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog
            {
                Title = "Select a folder to investigate"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                SelectedPathTextBox.Text = dialog.FolderName;
                ShowFolderDetails(dialog.FolderName);
                ScanVirusButton.IsEnabled = true;
            }
        }

        private void ShowFileDetails(string filePath)
        {
            FileInfo info = new FileInfo(filePath);

            NameValueText.Text = info.Name;
            PathValueText.Text = info.FullName;
            ExtValueText.Text = string.IsNullOrEmpty(info.Extension) ? "File (no extension)" : info.Extension;
            SizeValueText.Text = $"{Math.Round(info.Length / 1024.0, 2)} KB";
            CreatedValueText.Text = info.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
            ModifiedValueText.Text = info.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

            CheckSuspicious(new[] { filePath });
        }

        private void ShowFolderDetails(string folderPath)
        {
            DirectoryInfo info = new DirectoryInfo(folderPath);

            int fileCount = info.GetFiles("*", SearchOption.AllDirectories).Length;
            int folderCount = info.GetDirectories("*", SearchOption.AllDirectories).Length;
            long totalBytes = GetDirectorySize(info);

            NameValueText.Text = info.Name;
            PathValueText.Text = info.FullName;
            ExtValueText.Text = $"Folder ({fileCount} files, {folderCount} subfolders)";
            SizeValueText.Text = $"{Math.Round(totalBytes / 1024.0 / 1024.0, 2)} MB";
            CreatedValueText.Text = info.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
            ModifiedValueText.Text = info.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

            string[] allFiles = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
            CheckSuspicious(allFiles);
        }

        private void CheckSuspicious(string[] filePaths)
        {
            SuspiciousFindingsListBox.Items.Clear();

            int flaggedCount = 0;

            foreach (string path in filePaths)
            {
                string ext = Path.GetExtension(path).ToLowerInvariant();
                if (Array.IndexOf(RiskyExtensions, ext) >= 0)
                {
                    SuspiciousFindingsListBox.Items.Add($"{Path.GetFileName(path)}  ({ext})");
                    flaggedCount++;
                }
            }

            if (flaggedCount == 0)
            {
                SuspiciousFindingsListBox.Items.Add("No suspicious file types detected.");
                PossibleSolutionsText.Text = "No action needed — no risky file types were found.";
            }
            else
            {
                PossibleSolutionsText.Text =
                    $"{flaggedCount} file(s) use extensions commonly linked to scripts or executables " +
                    "(.exe, .bat, .cmd, .ps1, .vbs). These aren't automatically harmful, but if you don't " +
                    "recognize them, avoid running them and use the virus scan below to check them, or " +
                    "delete them if you're not sure where they came from.";
            }
        }

        private long GetDirectorySize(DirectoryInfo folder)
        {
            long total = 0;

            foreach (FileInfo file in folder.GetFiles())
                total += file.Length;

            foreach (DirectoryInfo sub in folder.GetDirectories())
                total += GetDirectorySize(sub);

            return total;
        }

        private async void ScanVirusButton_Click(object sender, RoutedEventArgs e)
        {
            ScanVirusButton.IsEnabled = false;
            VirusScanStatusText.Text = "Scanning... this may take a moment.";

            string path = SelectedPathTextBox.Text;
            VirusScanResult result = await _virusScanner.ScanAsync(path);

            if (!result.ScanSucceeded)
            {
                VirusScanStatusText.Text = $"⚠ {result.Summary}";
            }
            else if (result.ThreatsFound)
            {
                VirusScanStatusText.Text = $"🛑 {result.Summary}";
            }
            else
            {
                VirusScanStatusText.Text = $"✅ {result.Summary}";
            }

            ScanVirusButton.IsEnabled = true;
        }
    }
}