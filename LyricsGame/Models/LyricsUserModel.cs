using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LyricsGame.Models
{
    public class LyricsUser
    {
        public int LyricsUserID { get; set; }
        public int LyricSegmentID { get; set; }

        public int Time { get; set; }
        public string Lyrics { get; set; }
        public int UserID { get; set; }

        public virtual LyricSegment LyricSegment { get; set; }

    }
}