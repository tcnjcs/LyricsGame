using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LyricsGame.Models
{
    public class InputProcessor
    {
        MusicDBContext db;
        public InputProcessor(MusicDBContext db)
        {
            this.db = db;
        }
        public void CutOff(LyricSegment segment, LyricSegment nextSegment)
        {
                segment.CutOffCount++;
                if (segment.CutOffCount % 5 == 0 && segment.CutOffCount < 20)
                {
                    segment.End += 0.5;
                    nextSegment.Start += 0.5;
                }
                else if(segment.CutOffCount >= 25)
                {
                    segment.End = nextSegment.End;

                    segment.LyricStats.Clear();
                    segment.LyricUsers.Clear();

                    segment.CutOffCount = 0;
                    segment.OnlyMusicCount = 0;

                    db.Lyrics.Remove(nextSegment);
                }
            db.SaveChanges();
        }

        public void NoLyrics(LyricSegment segment)
        {
            segment.OnlyMusicCount++;
            if (segment.OnlyMusicCount > 4)
                segment.Complete = true;

            db.SaveChanges();
        }
    }
}