using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LyricsGame.Models
{
    public class UserSegmentVotes
    {
        public int UserSegmentVotesID{ get; set; }

        public int LyricSegmentID { get; set; }
        public int LyricsStatsID { get; set; }
        public int UserID { get; set; }

        public virtual LyricsStats LyricsStats { get; set; }

    }
}