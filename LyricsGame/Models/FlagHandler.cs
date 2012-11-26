using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LyricsGame.Models
{
    public class FlagHandler
    {
        public static void CutOff(LyricSegment segment, LyricSegment nextSegment, MusicDBContext db)
        {
            if (nextSegment != null && nextSegment.MusicID == segment.MusicID)
            {
                if (segment.CutOffCount < 4)
                {
                    segment.End += 0.5;
                    nextSegment.Start += 0.5;
                    segment.CutOffCount++;
                }
                else
                {
                    segment.End = nextSegment.End;
                    if (segment.Lyrics != null)
                        segment.Lyrics.Clear();
                    segment.CutOffCount = 0;
                    segment.OnlyMusicCount = 0;

                    db.Lyrics.Remove(nextSegment);
                }
            }
            db.SaveChanges();
        }
    }
}