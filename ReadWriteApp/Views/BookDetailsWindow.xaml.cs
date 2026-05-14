using System.Windows;
using System.Windows.Input;
using ReadWriteApp.Data;
using ReadWriteApp.Models;
using ReadWriteApp.Services;
using ReadWriteApp.Services.Interfaces;

namespace ReadWriteApp.Views
{
    public partial class BookDetailsWindow : Window
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private Book _book;

        public BookDetailsWindow(Book book)
        {
            InitializeComponent();
            _bookService = new BookService();
            _authorService = new AuthorService();
            _book = book;

            LoadBookDetails();
            CheckEditPermissions();
        }

        private void LoadBookDetails()
        {
            BookTitle.Text = _book.Title;
            BookGenre.Text = _book.GenresDisplay;
            BookDescription.Text = _book.Description;
            BookContent.Text = _book.Content;
            PublishDate.Text = $"Опубликовано: {_book.PublishedDate:dd.MM.yyyy}";

            var author = _authorService.GetAuthorById(_book.AuthorId);
            AuthorName.Text = author != null ? $"✍ {author.FullName}" : "Автор неизвестен";
        }

        private void CheckEditPermissions()
        {
            var currentUser = DataStore.CurrentUser;
            if (currentUser != null && currentUser.Role == UserRole.Author
                && currentUser.AuthorId == _book.AuthorId)
            {
                EditButton.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
            }
        }

        private void AuthorName_Click(object sender, MouseButtonEventArgs e)
        {
            var author = _authorService.GetAuthorById(_book.AuthorId);
            if (author != null)
            {
                var profileWindow = new AuthorProfileWindow(author);
                profileWindow.Owner = this;
                profileWindow.ShowDialog();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new AddEditBookWindow(_book);
            editWindow.Owner = this;
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                var updatedBook = _bookService.GetBookById(_book.Id);
                if (updatedBook != null)
                {
                    _book = updatedBook;
                    LoadBookDetails();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Вы действительно хотите удалить книгу \"{_book.Title}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _bookService.DeleteBook(_book.Id);
                this.DialogResult = true;
                this.Close();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
