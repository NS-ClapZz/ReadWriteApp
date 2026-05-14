using System;
using System.Collections.Generic;

namespace ReadWriteApp.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public string GenresDisplay => string.Join(", ", Genres);
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
    }
}
