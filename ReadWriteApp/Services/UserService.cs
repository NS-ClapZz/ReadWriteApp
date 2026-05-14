using System;
using Microsoft.Data.Sqlite;
using ReadWriteApp.Data;
using ReadWriteApp.Models;
using ReadWriteApp.Services.Interfaces;

namespace ReadWriteApp.Services
{
    public class UserService : IUserService
    {
        public bool Register(string login, string password, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return false;

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Login = @login COLLATE NOCASE";
            checkCmd.Parameters.AddWithValue("@login", login.Trim());
            long count = (long)checkCmd.ExecuteScalar()!;

            if (count > 0)
                return false;

            int? authorId = null;

            if (role == UserRole.Author)
            {
                var authorCmd = connection.CreateCommand();
                authorCmd.CommandText = "INSERT INTO Authors (FirstName, LastName, Bio) VALUES (@fn, @ln, @bio)";
                authorCmd.Parameters.AddWithValue("@fn", login.Trim());
                authorCmd.Parameters.AddWithValue("@ln", "");
                authorCmd.Parameters.AddWithValue("@bio", "");
                authorCmd.ExecuteNonQuery();

                authorCmd.CommandText = "SELECT last_insert_rowid()";
                authorId = Convert.ToInt32(authorCmd.ExecuteScalar()!);
            }

            var userCmd = connection.CreateCommand();
            userCmd.CommandText = "INSERT INTO Users (Login, Password, Role, AuthorId) VALUES (@login, @password, @role, @authorId)";
            userCmd.Parameters.AddWithValue("@login", login.Trim());
            userCmd.Parameters.AddWithValue("@password", password);
            userCmd.Parameters.AddWithValue("@role", (int)role);
            userCmd.Parameters.AddWithValue("@authorId", authorId.HasValue ? (object)authorId.Value : DBNull.Value);
            userCmd.ExecuteNonQuery();

            return true;
        }

        public bool Login(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return false;

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, Login, Password, Role, AuthorId FROM Users WHERE Login = @login COLLATE NOCASE AND Password = @password";
            cmd.Parameters.AddWithValue("@login", login.Trim());
            cmd.Parameters.AddWithValue("@password", password);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var user = new User
                {
                    Id = reader.GetInt32(0),
                    Login = reader.GetString(1),
                    Password = reader.GetString(2),
                    Role = (UserRole)reader.GetInt32(3),
                    AuthorId = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                };

                DataStore.CurrentUser = user;
                return true;
            }

            return false;
        }

        public void Logout()
        {
            DataStore.CurrentUser = null;
        }

        public User? GetCurrentUser()
        {
            return DataStore.CurrentUser;
        }
    }
}
