using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LyricsGame.Models;
using System.Data;
using System.Web.Script.Serialization;
using System.Dynamic;

namespace LyricsGame.Controllers
{
    [Authorize]
    public class GameController : Controller
    {

        MusicDBContext db = new MusicDBContext();
        UsersContext uc = new UsersContext();

        static Random rnd = new Random();

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Partial = "GameScreen";
            sendSongSegment();

            TimeSpan now = DateTime.UtcNow - new DateTime(1970, 1, 1);
            ViewBag.StartTime = now.TotalSeconds;
            return View();
        }

        [HttpPost]
        public ActionResult GameScreen(String flags, String segmentID, String entry, double startTime, String win)
        {
             int lyricSegID = 1;
            UserProfile activeUser = uc.UserProfiles.FirstOrDefault(g => g.UserName.ToLower() == User.Identity.Name);
   
            //Prevent breaking things if improper segmentID recieved
            try
            {
                lyricSegID = Int16.Parse(segmentID);
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "Sorry an error occured while processing your input. Please return to the homepage and start a new round.";
                return null;
            }

            LyricSegment segment = db.Lyrics.Find(lyricSegID);

            //Check if special case exists and do approiate action
            InputProcessor inputProcessor = new InputProcessor(db);
            if (flags.Equals("CutOff"))
            {
                IList<LyricSegment> nextSegCandidates = db.Lyrics.Where(ls => ls.MusicID == segment.MusicID && ls.Start == segment.End).ToList();
                //Procede only if segment is not last segment for song
                if (nextSegCandidates.Count() != 0)
                {
                    LyricSegment nextSegment = nextSegCandidates.First();
                    if (inputProcessor.CutOff(segment, nextSegment))
                    {
                        activeUser.Points += 2;
                        ViewBag.Points = "+2";
                    }
                    else
                        ViewBag.Points = "Sorry, answers did not match";
                }
            }
            else if (flags.Equals("NoLyrics") || entry == "")
            {
                if (inputProcessor.NoLyrics(segment))
                {
                    activeUser.Points += 2;
                    ViewBag.Points = "+2";
                }
                else
                    ViewBag.Points = "Sorry, answers did not match";
            }
            else if (flags.Equals("Lyrics"))
            {
                if (inputProcessor.Lyrics(segment, entry, startTime, activeUser))
                {
                    activeUser.Points += 10;
                    ViewBag.Points = "+10";
                    if (win.Equals("true"))
                    {
                        activeUser.Points += 2;
                        ViewBag.Bonus = "Speed Bonus: +2";
                    }
                }
                else
                    ViewBag.Points = "Sorry, your answers did not match";
            }
            sendSongSegment();
            TimeSpan now = DateTime.UtcNow - new DateTime(1970, 1, 1);
            ViewBag.StartTime = now.TotalSeconds;
            uc.SaveChanges();

            Ranks.UpdateRank(activeUser.UserName);
            
            return PartialView("GameScreen");
        }

        [HttpPost]
        public ActionResult SelectedResultSong(String songID)
        {
            int musicID = -1;
            
            try
            {
                musicID = Int16.Parse(songID);
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "The selected song has been moved or deleted. Please return to the home page and start a new round.";
                return null;
            }

            Music song = db.Music.Find(musicID);
            ViewBag.MusicID = musicID;
            ViewBag.FilePath = song.FilePath;
            ViewBag.Title = song.Title;
            ViewBag.Artist = song.Artist;
            ViewBag.SegCount = song.Lyrics.Count;
            
            int maxSeg = 0;
            IList<LyricSegment> lyricSeg = song.Lyrics.ToList();
            foreach (LyricSegment ls in lyricSeg)
            {
                IList<LyricsStats> lyStat = db.LyricStats.Where(lstat => lstat.LyricSegmentID == ls.LyricSegmentID).ToList();
                if (lyStat.Count > maxSeg)
                {
                    maxSeg = lyStat.Count;
                }
            }
            ViewBag.MaxNumOfSegs = maxSeg;

            return PartialView("SelectedResultSong", db.Lyrics.Where(ls => ls.MusicID == musicID));
        }

        public ActionResult Results()
        {
            ViewBag.Message = "";

            //db.Database.SqlQuery(System.Type.GetType("String"), "select DISTINCT(genre) from Musics", null);

            return View(db.Music.ToList());
        }

        public void sendSongSegment()
        {List<int> idList = new List<int>();
            string userName = User.Identity.Name;
            UserProfile activeUser = uc.UserProfiles.FirstOrDefault(g => g.UserName.ToLower() == userName);
            string specGenre = activeUser.ActiveGenre;
            if (specGenre == "")
            {
                specGenre = "Random";
            }

            if (specGenre == "Random")
            {
                idList = db.Music.Where(g => g.Genre != specGenre).Select(mID => mID.MusicID).ToList();
            }
            else
            {
                idList = db.Music.Where(g => g.Genre == specGenre).Select(mID => mID.MusicID).ToList();
            }
			
            int idIndex = rnd.Next(idList.Count);
            int musicID = idList[idIndex];

            Music song = db.Music.Find(musicID);
            ViewBag.MusicID = musicID;
            ViewBag.Path = song.FilePath;

            //Find segments pertaining to chosen song and chose one randomly.
            IList<LyricSegment> segments = db.Lyrics.Where(ls => ls.MusicID == musicID && !ls.Complete).ToList();
            if (segments.Count() == 0)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "The selected song has been moved or deleted. Please return to the home page and start a new round.";
            }

