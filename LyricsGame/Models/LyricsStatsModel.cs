using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LyricsGame.Models
{
    public class LyricsStats
    {
        public int LyricsStatsID{ get; set; }
        public int LyricSegmentID { get; set; }

        public String Lyrics { get; set; }
        public int Votes { get; set; }
        public int Count { get; set; }

        public virtual LyricSegment LyricSegment { get; set; }

    }
}