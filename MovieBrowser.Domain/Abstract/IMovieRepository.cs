using System.Linq;
using MovieBrowser.Domain.Entities;
using MovieBrowser.Domain.Abstract;

namespace MovieBrowser.Domain.Abstract
{
    public interface IMovieRepository
    {
        IQueryable<Movie> Movies { get; }

        void AddEntry(Movie movie);
        void ClearTable();
        Movie DeleteEntry(int movieID);
    }
}
