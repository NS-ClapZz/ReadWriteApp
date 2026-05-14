using ReadWriteApp.Models;

namespace ReadWriteApp.Data
{
    /// <summary>
    /// Хранит состояние текущей сессии (авторизованный пользователь)
    /// </summary>
    public static class DataStore
    {
        /// <summary>
        /// Текущий авторизованный пользователь
        /// </summary>
        public static User? CurrentUser { get; set; } = null;

        /// <summary>
        /// Инициализирует базу данных при запуске приложения
        /// </summary>
        public static void Initialize()
        {
            DatabaseHelper.InitializeDatabase();
        }
    }
}
