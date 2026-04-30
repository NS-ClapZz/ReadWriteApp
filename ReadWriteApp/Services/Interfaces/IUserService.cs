using ReadWriteApp.Models;

namespace ReadWriteApp.Services.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для работы с пользователями
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Зарегистрировать нового пользователя
        /// </summary>
        bool Register(string login, string password, UserRole role);

        /// <summary>
        /// Авторизация пользователя по логину и паролю
        /// </summary>
        bool Login(string login, string password);

        /// <summary>
        /// Выход из системы
        /// </summary>
        void Logout();

        /// <summary>
        /// Получить текущего авторизованного пользователя
        /// </summary>
        User? GetCurrentUser();
    }
}
