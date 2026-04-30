using System.Windows;
using ReadWriteApp.Data;
using ReadWriteApp.Models;
using ReadWriteApp.Services;
using ReadWriteApp.Services.Interfaces;

namespace ReadWriteApp.Views
{
    /// <summary>
    /// Окно профиля автора — просмотр и редактирование данных
    /// </summary>
    public partial class AuthorProfileWindow : Window
    {
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;
        private readonly Author _author;

        public AuthorProfileWindow(Author author)
        {
            InitializeComponent();
            _authorService = new AuthorService();
            _bookService = new BookService();
            _author = author;

            LoadAuthorData();
            CheckEditPermissions();
        }

        /// <summary>
        /// Загружает данные автора в форму
        /// </summary>
        private void LoadAuthorData()
        {
            FirstNameTextBox.Text = _author.FirstName;
            LastNameTextBox.Text = _author.LastName;
            BioTextBox.Text = _author.Bio;

            // Загружаем список книг автора
            var books = _bookService.GetBooksByAuthor(_author.Id);
            AuthorBooksListBox.ItemsSource = books;
        }

        /// <summary>
        /// Проверяет, может ли текущий пользователь редактировать профиль
        /// </summary>
        private void CheckEditPermissions()
        {
            var currentUser = DataStore.CurrentUser;

            if (currentUser != null && currentUser.AuthorId == _author.Id)
            {
                // Автор может редактировать свой профиль
                SaveButton.Visibility = Visibility.Visible;
                FirstNameTextBox.IsReadOnly = false;
                LastNameTextBox.IsReadOnly = false;
                BioTextBox.IsReadOnly = false;
            }
            else
            {
                // Другие пользователи могут только просматривать
                FirstNameTextBox.IsReadOnly = true;
                LastNameTextBox.IsReadOnly = true;
                BioTextBox.IsReadOnly = true;
                FirstNameTextBox.Background = System.Windows.Media.Brushes.WhiteSmoke;
                LastNameTextBox.Background = System.Windows.Media.Brushes.WhiteSmoke;
                BioTextBox.Background = System.Windows.Media.Brushes.WhiteSmoke;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сохранить"
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string bio = BioTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(firstName))
            {
                MessageBox.Show("Имя не может быть пустым", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _authorService.UpdateAuthor(_author.Id, firstName, lastName, bio);

            MessageBox.Show("Профиль успешно обновлён", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Закрыть"
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
