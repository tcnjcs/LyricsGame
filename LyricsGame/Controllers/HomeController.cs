using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LyricsGame.Models;


namespace LyricsGame.Controllers
{
    public class HomeController : Controller
    {
        MusicDBContext music = new MusicDBContext();

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Game()
        {
            //If player is first to guess lyrics for segment, half the points for the segment will automatically 
            //be added to user's points. Players are awarded points if they match segment in database
            Music song = music.Music.Find(16);
            ViewBag.Path = song.FilePath;

            IList<LyricSegment> segments = music.Lyrics.Where(od => od.MusicID == 16).ToList();
            int selection = new Random().Next(0, segments.Count);
            ViewBag.Start = segments[selection].Start;
            ViewBag.End = segments[selection].End;
            return View();
        }
    }
}
