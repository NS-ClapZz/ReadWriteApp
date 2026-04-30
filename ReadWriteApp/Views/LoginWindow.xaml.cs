using System.Windows;
using ReadWriteApp.Models;
using ReadWriteApp.Services;
using ReadWriteApp.Services.Interfaces;

namespace ReadWriteApp.Views
{
    /// <summary>
    /// Окно авторизации и регистрации пользователей
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IUserService _userService;

        public LoginWindow()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Войти"
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ErrorText.Text = "Введите логин и пароль";
                return;
            }

            bool success = _userService.Login(login, password);

            if (success)
            {
                // Открываем главное окно и закрываем окно входа
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ErrorText.Text = "Неверный логин или пароль";
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Зарегистрироваться"
        /// </summary>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ErrorText.Text = "Введите логин и пароль для регистрации";
                return;
            }

            if (password.Length < 3)
            {
                ErrorText.Text = "Пароль должен содержать минимум 3 символа";
                return;
            }

            // Определяем роль по выбранному пункту ComboBox
            UserRole role = RoleComboBox.SelectedIndex == 1 ? UserRole.Author : UserRole.Reader;

            bool success = _userService.Register(login, password, role);

            if (success)
            {
                // Автоматически входим после регистрации
                _userService.Login(login, password);
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ErrorText.Text = "Пользователь с таким логином уже существует";
            }
        }
    }
}
