using System.Collections.Generic;
using ReadWriteApp.Models;

namespace ReadWriteApp.Services.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для работы с книгами
    /// </summary>
    public interface IBookService
    {
        /// <summary>
        /// Добавить новую книгу в каталог
        /// </summary>
        void AddBook(string title, int authorId, List<string> genres, string description, string content);

        /// <summary>
        /// Получить список всех книг
        /// </summary>
        List<Book> GetAllBooks();

        /// <summary>
        /// Найти книгу по идентификатору
        /// </summary>
        Book? GetBookById(int id);

        /// <summary>
        /// Поиск книг по названию или описанию
        /// </summary>
        List<Book> SearchBooks(string query);

        /// <summary>
        /// Получить книги определённого жанра
        /// </summary>
        List<Book> GetBooksByGenre(string genre);

        /// <summary>
        /// Получить все книги конкретного автора
        /// </summary>
        List<Book> GetBooksByAuthor(int authorId);

        /// <summary>
        /// Удалить книгу по идентификатору
        /// </summary>
        void DeleteBook(int id);

        /// <summary>
        /// Обновить данные книги
        /// </summary>
        void UpdateBook(int id, string title, List<string> genres, string description, string content);

        /// <summary>
        /// Получить список всех жанров
        /// </summary>
        List<string> GetAllGenres();
    }
}
