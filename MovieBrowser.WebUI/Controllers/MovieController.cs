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

        public ViewResult Play(string name)
        {
            //Process.Start(name);
            //return Redirect(Url.Action("List", "Movie"));
            return View("Play");

        }

        public ActionResult Convert(string name)
        {
            var inputFile = new MediaFile { Filename = name };
            string output;
            if (name.Substring(name.Length - 4) == ".mkv")
            {
                output = (name.Substring(0, name.Length - 4) + ".mp4");

            }
            else
            {
                output = (name.Substring(0, name.Length - 4) + ".mkv");
            }
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
