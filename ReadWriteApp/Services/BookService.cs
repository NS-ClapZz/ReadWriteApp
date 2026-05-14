using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using ReadWriteApp.Data;
using ReadWriteApp.Models;
using ReadWriteApp.Services.Interfaces;

namespace ReadWriteApp.Services
{
    /// <summary>
    /// Сервис для работы с книгами (через SQLite)
    /// </summary>
    public class BookService : IBookService
    {
        /// <summary>
        /// Добавляет новую книгу в каталог
        /// </summary>
        public void AddBook(string title, int authorId, List<string> genres, string description, string content)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название книги не может быть пустым");

            if (genres == null || genres.Count == 0)
                throw new ArgumentException("Нужно указать хотя бы один жанр");

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            // Вставляем книгу
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Books (Title, AuthorId, Description, Content, PublishedDate) 
                VALUES (@title, @authorId, @desc, @content, @date)";
            cmd.Parameters.AddWithValue("@title", title.Trim());
            cmd.Parameters.AddWithValue("@authorId", authorId);
            cmd.Parameters.AddWithValue("@desc", description?.Trim() ?? "");
            cmd.Parameters.AddWithValue("@content", content?.Trim() ?? "");
            cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
            cmd.ExecuteNonQuery();

            // Получаем Id только что вставленной книги
            cmd.CommandText = "SELECT last_insert_rowid()";
            long bookId = (long)cmd.ExecuteScalar()!;

            // Привязываем жанры к книге
            foreach (var genreName in genres)
            {
                int genreId = GetOrCreateGenreId(connection, genreName.Trim());
                var linkCmd = connection.CreateCommand();
                linkCmd.CommandText = "INSERT OR IGNORE INTO BookGenres (BookId, GenreId) VALUES (@bookId, @genreId)";
                linkCmd.Parameters.AddWithValue("@bookId", bookId);
                linkCmd.Parameters.AddWithValue("@genreId", genreId);
                linkCmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Возвращает список всех книг из каталога
        /// </summary>
        public List<Book> GetAllBooks()
        {
            var books = new List<Book>();

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Title, AuthorId, Description, Content, PublishedDate FROM Books ORDER BY Id";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var book = ReadBookFromRow(reader);
                book.Genres = GetGenresForBook(connection, book.Id);
                books.Add(book);
            }

            return books;
        }

        /// <summary>
        /// Находит книгу по её идентификатору
        /// </summary>
        public Book? GetBookById(int id)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Title, AuthorId, Description, Content, PublishedDate FROM Books WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var book = ReadBookFromRow(reader);
                book.Genres = GetGenresForBook(connection, book.Id);
                return book;
            }

