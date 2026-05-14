using System.Windows;
using ReadWriteApp.Data;
using ReadWriteApp.Models;
using ReadWriteApp.Services;
using ReadWriteApp.Services.Interfaces;

namespace ReadWriteApp.Views
{
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

        private void LoadAuthorData()
        {
            FirstNameTextBox.Text = _author.FirstName;
            LastNameTextBox.Text = _author.LastName;
            BioTextBox.Text = _author.Bio;

            var books = _bookService.GetBooksByAuthor(_author.Id);
            AuthorBooksListBox.ItemsSource = books;
        }

        private void CheckEditPermissions()
        {
            var currentUser = DataStore.CurrentUser;

            if (currentUser != null && currentUser.AuthorId == _author.Id)
            {
                SaveButton.Visibility = Visibility.Visible;
                FirstNameTextBox.IsReadOnly = false;
                LastNameTextBox.IsReadOnly = false;
                BioTextBox.IsReadOnly = false;
            }
            else
            {
                FirstNameTextBox.IsReadOnly = true;
                LastNameTextBox.IsReadOnly = true;
                BioTextBox.IsReadOnly = true;
                FirstNameTextBox.Background = System.Windows.Media.Brushes.WhiteSmoke;
                LastNameTextBox.Background = System.Windows.Media.Brushes.WhiteSmoke;
                BioTextBox.Background = System.Windows.Media.Brushes.WhiteSmoke;
            }
        }

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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
