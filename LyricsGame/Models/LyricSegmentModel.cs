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
        
        public double Start { get; set; }
        public double End { get; set; }

        public bool Complete { get; set; }
        public int CutOffCount { get; set; }
        public int OnlyMusicCount { get; set; }

        public List<String> Lyrics { get; set; }
        public virtual Music Music { get; set; }

    }
}