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
    /// <summary>
    /// Окно добавления и редактирования книги
    /// </summary>
    public partial class AddEditBookWindow : Window
    {
        private readonly IBookService _bookService;
        private readonly Book? _editingBook;

        /// <summary>
        /// Конструктор для добавления новой книги
        /// </summary>
        public AddEditBookWindow()
        {
            InitializeComponent();
            _bookService = new BookService();
            _editingBook = null;

            LoadGenresList();
        }

        /// <summary>
        /// Конструктор для редактирования существующей книги
        /// </summary>
        public AddEditBookWindow(Book book) : this()
        {
            _editingBook = book;
            Title = "Редактировать книгу";
            WindowTitle.Text = "✏️ Редактирование книги";

            // Заполняем поля данными книги
            TitleTextBox.Text = book.Title;
            DescriptionTextBox.Text = book.Description;
            ContentTextBox.Text = book.Content;

            // Выделяем жанры, которые уже есть у книги
            SelectBookGenres(book.Genres);
        }

        /// <summary>
        /// Загружает список всех жанров в ListBox
        /// </summary>
        private void LoadGenresList()
        {
            var genres = _bookService.GetAllGenres();
            GenresListBox.ItemsSource = genres;
        }

        /// <summary>
        /// Выделяет жанры книги в ListBox при редактировании
        /// </summary>
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

        /// <summary>
        /// Обработчик нажатия кнопки "Сохранить"
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            string content = ContentTextBox.Text.Trim();

            // Собираем выбранные жанры
            var selectedGenres = GenresListBox.SelectedItems
                .Cast<string>()
                .ToList();

            // Валидация обязательных полей
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
                // Редактирование существующей книги
                _bookService.UpdateBook(_editingBook.Id, title, selectedGenres, description, content);
            }
            else
            {
                // Добавление новой книги
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

        /// <summary>
        /// Обработчик нажатия кнопки "Отмена"
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
