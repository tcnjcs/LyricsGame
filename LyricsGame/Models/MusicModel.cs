using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LyricsGame.Models
{
    public class Music
    {
            public int MusicID { get; set; }
            public bool Complete { get; set; }
            public string Title { get; set; }
            public string Artist { get; set; }
            public string Genre { get; set; }
            public virtual ICollection<LyricSegment> Lyrics { get; set; }
            public string FilePath { get; set; }
    }


    public class MusicDBContext : DbContext
    {
        public DbSet<Music> Music { get; set; }
        public DbSet<LyricSegment> Lyrics { get; set; }
        public DbSet<LyricsStats> LyricStats { get; set; }
        public DbSet<LyricsUser> LyricUsers { get; set; }
        public DbSet<UserSegmentVotes> UserSegmentVotes { get; set; }
    }


}