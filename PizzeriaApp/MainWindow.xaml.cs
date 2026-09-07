using System.Windows;

namespace PizzeriaApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Pages.LoginPage());
        }
    }
}
