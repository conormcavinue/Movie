using MovieBrowser.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieBrowser.WebUI.Controllers
{
    public class NavController : Controller
    {
        private IMovieRepository repository;

        public NavController(IMovieRepository repo)
        {
            repository = repo;
        }

        public PartialViewResult Menu(string genre = null)
        {
            ViewBag.SelectedGenre = genre;


            IEnumerable<string> genres = repository.Movies
                                            .Select(x => x.Genre)
                                            .Distinct()
                                            .OrderBy(x => x);

            return PartialView(genres);

        }
    }
}
