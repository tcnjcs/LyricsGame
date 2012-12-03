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

        public ActionResult Index()
        {
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

        public ActionResult Leaderboard()
        {
            var db = new UsersContext();
            var profiles = db.UserProfiles;
            List<UserProfile> results = (from p in profiles orderby p.Points descending select p).Take(5).ToList();
            List<string> leaders = new List<string>();
            List<int> points = new List<int>();
            foreach (var result in results)
            {
                leaders.Add(result.UserName);
                points.Add(result.Points);
            }
            ViewBag.leaders = leaders;
            ViewBag.points = points;
            return PartialView("Leaderboard");
        }


    }
}
