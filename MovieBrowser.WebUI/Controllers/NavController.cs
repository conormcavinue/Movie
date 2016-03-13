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

        public PartialViewResult Menu(string range = null)
        {
            ViewBag.SelectedRange = range;


            IEnumerable<string> ranges = repository.Movies
                                            .Select(x => x.Range)
                                            .Distinct()
                                            .OrderBy(x => x);

            return PartialView(ranges);

        }
    }
}
