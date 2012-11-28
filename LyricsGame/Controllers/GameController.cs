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
            int musicID = 21;
            Music song = db.Music.Find(musicID);
            ViewBag.MusicID = musicID;
            ViewBag.Path = song.FilePath;

            //Find segments pertaining to chosen song and chose one randomly. Will need to also check if segment is complete
            IList<LyricSegment> segments = db.Lyrics.Where(od => od.MusicID == musicID).ToList();
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
                throw new HttpException(404, "Sorry an error occured while processing your input. Please reload the page and try again.");
            }

            LyricSegment segment = db.Lyrics.Find(lyricSegID);
            LyricSegment nextSegment = db.Lyrics.Find(lyricSegID + 1);

            if (flags.Equals("CutOff"))
                FlagHandler.CutOff(segment, nextSegment, db);

            return View("Results");
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

            return PartialView("SelectedResultSong");
        }

        public ActionResult Results()
        {
            ViewBag.Message = "";

            return View(db.Music.ToList());
        }

    }
}
