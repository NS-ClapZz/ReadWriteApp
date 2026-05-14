using System.Collections.Generic;
using ReadWriteApp.Models;

namespace ReadWriteApp.Services.Interfaces
{
    public interface IBookService
    {
        void AddBook(string title, int authorId, List<string> genres, string description, string content);
        List<Book> GetAllBooks();
        Book? GetBookById(int id);
        List<Book> SearchBooks(string query);
        List<Book> GetBooksByGenre(string genre);
        List<Book> GetBooksByAuthor(int authorId);
        void DeleteBook(int id);
        void UpdateBook(int id, string title, List<string> genres, string description, string content);
        List<string> GetAllGenres();
    }
}
