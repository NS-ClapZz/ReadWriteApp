using System.Collections.Generic;
using ReadWriteApp.Models;

namespace ReadWriteApp.Services.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для работы с авторами
    /// </summary>
    public interface IAuthorService
    {
        /// <summary>
        /// Найти автора по идентификатору
        /// </summary>
        Author? GetAuthorById(int id);

        /// <summary>
        /// Получить список всех авторов
        /// </summary>
        List<Author> GetAllAuthors();

        /// <summary>
        /// Обновить данные автора
        /// </summary>
        void UpdateAuthor(int id, string firstName, string lastName, string bio);
    }
}
