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
using System.Text.RegularExpressions;

namespace MovieBrowser.WebUI.Controllers
{
    public class ScanController : Controller
    {
        static IMovieRepository repository;
        static int changeCount = 0;

        static DirectoryInfo MovieDir = null;
        static List<FileInfo> files = new List<FileInfo>();
        static DirectoryInfo MainDir = new DirectoryInfo(ConfigurationManager.AppSettings["baseFileDir"]);
        static List<string> LetterRanges = setRanges();

        private List<string> createExtList()
        {
            List<string> extensions = new List<string>();

            extensions.Add(".mkv");
            extensions.Add(".mp4");
            extensions.Add(".webm");

            return extensions;
        }

        static List<string> setRanges()
        {
            List<string> Ranges = new List<string>();

            Ranges.Add("A-D");
            Ranges.Add("E-H");
            Ranges.Add("I-L");
            Ranges.Add("M-P");
            Ranges.Add("Q-S");
            Ranges.Add("T-V");
            Ranges.Add("W-Z");
            Ranges.Add("0-9");

            return Ranges;
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

        public ActionResult Clear() 
        {
            repository.ClearTable();
            TempData["message"] = "Database Cleared";
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
                            
                            string firstLetter = f.Name.Substring(0, 1);
                            foreach (var interval in LetterRanges)
                            {
                                if (Regex.IsMatch(firstLetter, @"(?i)[" + interval + "]")) {
                                    m.Range = interval;
                                    break;
                                }
                            }
                            if(m.Range == null)
                            {
                                m.Range = @"£$%...";
                            }

                            m.Location = f.FullName.Replace(ConfigurationManager.AppSettings["baseFileDir"], ConfigurationManager.AppSettings["baseVirtualDir"]);
                            m.Location = m.Location.Replace("\\","/");
                            m.Name = f.Name;
                            m.Name = m.Name.Replace(s, "");
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
