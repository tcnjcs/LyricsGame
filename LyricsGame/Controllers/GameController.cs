using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LyricsGame.Models;

namespace LyricsGame.Controllers
{
    public class GameController : Controller
    {

        MusicDBContext db = new MusicDBContext();
        static Random rnd = new Random();

        public ActionResult Index()
        {
            //If player is first to guess lyrics for segment, half the points for the segment will automatically 
            //be added to user's points. Players are awarded points if they match segment in database
            string specGenre = "rock";
            //Create List of musicIDs from the genre specified by the User
            List<int> idList = db.Music.Where(g => g.Genre == specGenre).Select(mID => mID.MusicID).ToList();
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
                return null;
            }

            int selection = new Random().Next(0, segments.Count);
            ViewBag.SegmentID = segments[selection].LyricSegmentID;

            //Pull start and end times of chosen segment
            ViewBag.Start = segments[selection].Start;
            ViewBag.End = segments[selection].End;

            return View();
        }

        [HttpPost]
        public ActionResult Index(String flags, String segmentID)
        {
            int lyricSegID = 1;

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
            FlagHandler flagHandler = new FlagHandler(db);
            if (flags.Equals("CutOff"))
            {
                IList<LyricSegment> nextSegCandidates = db.Lyrics.Where(ls => ls.LyricSegmentID == lyricSegID+1 && ls.Start == segment.End).ToList();
                //Procede only if segment is not last segment for song
                if (nextSegCandidates.Count() != 0)
                {
                    LyricSegment nextSegment = nextSegCandidates.First();
                    flagHandler.CutOff(segment, nextSegment);
                }
            }
            else if (flags.Equals("NoLyrics"))
                flagHandler.NoLyrics(segment);

            return View("Results", db.Music.ToList());
        }

        //[HttpPost]
        //public ActionResult Results(String sID)
        //{
        //    int songID = -1;

        //    try
        //    {
        //        songID = Int16.Parse(sID);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new HttpException(404, "An error occured while selecting a song");
        //    }

        //    Music song = db.Music.Find(songID);
        //    ViewBag.MusicID = songID;
        //    ViewBag.FilePath = song.FilePath;

        //    return View(db.Music.ToList());
        //}

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
                throw new HttpException(404, "An error occured while obtaining song data.");
            }

            Music song = db.Music.Find(musicID);
            ViewBag.MusicID = musicID;
            ViewBag.FilePath = song.FilePath;
            ViewBag.Title = song.Title;
            ViewBag.Artist = song.Artist;

            return PartialView("SelectedResultSong", db.Lyrics.Where(ls => ls.MusicID == musicID));
        }


        public ActionResult Results()
        {
            ViewBag.Message = "";

            return View(db.Music.ToList());
        }

       
        public ActionResult GenreSelector()
        {

            //var janrahs = db.Music.Select(g => g.Genre).Distinct().ToList();
            //int i = 0;
            //string val;
            //var genres = new List<SelectListItem>();

            //foreach (string j in janrahs)
            //{
            //    i++;
            //    val = i.ToString();
            //    genres.Add(new SelectListItem()
            //    {
            //        Text = j,
            //        Value = val,
            //        Selected = false
            //    });
            //

            ViewBag.Message = "";
            var Genres = db.Music.Select(g => g.Genre).Distinct().ToList();
            ViewBag.Genres = Genres;

            return View(Genres);

        }

        [HttpPost]
        public ActionResult GenreSelector(string selectedGenre)
        {
            ViewBag.Message = "";

            return View();
        }


    }
}
