using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LyricsGame.Models
{
    public class Music
    {
            public int ID { get; set; }
            public string Title { get; set; }
            public string Artist { get; set; }
            public string Genre { get; set; }
            public string AllLyrics { get; set; }
            public string FilePath { get; set; }
    }

    public class MusicDBContext : DbContext
    {
        public DbSet<Music> Movies { get; set; }
    }
}