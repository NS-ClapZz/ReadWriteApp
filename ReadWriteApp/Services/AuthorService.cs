using System;
using System.Collections.Generic;
using System.Linq;
using ReadWriteApp.Data;
using ReadWriteApp.Models;
using ReadWriteApp.Services.Interfaces;

namespace ReadWriteApp.Services
{
    /// <summary>
    /// Сервис для работы с авторами
    /// </summary>
    public class AuthorService : IAuthorService
    {
        /// <summary>
        /// Находит автора по его идентификатору
        /// </summary>
        public Author? GetAuthorById(int id)
        {
            return DataStore.Authors.FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Возвращает список всех авторов
        /// </summary>
        public List<Author> GetAllAuthors()
        {
            return DataStore.Authors.ToList();
        }

        /// <summary>
        /// Обновляет данные автора (имя, фамилия, биография)
        /// </summary>
        public void UpdateAuthor(int id, string firstName, string lastName, string bio)
        {
            var author = DataStore.Authors.FirstOrDefault(a => a.Id == id);
            if (author != null)
            {
                author.FirstName = firstName?.Trim() ?? author.FirstName;
                author.LastName = lastName?.Trim() ?? author.LastName;
                author.Bio = bio?.Trim() ?? author.Bio;
            }
        }
    }
}
