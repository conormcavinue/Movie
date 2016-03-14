using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieBrowser.Domain.Entities;
using MovieBrowser.Domain.Abstract;
using MovieBrowser.Domain.Concrete;
using MovieBrowser.WebUI.Models;
using MediaToolkit;
using MediaToolkit.Model;
using System.Configuration;

namespace MovieBrowser.WebUI.Controllers
{
    public class MovieController : Controller
    {
        private IMovieRepository repository;
        public int PageSize = 6;

        public MovieController(IMovieRepository movieRepo)
        {
            this.repository = movieRepo;
        }

        public ViewResult Play(string location)
        {
            MovieViewModel viewModel = new MovieViewModel
            {
                Location = location
            };
       

            return View(viewModel);

        }

        public ActionResult Convert(string name)
        {
            EFDbContext context = new EFDbContext();
            int movieID = context.Movies.Where(u => u.Location == name).Select(u => u.MovieID).FirstOrDefault();

            name = name.Replace(ConfigurationManager.AppSettings["baseVirtualDir"], ConfigurationManager.AppSettings["baseFileDir"]);
            name = name.Replace("/", "\\");

            var inputFile = new MediaFile { Filename = name };

            string output = (name.Substring(0, name.LastIndexOf('.')) + ".mp4");

            var outputFile = new MediaFile { Filename = output };
            using (var engine = new Engine())
            {
                engine.Convert(inputFile, outputFile);
            }

            DeleteFromDB(movieID);
            DeleteAfterConvert(name);

            return Redirect(Url.Action("Scan", "Scan"));
        }

        public void DeleteAfterConvert(string name)
        {
            if(System.IO.File.Exists(name.Substring(0, name.LastIndexOf('.')) + ".mp4") && !name.EndsWith("mp4"))
            {
                System.IO.File.Delete(name);
            }
        }

        public ViewResult List(string range,int page=1)
        {
            MoviesListViewModel viewModel = new MoviesListViewModel
            {
                Movies = repository.Movies
                .Where(p => range == null || p.Range == range)
                .OrderBy(p => p.MovieID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = range == null ?
                            repository.Movies.Count() :
                            repository.Movies.Where(e => e.Range == range).Count()
                },
                CurrentRange = range
            };

            return View(viewModel);
        
        }

        public ActionResult DeleteFromDB(int movieID)
        {
            Movie deletedMovie = repository.DeleteEntry(movieID);
            TempData["message"] = string.Format("{0} has been removed from the database", deletedMovie.Name);
            return Redirect(Url.Action("List", "Movie"));
        }

    }
}
