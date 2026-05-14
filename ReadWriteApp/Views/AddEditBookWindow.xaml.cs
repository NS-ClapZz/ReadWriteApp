using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ReadWriteApp.Data;
using ReadWriteApp.Models;
using ReadWriteApp.Services;
using ReadWriteApp.Services.Interfaces;

namespace ReadWriteApp.Views
{
    public partial class AddEditBookWindow : Window
    {
        private readonly IBookService _bookService;
        private readonly Book? _editingBook;

        public AddEditBookWindow()
        {
            InitializeComponent();
            _bookService = new BookService();
            _editingBook = null;

            LoadGenresList();
        }

        public AddEditBookWindow(Book book) : this()
        {
            _editingBook = book;
            Title = "Редактировать книгу";
            WindowTitle.Text = "✏️ Редактирование книги";

            TitleTextBox.Text = book.Title;
            DescriptionTextBox.Text = book.Description;
            ContentTextBox.Text = book.Content;

            SelectBookGenres(book.Genres);
        }

        private void LoadGenresList()
        {
            var genres = _bookService.GetAllGenres();
            GenresListBox.ItemsSource = genres;
        }

        private void SelectBookGenres(List<string> bookGenres)
        {
            GenresListBox.SelectedItems.Clear();

            for (int i = 0; i < GenresListBox.Items.Count; i++)
            {
                string genre = GenresListBox.Items[i].ToString()!;
                if (bookGenres.Contains(genre))
                {
                    GenresListBox.SelectedItems.Add(GenresListBox.Items[i]);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            string content = ContentTextBox.Text.Trim();

            var selectedGenres = GenresListBox.SelectedItems
                .Cast<string>()
                .ToList();

            if (string.IsNullOrWhiteSpace(title))
            {
                ErrorText.Text = "Введите название книги";
                return;
            }

            if (selectedGenres.Count == 0)
            {
                ErrorText.Text = "Выберите хотя бы один жанр";
                return;
            }

            if (_editingBook != null)
            {
                _bookService.UpdateBook(_editingBook.Id, title, selectedGenres, description, content);
            }
            else
            {
                var currentUser = DataStore.CurrentUser;
                if (currentUser == null || !currentUser.AuthorId.HasValue)
                {
                    ErrorText.Text = "Ошибка: пользователь не авторизован как автор";
                    return;
                }

                _bookService.AddBook(title, currentUser.AuthorId.Value, selectedGenres, description, content);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
