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
using System.Configuration;

namespace MovieBrowser.WebUI.Controllers
{
    public class ScanController : Controller
    {
        static IMovieRepository repository;
        static int changeCount = 0;

        static DirectoryInfo MovieDir = null;
        static List<FileInfo> files = new List<FileInfo>();
        static DirectoryInfo MainDir = new DirectoryInfo(ConfigurationManager.AppSettings["baseFileDir"]);

        private List<string> createExtList()
        {
            List<string> extensions = new List<string>();

            extensions.Add(".mkv");
            extensions.Add(".mp4");
            extensions.Add(".webm");

            return extensions;
        }

        public ScanController(IMovieRepository repo)
        {
            repository = repo;
        }

        public ActionResult Scan()
        {
            int count = ScanDirs(MainDir.FullName, createExtList());
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
                            m.Genre = (temp.Name.ToString() == "Film") || (temp.Name.ToString() == "TV") ? temp.Name.ToString() : "Other" ;
                            m.Location = ConfigurationManager.AppSettings["baseVirtualDir"] + m.Genre + "/" + f.Name;
                            m.Name = f.Name;
                            //m.Name = m.Name.Replace(s, "");
                            m.Name = m.Name.Replace(".", " ");
                            if (!repository.Movies.Any(o => o.Location == m.Location)) {   
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
