using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieBrowser.Domain.Abstract;
using MovieBrowser.Domain.Entities;
using MediaToolkit.Model;
using MediaToolkit;

namespace MovieBrowser.WebUI.Controllers
{
    public class ScanController : Controller
    {
        static IMovieRepository repository;

        static int changeCount = 0;

        static DirectoryInfo MovieDir = null;
        static List<FileInfo> files = new List<FileInfo>();
        static DirectoryInfo MainDir = new DirectoryInfo("C:/Users/Conor/Downloads/Downloaded Torrents");

        public ScanController(IMovieRepository repo)
        {
            repository = repo;
        }

        public ActionResult Scan()
        {
            List<String> extensions = new List<String>();
            extensions.Add(".mkv");
            extensions.Add(".mp4");
            int count = ScanDirs(MainDir.FullName, extensions);
            TempData["message"] = string.Format("Scan completed. {0} item"+((count == 1) ? "" : "s")+" added to database", count);
            changeCount = 0;

            return Redirect(Url.Action("List", "Movie"));
        }

        public void Play(string name)
        {
            Process.Start(name);
        }

        public ActionResult Clear() 
        {
            repository.ClearTable();
            return Redirect(Url.Action("List", "Movie"));
        }

        private static int ScanDirs(string path, List<String> exts)
        {

            try
            {
                string FilePath = path;
                MovieDir = new DirectoryInfo(FilePath);
                
                FileInfo[] tempFiles = MovieDir.GetFiles();
                foreach (FileInfo f in tempFiles)
                {
                    

                    foreach (String s in exts)
                    {
                        if (f.FullName.EndsWith(s) && !f.Name.ToLower().Contains("sample"))
                        {
                            files.Add(f);
                            Movie m = new Movie();
                            DirectoryInfo temp = new DirectoryInfo(f.Directory.FullName);

                            if (!temp.Equals(null))
                            {
                                if (temp.Name.ToString() != MainDir.Name.ToString())
                                {
                                    while (temp.Parent.ToString() != MainDir.Name.ToString())
                                    {
                                        temp = temp.Parent;
                                    }
                                }
                            }
                            m.Genre = (temp.Name.ToString() == "Films") || (temp.Name.ToString() == "TV") ? temp.Name.ToString() : "Other" ;
                            string fileFolderPath = f.FullName.Replace("C:\\Users\\Conor\\Downloads\\Downloaded Torrents", "");
                            fileFolderPath = fileFolderPath.Replace("\\", "/");
                            m.Location = "http://localhost/Videos" + fileFolderPath;
                            m.Name = f.Name;
                            //m.Name = m.Name.Replace(s, "");
                            m.Name = m.Name.Replace(".", " ");
                            if (!repository.Movies.Any(o => o.Name == m.Name)) {   
                                repository.AddEntry(m);
                                changeCount++;
                            }
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException e)
            {
                throw new DirectoryNotFoundException("Directory not found");
            }
            catch (IOException e)
            {
                throw new IOException("Could not access files");
            }

            DirectoryInfo[] FilePathSubDirs = MovieDir.GetDirectories();
            ArrayList SubDirs = new ArrayList();
            foreach (DirectoryInfo subdir in FilePathSubDirs)
            {
                SubDirs.Add(subdir.FullName);
            }

            foreach (string dir in SubDirs)
            {
                ScanDirs(dir, exts);
            }
            return changeCount;
        }

    }
}
