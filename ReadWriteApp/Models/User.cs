namespace ReadWriteApp.Models
{
    /// <summary>
    /// Модель пользователя системы
    /// </summary>
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Роль пользователя в системе
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Идентификатор связанного автора (если роль — Автор)
        /// </summary>
        public int? AuthorId { get; set; }
    }
}
