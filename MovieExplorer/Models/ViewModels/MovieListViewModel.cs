namespace MovieExplorer.Models.ViewModels
{
    public class MovieListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PosterPath { get; set; } = string.Empty;
        public string ReleaseDate { get; set; } = string.Empty;
    }
}
