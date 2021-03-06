﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LyricsGame.Models;
using Microsoft.WindowsAPICodePack.Shell;

namespace LyricsGame.Controllers
{
    [Authorize]
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
            //Error checking for input
            if(title == "" || artist == "" || genre == "" || mp3 == null)
            {
                ViewBag.Error = "Please ensure all fields are populated and a file has been selected";
                ViewBag.SongTitle = title;
                ViewBag.Artist = artist;
                ViewBag.Genre = genre;
                
                return View();
            
            }
            else if(!mp3.FileName.EndsWith(".mp3"))
            {
                ViewBag.Error = "At this time only files with a .mp3 file extension may be used";
                ViewBag.SongTitle = title;
                ViewBag.Artist = artist;
                ViewBag.Genre = genre;

                return View();
            }

            IList<Music> sameSongs = db.Music.Where(sl => sl.Title.Equals(title) && sl.Artist.Equals(artist)).ToList();
            if (sameSongs.Count() != 0)
            {
                ViewBag.Error = "This song has already been uploaded";
                ViewBag.SongTitle = title;
                ViewBag.Artist = artist;
                ViewBag.Genre = genre;

                return View();
            }
                

            //Create music object
            Music music = new Music
                {
                    Title = title,
                    Artist = artist,
                    Genre = genre,
                    Complete = false
                };

            //Add music object to db and save mp3 to listed application directory
            music.FilePath = "~/Content/MusicUploads/" + music.Artist + "-" + music.Title + ".mp3";
            db.Music.Add(music);
            if (mp3 != null)
                mp3.SaveAs(Request.PhysicalApplicationPath + "Content\\MusicUploads\\" + music.Artist + "-" + music.Title + ".mp3");

            //Calculate mp3 duration
            ShellFile f = ShellFile.FromFilePath(Request.PhysicalApplicationPath + "Content\\MusicUploads\\" + music.Artist + "-" + music.Title + ".mp3");
            double nanoseconds;
            double.TryParse(f.Properties.System.Media.Duration.Value.ToString(), out nanoseconds); 
            int duration = (int)(nanoseconds * 0.0000001);

            //Make sure song can be processed and is at least 10 seconds in length
            if (duration < 10)
            {
                ViewBag.Error = "Song is too short or has a corrupt duration header. Please upload another file";
                ViewBag.SongTitle = title;
                ViewBag.Artist = artist;
                ViewBag.Genre = genre;

                return View();
            }

            //Initialize values for first segment
            int segID = 0;
            int start = 0;
            int end = 5;
            int clipLength = 5;

            //Create all segments (except possibly last one)
            while (end <= duration) 
            {
                LyricSegment newSegment = new LyricSegment();
                newSegment.LyricSegmentID = segID;
                newSegment.MusicID = music.MusicID;
                newSegment.Start = start;
                newSegment.End = end;
                start += clipLength;
                end += clipLength;

                //If last segment wont be created, append to end of active segment
                if (end > duration)
                    newSegment.End = duration;

                db.Lyrics.Add(newSegment);
                segID++;
            }

            //Save changes to db
            db.SaveChanges();
            return RedirectToAction("Index");
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

        public ActionResult UploadComplete()
        {
            return View();
        }
    }
}