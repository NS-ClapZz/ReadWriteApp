using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ReadWriteApp.Data;
using ReadWriteApp.Models;
using ReadWriteApp.Services;
using ReadWriteApp.Services.Interfaces;
using ReadWriteApp.Views;

namespace ReadWriteApp
{
    public partial class MainWindow : Window
    {
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly IAuthorService _authorService;

        public MainWindow()
        {
            InitializeComponent();
            _bookService = new BookService();
            _userService = new UserService();
            _authorService = new AuthorService();

            LoadUserInfo();
            LoadGenres();
            LoadBooks();
        }

        private void LoadUserInfo()
        {
            var user = _userService.GetCurrentUser();
            if (user != null)
            {
                string roleName = user.Role == UserRole.Author ? "Автор" : "Читатель";
                UserInfoText.Text = $"Вы вошли как: {user.Login} ({roleName})";

                if (user.Role == UserRole.Author)
                {
                    AddBookButton.Visibility = Visibility.Visible;
                    ProfileButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void LoadGenres()
        {
            GenreComboBox.Items.Clear();
            GenreComboBox.Items.Add("Все жанры");

            var genres = _bookService.GetAllGenres();
            foreach (var genre in genres)
            {
                GenreComboBox.Items.Add(genre);
            }
            GenreComboBox.SelectedIndex = 0;
        }

        private void LoadBooks()
        {
            string searchQuery = SearchTextBox?.Text ?? "";
            string selectedGenre = GenreComboBox?.SelectedItem?.ToString() ?? "Все жанры";

            var books = _bookService.GetAllBooks();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                books = _bookService.SearchBooks(searchQuery);
            }

            if (selectedGenre != "Все жанры" && !string.IsNullOrWhiteSpace(selectedGenre))
            {
                books = books.Where(b => b.Genres.Contains(selectedGenre)).ToList();
            }

            BooksListBox.ItemsSource = books;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadBooks();
        }

        private void GenreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadBooks();
        }

        private void BooksListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (BooksListBox.SelectedItem is Book selectedBook)
            {
                var detailsWindow = new BookDetailsWindow(selectedBook);
                detailsWindow.Owner = this;
                detailsWindow.ShowDialog();

                LoadGenres();
                LoadBooks();
            }
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditBookWindow();
            addWindow.Owner = this;
            addWindow.ShowDialog();

            LoadGenres();
            LoadBooks();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var user = _userService.GetCurrentUser();
            if (user != null && user.AuthorId.HasValue)
            {
                var author = _authorService.GetAuthorById(user.AuthorId.Value);
                if (author != null)
                {
                    var profileWindow = new AuthorProfileWindow(author);
                    profileWindow.Owner = this;
                    profileWindow.ShowDialog();
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _userService.Logout();
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}