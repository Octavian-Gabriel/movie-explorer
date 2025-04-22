using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieExplorer.Models.ViewModels
{
    public class MovieSearchViewModel
    {
        //the movie name provided by the visitor
        public string MovieName { get; set; }=string.Empty;
        // the genre provided by the visitor
        public int? GenreId { get; set; }
        // a list of genres for the dropdown menu
        public IEnumerable<SelectListItem> GenreList { get; set; } = Enumerable.Empty<SelectListItem>();
        //results of the search
        public IEnumerable<MovieListViewModel> MovieList { get; set;} = Enumerable.Empty<MovieListViewModel>();
    }
}
