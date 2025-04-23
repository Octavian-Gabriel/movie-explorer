namespace MovieExplorer.Models.ViewModels
{
    public class MovieDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "Unknown";
        public string Description { get; set; } = "Unknown";
        public List<string> ImageUrls { get; set; }= new ();
        public List<ActorViewModel> Actors { get; set; } = new ();
        public List<CommentViewModel> Comments { get; set; }=new ();
        public List<string> Genres { get; set; } = new(); 
    }
    public class ActorViewModel
    {
        public string Name { get; set; } = "Unknown";
        public string Character { get; set; } = "Unknown";
        public string? ProfileImgUrl { get; set; }
    }
    public class CommentViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

    }
}