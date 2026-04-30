namespace ReadWriteApp.Models
{
    /// <summary>
    /// Модель автора
    /// </summary>
    public class Author
    {
        /// <summary>
        /// Уникальный идентификатор автора
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Имя автора
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Фамилия автора
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Биография автора
        /// </summary>
        public string Bio { get; set; } = string.Empty;

        /// <summary>
        /// Полное имя автора (имя + фамилия)
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
