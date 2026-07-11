using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using FileGuardian.Services;

namespace FileGuardian
{
    public partial class OrganizerWindow : Window
    {
        private readonly OrganizerService _organizerService = new OrganizerService();
        private readonly DuplicateFinderService _duplicateFinder = new DuplicateFinderService(new HashService());
        private readonly List<string> _currentFiles = new List<string>();

        public OrganizerWindow()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog
            {
                Title = "Select a folder to organize"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                SelectedPathTextBox.Text = dialog.FolderName;
                _currentFiles.Clear();
                _currentFiles.AddRange(Directory.GetFiles(dialog.FolderName));
                RefreshFileLists();
            }
        }

        private void AttachMoreFilesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Attach additional files",
                Filter = "All Files (*.*)|*.*",
                Multiselect = true
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                foreach (string file in dialog.FileNames)
                {
                    if (!_currentFiles.Contains(file))
                        _currentFiles.Add(file);
                }
                RefreshFileLists();
            }
        }

        private void RefreshFileLists()
        {
            FilesListBox.Items.Clear();
            foreach (string file in _currentFiles)
                FilesListBox.Items.Add(Path.GetFileName(file));

            FileTypesListBox.Items.Clear();
            var groupedByExtension = _currentFiles
                .Select(f => string.IsNullOrEmpty(Path.GetExtension(f)) ? "(no extension)" : Path.GetExtension(f).ToLowerInvariant())
                .GroupBy(ext => ext)
                .OrderByDescending(g => g.Count());

            foreach (var group in groupedByExtension)
                FileTypesListBox.Items.Add($"{group.Key}  —  {group.Count()} file(s)");

            bool hasFiles = _currentFiles.Count > 0;
            OrganizeFilesButton.IsEnabled = hasFiles;
            OrganizeStatusText.Text = "";
        }

        private void OrganizeFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentFiles.Count == 0)
                return;

            List<List<string>> duplicateGroups = _duplicateFinder.FindDuplicateGroups(_currentFiles);

            if (duplicateGroups.Count > 0)
            {
                int totalDuplicateFiles = duplicateGroups.Sum(group => group.Count - 1);

                string preview = string.Join("\n", duplicateGroups.Select(group =>
                    $"- {Path.GetFileName(group[0])}  ({group.Count} copies)"));

                bool deleteConfirmed = ConfirmDialogWindow.Ask(
                    this,
                    "Duplicate Files Found",
                    $"Found {duplicateGroups.Count} duplicate group(s), {totalDuplicateFiles} file(s) that can be removed:\n\n{preview}\n\n" +
                    "Delete the extra copies (keeping one of each)?");

                if (deleteConfirmed)
                {
                    foreach (List<string> group in duplicateGroups)
                    {
                        for (int i = 1; i < group.Count; i++)
                        {
                            try
                            {
                                File.Delete(group[i]);
                                _currentFiles.Remove(group[i]);
                            }
                            catch
                            {
                                // If a file can't be deleted (in use, permissions), skip it
                            }
                        }
                    }
                }
            }

            if (_currentFiles.Count == 0)
            {
                RefreshFileLists();
                return;
            }

            string targetFolder = Directory.Exists(SelectedPathTextBox.Text)
                ? SelectedPathTextBox.Text
                : Path.GetDirectoryName(_currentFiles[0]);

            bool organizeConfirmed = ConfirmDialogWindow.Ask(
                this,
                "Confirm Organize",
                $"This will move {_currentFiles.Count} file(s) into category folders inside:\n{targetFolder}\n\nContinue?");

            if (!organizeConfirmed)
                return;

            int movedCount = _organizerService.OrganizeFiles(targetFolder, _currentFiles, out List<string> errors);

            if (errors.Count > 0)
            {
                OrganizeStatusText.Text =
                    $"⚠ {errors.Count} file(s) could not be moved:\n" +
                    string.Join("\n", errors);
            }
            else
            {
                OrganizeStatusText.Text = "";
            }

            _currentFiles.Clear();
            RefreshFileLists();
        }
    }
}