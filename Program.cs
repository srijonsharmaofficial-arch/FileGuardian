using System;
using System.IO;
using System.Windows;

namespace FileGuardian
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }
            catch (Exception ex)
            {
                string logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "FileGuardian_CrashLog.txt");

                File.WriteAllText(logPath, ex.ToString());

                MessageBox.Show(
                    "FileGuardian crashed on startup. Details were saved to:\n" + logPath,
                    "Startup Error");
            }
        }
    }
}