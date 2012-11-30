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

        public ActionResult Index()
        {
            //If player is first to guess lyrics for segment, half the points for the segment will automatically 
            //be added to user's points. Players are awarded points if they match segment in database

            //Temporary find song with ID and use it as chosen song
            int musicID = 29;

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
            InputProcessor inputProcessor = new InputProcessor(db);
            if (flags.Equals("CutOff"))
            {
                IList<LyricSegment> nextSegCandidates = db.Lyrics.Where(ls => ls.MusicID == segment.MusicID && ls.Start == segment.End).ToList();
                //Procede only if segment is not last segment for song
                if (nextSegCandidates.Count() != 0)
                {
                    LyricSegment nextSegment = nextSegCandidates.First();
                    inputProcessor.CutOff(segment, nextSegment);
                }
            }
            else if (flags.Equals("NoLyrics"))
                inputProcessor.NoLyrics(segment);

            
            return View("Results", db.Music.ToList());
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

            return PartialView("SelectedResultSong", db.Lyrics.Where(ls => ls.MusicID == musicID));
        }

        [HttpPost]
        public ActionResult ResultSongPossibleLyrics(String songID)
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
            ViewBag.SegNum = song.Lyrics.Count;

            return PartialView("ResultSongPossibleLyrics");
        }

        public ActionResult Results()
        {
            ViewBag.Message = "";

            //db.Database.SqlQuery(System.Type.GetType("String"), "select DISTINCT(genre) from Musics", null);

            return View(db.Music.ToList());
        }
    }
}
