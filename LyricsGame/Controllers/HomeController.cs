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

        /// <summary>
        /// Generates the leaderboard and user's rank and points needed.
        /// </summary>
        /// <returns>The leaderboard and user's rank/points needed as a partial view.</returns>
        public ActionResult Leaderboard()
        {
            var db = new UsersContext();
            var profiles = db.UserProfiles;
            var activeUser = new UserProfile();
            var username = User.Identity.Name;

            //if there is a logged in user
            if (username != null && username !="")
            {
                var user = from p in profiles
                           where p.UserName == username
                           select p;
                activeUser = user.FirstOrDefault();
            }

            //select top 10 users, ordered by points descending.
            List<UserProfile> results = (from p in profiles orderby p.Points descending select p).Take(10).ToList();

            List<string> leaders = new List<string>();
            List<int> points = new List<int>();

            //create the leaderboard list
            foreach (var result in results)
            {
                leaders.Add(result.UserName);
                points.Add(result.Points);
            }
            ViewBag.leaders = leaders;
            ViewBag.points = points;

            //update user's view of rank and points.
            if (activeUser != null)
            {
                ViewBag.myrank = activeUser.Rank;
                ViewBag.mypoints = activeUser.Points;

                string origrank = activeUser.Rank;
                string currentRank = origrank;
                int pointsTilNext = 0;
                int myPoints = activeUser.Points;

                //calculate points needed to rank up.
                while (currentRank == origrank)
                {
                    pointsTilNext++;
                    currentRank = Ranks.GetRank(myPoints + pointsTilNext);
                }

                ViewBag.pointstogo = pointsTilNext;

            }
            
            //return as partial view so it can be on many pages.
            return PartialView("Leaderboard");
        }




    }
}
