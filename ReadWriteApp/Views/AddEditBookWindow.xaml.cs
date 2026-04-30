using System.Windows;
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
            GenreComboBox.Text = book.Genre;
            DescriptionTextBox.Text = book.Description;
            ContentTextBox.Text = book.Content;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сохранить"
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text.Trim();
            string genre = GenreComboBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            string content = ContentTextBox.Text.Trim();

            // Валидация обязательных полей
            if (string.IsNullOrWhiteSpace(title))
            {
                ErrorText.Text = "Введите название книги";
                return;
            }

            if (string.IsNullOrWhiteSpace(genre))
            {
                ErrorText.Text = "Укажите жанр книги";
                return;
            }

            if (_editingBook != null)
            {
                // Редактирование существующей книги
                _bookService.UpdateBook(_editingBook.Id, title, genre, description, content);
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

                _bookService.AddBook(title, currentUser.AuthorId.Value, genre, description, content);
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
