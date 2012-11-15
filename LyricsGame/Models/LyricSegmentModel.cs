using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LyricsGame.Models
{
    public class LyricSegment
    {
        public int LyricSegmentID { get; set; }
        public int MusicID { get; set; }
        
        public int Start { get; set; }
        public int End { get; set; }
        public String Lyrics { get; set; }
        public virtual Music Music { get; set; }

    }
}