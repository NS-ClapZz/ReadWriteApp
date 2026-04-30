using System;
using System.Collections.Generic;
using System.Linq;
using ReadWriteApp.Data;
using ReadWriteApp.Models;
using ReadWriteApp.Services.Interfaces;

namespace ReadWriteApp.Services
{
    /// <summary>
    /// Сервис для работы с книгами
    /// </summary>
    public class BookService : IBookService
    {
        /// <summary>
        /// Добавляет новую книгу в каталог
        /// </summary>
        public void AddBook(string title, int authorId, string genre, string description, string content)
        {
            // Проверяем, что обязательные поля заполнены
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название книги не может быть пустым");

            if (string.IsNullOrWhiteSpace(genre))
                throw new ArgumentException("Жанр книги не может быть пустым");

            var book = new Book
            {
                Id = DataStore.GetNextBookId(),
                Title = title.Trim(),
                AuthorId = authorId,
                Genre = genre.Trim(),
                Description = description?.Trim() ?? string.Empty,
                Content = content?.Trim() ?? string.Empty,
                PublishedDate = DateTime.Now
            };

            DataStore.Books.Add(book);
        }

        /// <summary>
        /// Возвращает список всех книг из каталога
        /// </summary>
        public List<Book> GetAllBooks()
        {
            return DataStore.Books.ToList();
        }

        /// <summary>
        /// Находит книгу по её идентификатору
        /// </summary>
        public Book? GetBookById(int id)
        {
            return DataStore.Books.FirstOrDefault(b => b.Id == id);
        }

        /// <summary>
        /// Ищет книги по совпадению в названии или описании
        /// </summary>
        public List<Book> SearchBooks(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return GetAllBooks();

            string lowerQuery = query.ToLower();

            return DataStore.Books
                .Where(b => b.Title.ToLower().Contains(lowerQuery)
                         || b.Description.ToLower().Contains(lowerQuery))
                .ToList();
        }

        /// <summary>
        /// Фильтрует книги по указанному жанру
        /// </summary>
        public List<Book> GetBooksByGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre))
                return GetAllBooks();

            return DataStore.Books
                .Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Возвращает все книги конкретного автора
        /// </summary>
        public List<Book> GetBooksByAuthor(int authorId)
        {
            return DataStore.Books
                .Where(b => b.AuthorId == authorId)
                .ToList();
        }

        /// <summary>
        /// Удаляет книгу из каталога по идентификатору
        /// </summary>
        public void DeleteBook(int id)
        {
            var book = DataStore.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                DataStore.Books.Remove(book);
            }
        }

        /// <summary>
        /// Обновляет данные существующей книги
        /// </summary>
        public void UpdateBook(int id, string title, string genre, string description, string content)
        {
            var book = DataStore.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                book.Title = title?.Trim() ?? book.Title;
                book.Genre = genre?.Trim() ?? book.Genre;
                book.Description = description?.Trim() ?? book.Description;
                book.Content = content?.Trim() ?? book.Content;
            }
        }

        /// <summary>
        /// Возвращает список всех уникальных жанров из каталога
        /// </summary>
        public List<string> GetAllGenres()
        {
            return DataStore.Books
                .Select(b => b.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToList();
        }
    }
}
