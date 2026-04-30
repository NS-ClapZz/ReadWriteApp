namespace ReadWriteApp.Models
{
    /// <summary>
    /// Роли пользователей в системе
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Читатель — может просматривать книги
        /// </summary>
        Reader,

        /// <summary>
        /// Автор — может публиковать и редактировать свои книги
        /// </summary>
        Author
    }
}
