using System;

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
        /// Жанр книги
        /// </summary>
        public string Genre { get; set; } = string.Empty;

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
