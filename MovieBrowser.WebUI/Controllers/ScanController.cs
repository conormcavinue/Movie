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

namespace MovieBrowser.WebUI.Controllers
{
    public class ScanController : Controller
    {
        static IMovieRepository repository;

        static DirectoryInfo MovieDir = null;
        static List<FileInfo> files = new List<FileInfo>();
        static DirectoryInfo MainDir = new DirectoryInfo("C:/Users/Conor/Downloads/Downloaded Torrents");

        public ScanController(IMovieRepository repo)
        {
            repository = repo;
        }

        public ViewResult Scan()
        {
            List<String> extensions = new List<String>();
            extensions.Add(".mkv");
            extensions.Add(".mp4");
            ScanDirs(MainDir.FullName, extensions);
            

            return View(files);
        }

        public void Play(string name)
        {
            Process.Start(name);
        }

        public void Clear() 
        {
            repository.ClearTable();
        }

        private static void ScanDirs(string path, List<String> exts)
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
                        if (f.FullName.EndsWith(s) && !f.Name.Contains("sample") && !f.Name.Contains("Sample"))
                        {
                            files.Add(f);
                            Movie m = new Movie();
                            DirectoryInfo temp = new DirectoryInfo(f.Directory.FullName);

                            if (!temp.Equals(null))
                            {
                                while (temp.Parent.ToString() != MainDir.Name.ToString())
                                {
                                    temp = temp.Parent;
                                }
                            }
                            m.Genre = temp.Name.ToString();
                            m.Location = f.FullName;
                            m.Name = f.Name;
                            if (!repository.Movies.Any(o => o.Name == m.Name)) {   
                                repository.AddEntry(m);
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

        }

    }
}
