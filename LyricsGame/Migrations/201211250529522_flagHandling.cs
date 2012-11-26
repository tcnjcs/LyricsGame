namespace LyricsGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class flagHandling : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LyricSegments", "Complete", c => c.Boolean(nullable: false));
            AddColumn("dbo.LyricSegments", "CutOffCount", c => c.Int(nullable: false));
            AddColumn("dbo.LyricSegments", "OnlyMusicCount", c => c.Int(nullable: false));
            AlterColumn("dbo.LyricSegments", "Start", c => c.Double(nullable: false));
            AlterColumn("dbo.LyricSegments", "End", c => c.Double(nullable: false));
            DropColumn("dbo.LyricSegments", "Lyrics");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LyricSegments", "Lyrics", c => c.String());
            AlterColumn("dbo.LyricSegments", "End", c => c.Int(nullable: false));
            AlterColumn("dbo.LyricSegments", "Start", c => c.Int(nullable: false));
            DropColumn("dbo.LyricSegments", "OnlyMusicCount");
            DropColumn("dbo.LyricSegments", "CutOffCount");
            DropColumn("dbo.LyricSegments", "Complete");
        }
    }
}
