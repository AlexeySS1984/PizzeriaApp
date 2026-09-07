using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PizzeriaApp.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.");
                return;
            }

            string hash = Core.HashPassword(password);

            Users user = Core.Context.Users
                .FirstOrDefault(u => u.Login == login && u.PasswordHash == hash);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.");
                return;
            }

            Core.CurrentUser = user;
            NavigationService.Navigate(new CombosPage());
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}
