using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileGuardian
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void OrganizerButton_Click(object sender, RoutedEventArgs e)
        {
            OrganizerWindow organizerWindow = new OrganizerWindow();
            organizerWindow.Show();
            this.Close();
        }

        public void ForensicsButton_Click(object sender, RoutedEventArgs e)
        {
            ForensicsWindow forensicsWindow = new ForensicsWindow();
            forensicsWindow.Show();
            this.Close();
        }
    }
}