using System.Collections.Generic;
using MovieBrowser.Domain.Entities;

namespace MovieBrowser.WebUI.Models
{
    public class MoviesListViewModel
    {
        public IEnumerable<Movie> Movies { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentGenre { get; set; }
    }
}