            return null;
        }

        /// <summary>
        /// Ищет книги по совпадению в названии или описании
        /// </summary>
        public List<Book> SearchBooks(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return GetAllBooks();

            var books = new List<Book>();

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, Title, AuthorId, Description, Content, PublishedDate 
                FROM Books 
                WHERE Title LIKE @query OR Description LIKE @query
                ORDER BY Id";
            cmd.Parameters.AddWithValue("@query", $"%{query}%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var book = ReadBookFromRow(reader);
                book.Genres = GetGenresForBook(connection, book.Id);
                books.Add(book);
            }

            return books;
        }

        /// <summary>
        /// Фильтрует книги по указанному жанру (ищет среди всех жанров книги)
        /// </summary>
        public List<Book> GetBooksByGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre))
                return GetAllBooks();

            var books = new List<Book>();

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT DISTINCT b.Id, b.Title, b.AuthorId, b.Description, b.Content, b.PublishedDate 
                FROM Books b
                INNER JOIN BookGenres bg ON b.Id = bg.BookId
                INNER JOIN Genres g ON bg.GenreId = g.Id
                WHERE g.Name = @genre
                ORDER BY b.Id";
            cmd.Parameters.AddWithValue("@genre", genre);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var book = ReadBookFromRow(reader);
                book.Genres = GetGenresForBook(connection, book.Id);
                books.Add(book);
            }

            return books;
        }

        /// <summary>
        /// Возвращает все книги конкретного автора
        /// </summary>
        public List<Book> GetBooksByAuthor(int authorId)
        {
            var books = new List<Book>();

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, Title, AuthorId, Description, Content, PublishedDate 
                FROM Books 
                WHERE AuthorId = @authorId
                ORDER BY Id";
            cmd.Parameters.AddWithValue("@authorId", authorId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var book = ReadBookFromRow(reader);
                book.Genres = GetGenresForBook(connection, book.Id);
                books.Add(book);
            }

            return books;
        }

        /// <summary>
        /// Удаляет книгу из каталога по идентификатору
        /// </summary>
        public void DeleteBook(int id)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            // Сначала удаляем связи с жанрами
            var cmdGenres = connection.CreateCommand();
            cmdGenres.CommandText = "DELETE FROM BookGenres WHERE BookId = @id";
            cmdGenres.Parameters.AddWithValue("@id", id);
            cmdGenres.ExecuteNonQuery();

            // Затем удаляем саму книгу
            var cmdBook = connection.CreateCommand();
            cmdBook.CommandText = "DELETE FROM Books WHERE Id = @id";
            cmdBook.Parameters.AddWithValue("@id", id);
            cmdBook.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновляет данные существующей книги
        /// </summary>
        public void UpdateBook(int id, string title, List<string> genres, string description, string content)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            // Обновляем основные поля книги
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                UPDATE Books 
                SET Title = @title, Description = @desc, Content = @content 
                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@title", title?.Trim() ?? "");
            cmd.Parameters.AddWithValue("@desc", description?.Trim() ?? "");
            cmd.Parameters.AddWithValue("@content", content?.Trim() ?? "");
            cmd.ExecuteNonQuery();

            // Удаляем старые связи с жанрами
            var delCmd = connection.CreateCommand();
            delCmd.CommandText = "DELETE FROM BookGenres WHERE BookId = @id";
            delCmd.Parameters.AddWithValue("@id", id);
            delCmd.ExecuteNonQuery();

            // Добавляем новые связи с жанрами
            if (genres != null)
            {
                foreach (var genreName in genres)
                {
                    int genreId = GetOrCreateGenreId(connection, genreName.Trim());
                    var linkCmd = connection.CreateCommand();
                    linkCmd.CommandText = "INSERT OR IGNORE INTO BookGenres (BookId, GenreId) VALUES (@bookId, @genreId)";
                    linkCmd.Parameters.AddWithValue("@bookId", id);
                    linkCmd.Parameters.AddWithValue("@genreId", genreId);
                    linkCmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Возвращает список всех уникальных жанров из каталога
        /// </summary>
        public List<string> GetAllGenres()
        {
            var genres = new List<string>();

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Name FROM Genres ORDER BY Name";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                genres.Add(reader.GetString(0));
            }

            return genres;
        }

        /// <summary>
        /// Создаёт объект Book из строки результата запроса
        /// </summary>
        private Book ReadBookFromRow(SqliteDataReader reader)
        {
            return new Book
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                AuthorId = reader.GetInt32(2),
                Description = reader.GetString(3),
                Content = reader.GetString(4),
                PublishedDate = DateTime.Parse(reader.GetString(5))
            };
        }

        /// <summary>
        /// Получает список жанров для конкретной книги
        /// </summary>
        private List<string> GetGenresForBook(SqliteConnection connection, int bookId)
        {
            var genres = new List<string>();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT g.Name 
                FROM Genres g
                INNER JOIN BookGenres bg ON g.Id = bg.GenreId
                WHERE bg.BookId = @bookId
                ORDER BY g.Name";
            cmd.Parameters.AddWithValue("@bookId", bookId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                genres.Add(reader.GetString(0));
            }

            return genres;
        }

        /// <summary>
        /// Находит жанр по имени или создаёт новый, возвращает Id
        /// </summary>
        private int GetOrCreateGenreId(SqliteConnection connection, string genreName)
        {
            // Сначала ищем существующий жанр
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id FROM Genres WHERE Name = @name";
            cmd.Parameters.AddWithValue("@name", genreName);

            var result = cmd.ExecuteScalar();
            if (result != null)
                return Convert.ToInt32(result);

            // Если жанра нет — создаём
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = "INSERT INTO Genres (Name) VALUES (@name)";
            insertCmd.Parameters.AddWithValue("@name", genreName);
            insertCmd.ExecuteNonQuery();

            insertCmd.CommandText = "SELECT last_insert_rowid()";
            return Convert.ToInt32(insertCmd.ExecuteScalar()!);
        }
    }
}
