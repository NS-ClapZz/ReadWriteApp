using System;
using System.Linq;
using ReadWriteApp.Data;
using ReadWriteApp.Models;
using ReadWriteApp.Services.Interfaces;

namespace ReadWriteApp.Services
{
    /// <summary>
    /// Сервис для работы с пользователями (регистрация, авторизация)
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// Регистрирует нового пользователя в системе
        /// </summary>
        /// <returns>true, если регистрация прошла успешно; false, если логин уже занят</returns>
        public bool Register(string login, string password, UserRole role)
        {
            // Проверяем, что логин и пароль не пустые
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return false;

            // Проверяем, не занят ли логин
            bool loginExists = DataStore.Users.Any(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
            if (loginExists)
                return false;

            var user = new User
            {
                Id = DataStore.GetNextUserId(),
                Login = login.Trim(),
                Password = password,
                Role = role
            };

            // Если пользователь регистрируется как автор, создаём запись автора
            if (role == UserRole.Author)
            {
                var author = new Author
                {
                    Id = DataStore.GetNextAuthorId(),
                    FirstName = login.Trim(),
                    LastName = "",
                    Bio = ""
                };
                DataStore.Authors.Add(author);
                user.AuthorId = author.Id;
            }

            DataStore.Users.Add(user);
            return true;
        }

        /// <summary>
        /// Авторизует пользователя по логину и паролю
        /// </summary>
        /// <returns>true, если авторизация успешна; false, если логин или пароль неверные</returns>
        public bool Login(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return false;

            var user = DataStore.Users.FirstOrDefault(u =>
                u.Login.Equals(login, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);

            if (user != null)
            {
                DataStore.CurrentUser = user;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Выполняет выход пользователя из системы
        /// </summary>
        public void Logout()
        {
            DataStore.CurrentUser = null;
        }

        /// <summary>
        /// Возвращает текущего авторизованного пользователя
        /// </summary>
        public User? GetCurrentUser()
        {
            return DataStore.CurrentUser;
        }
    }
}
