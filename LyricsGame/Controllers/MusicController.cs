using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LyricsGame.Models;
using TagLib;

namespace LyricsGame.Controllers
{
    public class MusicController : Controller
    {
        private MusicDBContext db = new MusicDBContext();

        //
        // GET: /Music/

        public ActionResult Index()
        {
            return View(db.Music.ToList());
        }

        //
        // GET: /Music/Details/5

        public ActionResult Details(int id = 0)
        {
            Music music = db.Music.Find(id);
            if (music == null)
            {
                return HttpNotFound();
            }
            return View(music);
        }

        //
        // GET: /Music/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Music/Create

        [HttpPost]
        public ActionResult Create(String title, String artist, String genre, HttpPostedFileBase mp3)
        {
            Music music = new Music
                {
                    Title = title,
                    Artist = artist,
                    Genre = genre
                };
            if (ModelState.IsValid)
            {

                string path = Request.PhysicalApplicationPath + "Content\\MusicUploads\\";

                music.FilePath = path + music.Artist + "-" + music.Title + ".mp3";
                db.Music.Add(music);

                if (mp3 != null)
                    mp3.SaveAs(music.FilePath);

                TagLib.File f = TagLib.File.Create(music.FilePath);
                TimeSpan songSpan = f.Properties.Duration;
                int duration = songSpan.Seconds;

                int segID = 1;
                int start = 0;
                int end = 10;
                while (end <= duration) 
                {
                    LyricSegment newSegment = new LyricSegment();
                    newSegment.LyricSegmentID = segID;
                    newSegment.MusicID = music.MusicID;
                    newSegment.Start = start;
                    newSegment.End = end;
                    start += 10;
                    end += 10;

                    db.Lyrics.Add(newSegment);

                    segID++;
                }
                if (end != duration)
                {
                    LyricSegment newSegment = new LyricSegment();
                    newSegment.LyricSegmentID = segID;
                    newSegment.MusicID = music.MusicID;
                    newSegment.Start = start;
                    newSegment.End = duration;

                    db.Lyrics.Add(newSegment);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(music);
        }

        //
        // GET: /Music/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Music music = db.Music.Find(id);
            if (music == null)
            {
                return HttpNotFound();
            }
            return View(music);
        }

        //
        // POST: /Music/Edit/5

        [HttpPost]
        public ActionResult Edit(Music music)
        {
            if (ModelState.IsValid)
            {
                db.Entry(music).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(music);
        }

        //
        // GET: /Music/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Music music = db.Music.Find(id);
            if (music == null)
            {
                return HttpNotFound();
            }
            return View(music);
        }

        //
        // POST: /Music/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Music music = db.Music.Find(id);
            db.Music.Remove(music);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}