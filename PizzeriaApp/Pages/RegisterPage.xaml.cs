using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PizzeriaApp.Pages
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = NameTextBox.Text.Trim();
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string repeatPassword = RepeatPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            if (password.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать минимум 4 символа.");
                return;
            }

            if (password != repeatPassword)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            if (Core.Context.Users.Any(u => u.Login == login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует.");
                return;
            }

            Users user = new Users
            {
                FullName = fullName,
                Login = login,
                PasswordHash = Core.HashPassword(password)
            };

            Core.Context.Users.Add(user);
            Core.Context.SaveChanges();

            MessageBox.Show("Регистрация завершена. Теперь войдите.");
            NavigationService.Navigate(new LoginPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}
