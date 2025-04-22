namespace MovieExplorer.Models.ViewModels
{
    public class MovieDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "Unknown";
        public string Description { get; set; } = "Unknown";
        public List<string> ImageUrls { get; set; }= new List<string>();
        public List<ActorViewModel> Actors { get; set; } = new List<ActorViewModel>();
        public List<CommentViewModel> Comments { get; set; }=new List<CommentViewModel>();

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
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

    }
}