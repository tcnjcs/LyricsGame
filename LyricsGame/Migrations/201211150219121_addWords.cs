namespace LyricsGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addWords : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LyricSegments", "Lyrics", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LyricSegments", "Lyrics");
        }
    }
}
