using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LyricsGame.Models;
using System.Data;

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
            int musicID = 32;

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
        public ActionResult Index(String flags, String segmentID, String input)
        {
            if (flags == "" || input == "")
            {
                ViewBag.Error = "Please enter lyrics!";
                return View();

            }

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
            else if (flags.Equals("Lyrics"))
                inputProcessor.Lyrics(segment, input);

            
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
            IList<LyricSegment> lyricSeg = song.Lyrics.ToList();
            ViewBag.SegNum = lyricSeg.Count;
            int maxSeg = 0;

            foreach (LyricSegment ls in lyricSeg)
            {
                IList<LyricsStats> lyStat = db.LyricStats.Where(lstat => lstat.LyricSegmentID == ls.LyricSegmentID).ToList();
                if (lyStat.Count > maxSeg)
                {
                    maxSeg = lyStat.Count;
                }
            }
            ViewBag.MaxNumOfSegs = maxSeg;

            DataTable dt = new DataTable("StatLyrics");

            /*for (int i = 0; i < lyricSeg.Count; i++)
            {
                dt.Columns.Add(new DataColumn(i.ToString(),typeof(string)));
            }*/

            foreach (LyricSegment ls in lyricSeg)
            {
                dt.Columns.Add(new DataColumn(ls.LyricSegmentID.ToString(),typeof(string)));
                IList<LyricsStats> lyStat = db.LyricStats.Where(lstat => lstat.LyricSegmentID == ls.LyricSegmentID).ToList();
                int i = 0;
                foreach (LyricsStats lys in lyStat)
                {
                    if (i >= dt.Rows.Count)
                    {
                        DataRow row = dt.NewRow();
                        dt.Rows.Add(row);
                    }
                    dt.Rows[i][ls.LyricSegmentID.ToString()] = lys.Lyrics;
                    i++;
                }
            }

            /*for (int i = 0; i < maxSeg; i++)
            {
                DataRow row = dt.NewRow();
                dt.Rows.Add(row);
            }*/

            return PartialView("ResultSongPossibleLyrics", dt);
        }

        public ActionResult Results()
        {
            ViewBag.Message = "";

            //db.Database.SqlQuery(System.Type.GetType("String"), "select DISTINCT(genre) from Musics", null);

            return View(db.Music.ToList());
        }
    }
}
