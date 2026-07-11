using System.Windows;

namespace FileGuardian
{
    public partial class ConfirmDialogWindow : Window
    {
        public bool Result { get; private set; } = false;

        public ConfirmDialogWindow(string title, string message)
        {
            InitializeComponent();
            TitleText.Text = title;
            MessageText.Text = message;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        public static bool Ask(Window owner, string title, string message)
        {
            ConfirmDialogWindow dialog = new ConfirmDialogWindow(title, message);
            dialog.Owner = owner;
            dialog.ShowDialog();
            return dialog.Result;
        }
    }
}