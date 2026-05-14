using ReadWriteApp.Models;

namespace ReadWriteApp.Services.Interfaces
{
    public interface IUserService
    {
        bool Register(string login, string password, UserRole role);
        bool Login(string login, string password);
        void Logout();
        User? GetCurrentUser();
    }
}
