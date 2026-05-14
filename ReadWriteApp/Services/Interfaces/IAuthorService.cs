using System.Collections.Generic;
using ReadWriteApp.Models;

namespace ReadWriteApp.Services.Interfaces
{
    public interface IAuthorService
    {
        Author? GetAuthorById(int id);
        List<Author> GetAllAuthors();
        void UpdateAuthor(int id, string firstName, string lastName, string bio);
    }
}
