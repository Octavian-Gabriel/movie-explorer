﻿namespace MovieExplorer.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public required User User { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
