using System;
using System.Collections.Generic;
using ReadWriteApp.Models;

namespace ReadWriteApp.Data
{
    /// <summary>
    /// Хранилище данных в памяти (имитация базы данных для прототипа)
    /// </summary>
    public static class DataStore
    {
        /// <summary>
        /// Список всех книг
        /// </summary>
        public static List<Book> Books { get; set; } = new List<Book>();

        /// <summary>
        /// Список всех авторов
        /// </summary>
        public static List<Author> Authors { get; set; } = new List<Author>();

        /// <summary>
        /// Список всех пользователей
        /// </summary>
        public static List<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// Текущий авторизованный пользователь
        /// </summary>
        public static User? CurrentUser { get; set; } = null;

        // Счетчики для генерации уникальных Id
        private static int _nextBookId = 1;
        private static int _nextAuthorId = 1;
        private static int _nextUserId = 1;

        /// <summary>
        /// Получить следующий уникальный Id для книги
        /// </summary>
        public static int GetNextBookId() => _nextBookId++;

        /// <summary>
        /// Получить следующий уникальный Id для автора
        /// </summary>
        public static int GetNextAuthorId() => _nextAuthorId++;

        /// <summary>
        /// Получить следующий уникальный Id для пользователя
        /// </summary>
        public static int GetNextUserId() => _nextUserId++;

        /// <summary>
        /// Заполняет хранилище начальными тестовыми данными
        /// </summary>
        public static void Initialize()
        {
            // Создаём тестовых авторов
            var author1 = new Author
            {
                Id = GetNextAuthorId(),
                FirstName = "Анна",
                LastName = "Светлова",
                Bio = "Молодая писательница, автор сказок и рассказов для детей."
            };

            var author2 = new Author
            {
                Id = GetNextAuthorId(),
                FirstName = "Дмитрий",
                LastName = "Ночёв",
                Bio = "Пишет фантастические повести и рассказы с 2020 года."
            };

            var author3 = new Author
            {
                Id = GetNextAuthorId(),
                FirstName = "Елена",
                LastName = "Книжкина",
                Bio = "Автор коротких рассказов и стихов о природе."
            };

            Authors.AddRange(new[] { author1, author2, author3 });

            // Создаём тестовых пользователей
            var user1 = new User
            {
                Id = GetNextUserId(),
                Login = "anna",
                Password = "123",
                Role = UserRole.Author,
                AuthorId = author1.Id
            };

            var user2 = new User
            {
                Id = GetNextUserId(),
                Login = "dmitry",
                Password = "123",
                Role = UserRole.Author,
                AuthorId = author2.Id
            };

            var user3 = new User
            {
                Id = GetNextUserId(),
                Login = "elena",
                Password = "123",
                Role = UserRole.Author,
                AuthorId = author3.Id
            };

            var user4 = new User
            {
                Id = GetNextUserId(),
                Login = "reader",
                Password = "123",
                Role = UserRole.Reader
            };

            Users.AddRange(new[] { user1, user2, user3, user4 });

            // Создаём тестовые книги
            Books.Add(new Book
            {
                Id = GetNextBookId(),
                Title = "Сказка о потерянном коте",
                AuthorId = author1.Id,
                Genre = "Сказка",
                Description = "Добрая история о котике, который искал свой дом и нашёл настоящих друзей.",
                Content = "Жил-был на свете маленький рыжий кот. Однажды он выглянул в окно и увидел, как по улице бежит собака. Кот решил, что тоже хочет погулять, и выпрыгнул через форточку.\n\nОн шёл по улице, разглядывая дома и деревья. Всё было таким новым и интересным! Но когда стемнело, кот понял, что не помнит, где его дом.\n\n— Мяу! — позвал он. — Кто-нибудь, помогите мне!\n\nИз-за забора выглянул пёс.\n— Ты потерялся? Пойдём, я знаю, кто может помочь.\n\nТак кот нашёл новых друзей, а вскоре и дорогу домой.",
                PublishedDate = new DateTime(2025, 3, 15)
            });

            Books.Add(new Book
            {
                Id = GetNextBookId(),
                Title = "Звёздный путешественник",
                AuthorId = author2.Id,
                Genre = "Фантастика",
                Description = "Мальчик находит загадочный прибор и отправляется в путешествие по галактике.",
                Content = "Кирилл нашёл странную штуку на чердаке дедушкиного дома. Она была похожа на компас, но вместо стрелки внутри мерцала маленькая звезда.\n\nКогда он нажал на кнопку сбоку, комната вдруг закружилась, и через секунду Кирилл стоял на поверхности другой планеты. Небо было фиолетовым, а вместо солнца светили два оранжевых шара.\n\n— Добро пожаловать на Аркадию, — сказал кто-то за спиной.\n\nКирилл обернулся и увидел существо, похожее на светящуюся медузу.\n\n— Мы ждали тебя, путешественник.",
                PublishedDate = new DateTime(2025, 6, 1)
            });

            Books.Add(new Book
            {
                Id = GetNextBookId(),
                Title = "Утренний туман",
                AuthorId = author3.Id,
                Genre = "Поэзия",
                Description = "Сборник стихов о природе, временах года и красоте окружающего мира.",
                Content = "Утренний туман ложится на поля,\nРоса блестит на травах серебром.\nПросыпается тихая земля,\nИ птицы запевают за окном.\n\n* * *\n\nЛистья падают — осень пришла,\nЗолотая, тихая, родная.\nВетер шепчет у окна,\nПесню грустную напевая.\n\n* * *\n\nЗима укрыла землю белым сном,\nСнежинки кружат в танце над рекой.\nИ мир затих, окутанный теплом\nДомашнего уюта и покой.",
                PublishedDate = new DateTime(2025, 9, 10)
            });

            Books.Add(new Book
            {
                Id = GetNextBookId(),
                Title = "Приключения робота Винтика",
                AuthorId = author2.Id,
                Genre = "Фантастика",
                Description = "История о маленьком роботе, который мечтал стать живым.",
                Content = "В лаборатории профессора Шестерёнкина родился маленький робот. Профессор назвал его Винтик.\n\nВинтик умел считать, читать и даже рисовать, но одного он не умел — чувствовать. Он видел, как люди смеются и плачут, обнимаются и грустят, и очень хотел понять, что это такое.\n\n— Профессор, что такое радость? — спросил однажды Винтик.\n— Радость — это когда внутри тебе тепло и хочется улыбаться.\n— Но у меня нет тепла внутри. Только провода и микросхемы.\n\nПрофессор задумался. Может быть, радость — это не только тепло?",
                PublishedDate = new DateTime(2025, 11, 20)
            });

            Books.Add(new Book
            {
                Id = GetNextBookId(),
                Title = "Бабушкины рецепты счастья",
                AuthorId = author1.Id,
                Genre = "Рассказ",
                Description = "Тёплые истории о семье, доброте и бабушкиной мудрости.",
                Content = "Каждое лето Маша ездила к бабушке в деревню. Бабушка Вера жила в маленьком домике с голубыми ставнями и огромным садом.\n\n— Бабуль, а почему ты всегда улыбаешься? — спросила Маша.\n— А у меня рецепт есть, — ответила бабушка. — Утром встаёшь, открываешь окно и говоришь: «Здравствуй, новый день!» А потом делаешь что-нибудь доброе. И всё — день удался.\n\nМаша попробовала. И правда — работает.",
                PublishedDate = new DateTime(2026, 1, 5)
            });
        }
    }
}
