using System;
using System.Collections.Generic;

namespace ReadWriteApp.Models
{
    /// <summary>
    /// Модель книги в системе
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Уникальный идентификатор книги
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название книги
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор автора книги
        /// </summary>
        public int AuthorId { get; set; }

        /// <summary>
        /// Список жанров книги
        /// </summary>
        public List<string> Genres { get; set; } = new List<string>();

        /// <summary>
        /// Жанры через запятую (для отображения в интерфейсе)
        /// </summary>
        public string GenresDisplay => string.Join(", ", Genres);

        /// <summary>
        /// Краткое описание книги
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Содержание (текст) книги
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Дата публикации
        /// </summary>
        public DateTime PublishedDate { get; set; }
    }
}
