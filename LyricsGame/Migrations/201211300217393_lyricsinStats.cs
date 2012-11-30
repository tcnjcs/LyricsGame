namespace LyricsGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lyricsinStats : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LyricsStats", "Lyrics", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LyricsStats", "Lyrics");
        }
    }
}
