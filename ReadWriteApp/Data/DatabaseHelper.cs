using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace ReadWriteApp.Data
{
    public static class DatabaseHelper
    {
        private static string _dbPath = string.Empty;

        public static string ConnectionString => $"Data Source={_dbPath}";

        public static void InitializeDatabase()
        {
            string appFolder = AppDomain.CurrentDomain.BaseDirectory;
            _dbPath = Path.Combine(appFolder, "readwrite.db");

            CreateTables();

            if (GetRecordCount("Authors") == 0)
            {
                SeedTestData();
            }
        }

        private static void CreateTables()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Authors (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL DEFAULT '',
                    Bio TEXT NOT NULL DEFAULT ''
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Login TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Role INTEGER NOT NULL DEFAULT 0,
                    AuthorId INTEGER,
                    FOREIGN KEY (AuthorId) REFERENCES Authors(Id)
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Books (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    AuthorId INTEGER NOT NULL,
                    Description TEXT NOT NULL DEFAULT '',
                    Content TEXT NOT NULL DEFAULT '',
                    PublishedDate TEXT NOT NULL,
                    FOREIGN KEY (AuthorId) REFERENCES Authors(Id)
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Genres (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS BookGenres (
                    BookId INTEGER NOT NULL,
                    GenreId INTEGER NOT NULL,
                    PRIMARY KEY (BookId, GenreId),
                    FOREIGN KEY (BookId) REFERENCES Books(Id) ON DELETE CASCADE,
                    FOREIGN KEY (GenreId) REFERENCES Genres(Id)
                )";
            cmd.ExecuteNonQuery();
        }

        private static long GetRecordCount(string tableName)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT COUNT(*) FROM {tableName}";
            return (long)cmd.ExecuteScalar()!;
        }

        private static void SeedTestData()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            ExecuteInsert(connection, "INSERT INTO Authors (FirstName, LastName, Bio) VALUES (@fn, @ln, @bio)",
                new Dictionary<string, object> {
                    {"@fn", "Анна"}, {"@ln", "Светлова"},
                    {"@bio", "Молодая писательница, автор сказок и рассказов для детей."}
                });
            ExecuteInsert(connection, "INSERT INTO Authors (FirstName, LastName, Bio) VALUES (@fn, @ln, @bio)",
                new Dictionary<string, object> {
                    {"@fn", "Дмитрий"}, {"@ln", "Ночёв"},
                    {"@bio", "Пишет фантастические повести и рассказы с 2020 года."}
                });
            ExecuteInsert(connection, "INSERT INTO Authors (FirstName, LastName, Bio) VALUES (@fn, @ln, @bio)",
                new Dictionary<string, object> {
                    {"@fn", "Елена"}, {"@ln", "Книжкина"},
                    {"@bio", "Автор коротких рассказов и стихов о природе."}
                });

            ExecuteInsert(connection, "INSERT INTO Users (Login, Password, Role, AuthorId) VALUES (@l, @p, @r, @a)",
                new Dictionary<string, object> {
                    {"@l", "anna"}, {"@p", "123"}, {"@r", 1}, {"@a", 1}
                });
            ExecuteInsert(connection, "INSERT INTO Users (Login, Password, Role, AuthorId) VALUES (@l, @p, @r, @a)",
                new Dictionary<string, object> {
                    {"@l", "dmitry"}, {"@p", "123"}, {"@r", 1}, {"@a", 2}
                });
            ExecuteInsert(connection, "INSERT INTO Users (Login, Password, Role, AuthorId) VALUES (@l, @p, @r, @a)",
                new Dictionary<string, object> {
                    {"@l", "elena"}, {"@p", "123"}, {"@r", 1}, {"@a", 3}
                });
            ExecuteInsert(connection, "INSERT INTO Users (Login, Password, Role) VALUES (@l, @p, @r)",
                new Dictionary<string, object> {
                    {"@l", "reader"}, {"@p", "123"}, {"@r", 0}
                });

            string[] genres = { "Сказка", "Фантастика", "Поэзия", "Рассказ", "Повесть", "Роман", "Детектив", "Приключения" };
            foreach (var genre in genres)
            {
                ExecuteInsert(connection, "INSERT INTO Genres (Name) VALUES (@name)",
                    new Dictionary<string, object> { {"@name", genre} });
            }

            ExecuteInsert(connection,
                "INSERT INTO Books (Title, AuthorId, Description, Content, PublishedDate) VALUES (@t, @a, @d, @c, @p)",
                new Dictionary<string, object> {
                    {"@t", "Сказка о потерянном коте"}, {"@a", 1},
                    {"@d", "Добрая история о котике, который искал свой дом и нашёл настоящих друзей."},
                    {"@c", "Жил-был на свете маленький рыжий кот. Однажды он выглянул в окно и увидел, как по улице бежит собака. Кот решил, что тоже хочет погулять, и выпрыгнул через форточку.\n\nОн шёл по улице, разглядывая дома и деревья. Всё было таким новым и интересным! Но когда стемнело, кот понял, что не помнит, где его дом.\n\n— Мяу! — позвал он. — Кто-нибудь, помогите мне!\n\nИз-за забора выглянул пёс.\n— Ты потерялся? Пойдём, я знаю, кто может помочь.\n\nТак кот нашёл новых друзей, а вскоре и дорогу домой."},
                    {"@p", "2025-03-15"}
                });
            ExecuteInsert(connection, "INSERT INTO BookGenres (BookId, GenreId) VALUES (1, 1)", null);
            ExecuteInsert(connection, "INSERT INTO BookGenres (BookId, GenreId) VALUES (1, 4)", null);

            ExecuteInsert(connection,
                "INSERT INTO Books (Title, AuthorId, Description, Content, PublishedDate) VALUES (@t, @a, @d, @c, @p)",
                new Dictionary<string, object> {
                    {"@t", "Звёздный путешественник"}, {"@a", 2},
                    {"@d", "Мальчик находит загадочный прибор и отправляется в путешествие по галактике."},
                    {"@c", "Кирилл нашёл странную штуку на чердаке дедушкиного дома. Она была похожа на компас, но вместо стрелки внутри мерцала маленькая звезда.\n\nКогда он нажал на кнопку сбоку, комната вдруг закружилась, и через секунду Кирилл стоял на поверхности другой планеты. Небо было фиолетовым, а вместо солнца светили два оранжевых шара.\n\n— Добро пожаловать на Аркадию, — сказал кто-то за спиной.\n\nКирилл обернулся и увидел существо, похожее на светящуюся медузу.\n\n— Мы ждали тебя, путешественник."},
                    {"@p", "2025-06-01"}
                });
            ExecuteInsert(connection, "INSERT INTO BookGenres (BookId, GenreId) VALUES (2, 2)", null);
            ExecuteInsert(connection, "INSERT INTO BookGenres (BookId, GenreId) VALUES (2, 8)", null);

            ExecuteInsert(connection,
                "INSERT INTO Books (Title, AuthorId, Description, Content, PublishedDate) VALUES (@t, @a, @d, @c, @p)",
                new Dictionary<string, object> {
                    {"@t", "Утренний туман"}, {"@a", 3},
                    {"@d", "Сборник стихов о природе, временах года и красоте окружающего мира."},
                    {"@c", "Утренний туман ложится на поля,\nРоса блестит на травах серебром.\nПросыпается тихая земля,\nИ птицы запевают за окном.\n\n* * *\n\nЛистья падают — осень пришла,\nЗолотая, тихая, родная.\nВетер шепчет у окна,\nПесню грустную напевая.\n\n* * *\n\nЗима укрыла землю белым сном,\nСнежинки кружат в танце над рекой.\nИ мир затих, окутанный теплом\nДомашнего уюта и покой."},
                    {"@p", "2025-09-10"}
                });
            ExecuteInsert(connection, "INSERT INTO BookGenres (BookId, GenreId) VALUES (3, 3)", null);

            ExecuteInsert(connection,
                "INSERT INTO Books (Title, AuthorId, Description, Content, PublishedDate) VALUES (@t, @a, @d, @c, @p)",
                new Dictionary<string, object> {
                    {"@t", "Приключения робота Винтика"}, {"@a", 2},
                    {"@d", "История о маленьком роботе, который мечтал стать живым."},
                    {"@c", "В лаборатории профессора Шестерёнкина родился маленький робот. Профессор назвал его Винтик.\n\nВинтик умел считать, читать и даже рисовать, но одного он не умел — чувствовать. Он видел, как люди смеются и плачут, обнимаются и грустят, и очень хотел понять, что это такое.\n\n— Профессор, что такое радость? — спросил однажды Винтик.\n— Радость — это когда внутри тебе тепло и хочется улыбаться.\n— Но у меня нет тепла внутри. Только провода и микросхемы.\n\nПрофессор задумался. Может быть, радость — это не только тепло?"},
                    {"@p", "2025-11-20"}
                });
            ExecuteInsert(connection, "INSERT INTO BookGenres (BookId, GenreId) VALUES (4, 2)", null);
            ExecuteInsert(connection, "INSERT INTO BookGenres (BookId, GenreId) VALUES (4, 1)", null);

            ExecuteInsert(connection,
                "INSERT INTO Books (Title, AuthorId, Description, Content, PublishedDate) VALUES (@t, @a, @d, @c, @p)",
                new Dictionary<string, object> {
                    {"@t", "Бабушкины рецепты счастья"}, {"@a", 1},
                    {"@d", "Тёплые истории о семье, доброте и бабушкиной мудрости."},
                    {"@c", "Каждое лето Маша ездила к бабушке в деревню. Бабушка Вера жила в маленьком домике с голубыми ставнями и огромным садом.\n\n— Бабуль, а почему ты всегда улыбаешься? — спросила Маша.\n— А у меня рецепт есть, — ответила бабушка. — Утром встаёшь, открываешь окно и говоришь: «Здравствуй, новый день!» А потом делаешь что-нибудь доброе. И всё — день удался.\n\nМаша попробовала. И правда — работает."},
                    {"@p", "2026-01-05"}
                });
            ExecuteInsert(connection, "INSERT INTO BookGenres (BookId, GenreId) VALUES (5, 4)", null);

            transaction.Commit();
        }

        private static void ExecuteInsert(SqliteConnection connection, string sql, Dictionary<string, object>? parameters)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            cmd.ExecuteNonQuery();
        }
    }
}
