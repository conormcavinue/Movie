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

        public void Play(string name)
        {
            Process.Start(name);
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

        public void Delete(int movieID)
        {
            Movie deletedMovie = repository.DeleteEntry(movieID);
        }

    }
}