            int selection = new Random().Next(0, segments.Count);
            ViewBag.SegmentID = segments[selection].LyricSegmentID;
            //Pull start and end times of chosen segment
            ViewBag.Start = segments[selection].Start;
            ViewBag.End = segments[selection].End;

            List<LyricsUser> possibleUsers = segments[selection].LyricUsers.ToList();
            if (possibleUsers.Count != 0)
            {
                int userNum = new Random().Next(0, possibleUsers.Count);
                ViewBag.Time = possibleUsers[userNum].Time;
            }
            else
                ViewBag.Time = 40;

        }

        public ActionResult GenreSelector()
        {
            ViewBag.Message = "";
            List<string> Genres = db.Music.Select(g => g.Genre).Distinct().ToList();
            ViewBag.Genres = Genres;

            return View();
        }

        [HttpPost]
        public ActionResult GenreSelector(FormCollection collection)
        {

            List<string> Genres = db.Music.Select(g => g.Genre).Distinct().ToList();
            ViewBag.Genres = Genres;

            string choiceGenre = collection["Choice"];

            string userName = User.Identity.Name;
            UserProfile activeUser = uc.UserProfiles.FirstOrDefault(g => g.UserName.ToLower() == userName);
            activeUser.ActiveGenre = choiceGenre;
            uc.SaveChanges();

            return View();
        }

        [HttpPost]
        public ActionResult GetLyricsForSong(String songID)
        {
            int musicID = -1;

            try
            {
                musicID = Int16.Parse(songID);
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "The selected song has been moved or deleted. Please return to the home page and start a new round.";
                return null;
            }

            Music song = db.Music.Find(musicID);
            IList<LyricSegment> lyricSegs = song.Lyrics.ToList();

            var ret = new Dictionary<string, Dictionary<string, string>>{};

            foreach (LyricSegment ls in lyricSegs)
            {
                IList<LyricsStats> lyStat = db.LyricStats.Where(lsStat => lsStat.LyricSegmentID == ls.LyricSegmentID).OrderBy(lst => lst.Votes).ToList();
                Dictionary<string, string> d = new Dictionary<string, string>{};
                foreach (LyricsStats lys in lyStat)
                {
                    if (lys.Available)
                    {
                        d.Add(lys.LyricsStatsID.ToString(), lys.Lyrics);
                        //uncomment the above line and comment the below lin in order to only see "available" lystats.
                    }
                    //d.Add(lys.LyricsStatsID.ToString(), lys.Lyrics);
                }
                ret.Add(ls.LyricSegmentID.ToString(), d);
            }

            return Json(ret);
        }

        [HttpPost]
        public ActionResult castVote(string statID)
        {
            int id = -1;

            try
            {
                id = Int16.Parse(statID);
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "The selected song has been moved or deleted. Please return to the home page and start a new round.";
                return null;
            }

            LyricsStats lyStat = db.LyricStats.Where(ls => ls.LyricsStatsID == id).FirstOrDefault();
            UserProfile activeUser = uc.UserProfiles.FirstOrDefault(g => g.UserName.ToLower() == User.Identity.Name);

            IList<UserSegmentVotes> otherVote = db.UserSegmentVotes.Where(us => us.LyricSegmentID == lyStat.LyricSegmentID && us.UserID == activeUser.UserId).ToList();

            if (otherVote.Count() == 0)
            {
                UserSegmentVotes newEntryUser = new UserSegmentVotes();
                newEntryUser.LyricSegmentID = lyStat.LyricSegmentID;
                newEntryUser.LyricsStatsID = lyStat.LyricsStatsID;
                newEntryUser.UserID = activeUser.UserId;
                db.UserSegmentVotes.Add(newEntryUser);
                lyStat.Votes += 1;
            }
            else
            {
                UserSegmentVotes usv = otherVote.First();
                int oldID = otherVote.First().LyricsStatsID;
                IList<LyricsStats> oldCounts = db.LyricStats.Where(ls => ls.LyricsStatsID == oldID).ToList();

                foreach (LyricsStats oldCount in oldCounts)
                {
                    oldCount.Votes -= 1;
                }

                lyStat.Votes += 1;
                
                UserSegmentVotes newEntryUser = new UserSegmentVotes();
                newEntryUser.LyricSegmentID = lyStat.LyricSegmentID;
                newEntryUser.LyricsStatsID = lyStat.LyricsStatsID;
                newEntryUser.UserID = activeUser.UserId;
                db.UserSegmentVotes.Add(newEntryUser);

                db.UserSegmentVotes.Remove(usv);
            }

            /*IList<LyricsStats> test = db.LyricStats.ToArray();

            foreach (LyricsStats test2 in test)
            {
                test2.Votes = 0;
            }*/

            db.SaveChanges();

            return Json(new { });
        }

        [HttpPost]
        public ActionResult getVotedList()
        {
            UserProfile activeUser = uc.UserProfiles.FirstOrDefault(g => g.UserName.ToLower() == User.Identity.Name);

            IList<UserSegmentVotes> userVotes = db.UserSegmentVotes.Where(usv => usv.UserID == activeUser.UserId).ToList();

            var ret = new Dictionary<string, string>();
            int i = 0;

            foreach (UserSegmentVotes uVotes in userVotes)
            {
                ret.Add(i.ToString(), uVotes.LyricsStatsID.ToString());
                i++;
            }

            return Json(ret);
        }

    }
}