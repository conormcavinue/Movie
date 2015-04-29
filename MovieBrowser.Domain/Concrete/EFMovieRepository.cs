using MovieBrowser.Domain.Entities;
using MovieBrowser.Domain.Abstract;
using System.Linq;


namespace MovieBrowser.Domain.Concrete
{
    public class EFMovieRepository : IMovieRepository
    {
        private EFDbContext context = new EFDbContext();

        public IQueryable<Movie> Movies
        {
            get { return context.Movies; }
        }

        public void AddEntry(Movie movie)
        {
            Movie dbEntry = context.Movies.Add(movie);
            context.SaveChanges();
        }

        public void ClearTable()
        {
            foreach (Movie m in Movies)
            {
                context.Movies.Remove(m);
            }
            context.SaveChanges();
        }

        public Movie DeleteEntry(int movieID)
        {
            Movie dbEntry = context.Movies.Find(movieID);
            if (dbEntry != null)
            {
                context.Movies.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
    }
}
