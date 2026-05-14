using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using ReadWriteApp.Data;
using ReadWriteApp.Models;
using ReadWriteApp.Services.Interfaces;

namespace ReadWriteApp.Services
{
    public class AuthorService : IAuthorService
    {
        public Author? GetAuthorById(int id)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, FirstName, LastName, Bio FROM Authors WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Author
                {
                    Id = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Bio = reader.GetString(3)
                };
            }

            return null;
        }

        public List<Author> GetAllAuthors()
        {
            var authors = new List<Author>();

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, FirstName, LastName, Bio FROM Authors ORDER BY Id";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                authors.Add(new Author
                {
                    Id = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Bio = reader.GetString(3)
                });
            }

            return authors;
        }

        public void UpdateAuthor(int id, string firstName, string lastName, string bio)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                UPDATE Authors
                SET FirstName = @fn, LastName = @ln, Bio = @bio
                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fn", firstName?.Trim() ?? "");
            cmd.Parameters.AddWithValue("@ln", lastName?.Trim() ?? "");
            cmd.Parameters.AddWithValue("@bio", bio?.Trim() ?? "");
            cmd.ExecuteNonQuery();
        }
    }
}
