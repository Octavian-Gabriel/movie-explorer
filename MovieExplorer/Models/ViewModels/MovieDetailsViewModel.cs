namespace MovieExplorer.Models.ViewModels
{
    public class MovieDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "Unknown";
        public string Description { get; set; } = "Unknown";
        public List<string> ImageUrls { get; set; }= new List<string>();
        public List<ActorViewModel> Actors { get; set; } = new List<ActorViewModel>();
    }
    public class ActorViewModel
    {
        public string Name { get; set; } = "Unknown";
        public string Character { get; set; } = "Unknown";
        public string? ProfileImgUrl { get; set; }
    }
}