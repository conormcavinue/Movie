using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieBrowser.Domain.Entities;
using MovieBrowser.Domain.Abstract;
using MovieBrowser.WebUI.Models;
using MediaToolkit;
using MediaToolkit.Model;

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
            //Process.Start(name);
            //return Redirect(Url.Action("List", "Movie"));
       

            return View(viewModel);

        }

        public ActionResult Convert(string name)
        {
            name = name.Replace(ScanController.getBaseVirtualDir(), ScanController.getBaseFileDir());
            //name = name.Replace("/", "\\");

            var inputFile = new MediaFile { Filename = name };

            string output = (name.Substring(0, name.LastIndexOf('.')) + ".mp4");

            var outputFile = new MediaFile { Filename = output };
            using (var engine = new Engine())
            {
                engine.Convert(inputFile, outputFile);
            }
            return Redirect(Url.Action("List", "Movie"));
        }

        public ViewResult List(string genre,int page=1)
        {
            MoviesListViewModel viewModel = new MoviesListViewModel
            {
                Movies = repository.Movies
                .Where(p => genre == null || p.Genre == genre)
                .OrderBy(p => p.MovieID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = genre == null ?
                            repository.Movies.Count() :
                            repository.Movies.Where(e => e.Genre == genre).Count()
                },
                CurrentGenre = genre
            };

            return View(viewModel);
        
        }

        public ActionResult Delete(int movieID)
        {
            Movie deletedMovie = repository.DeleteEntry(movieID);
            TempData["message"] = string.Format("{0} has been removed from the database", deletedMovie.Name);
            return Redirect(Url.Action("List", "Movie"));
        }

    }
}
