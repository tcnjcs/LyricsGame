namespace LyricsGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixDBcontext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LyricsStats",
                c => new
                    {
                        LyricsStatsID = c.Int(nullable: false, identity: true),
                        LyricSegmentID = c.Int(nullable: false),
                        Votes = c.Int(nullable: false),
                        Count = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LyricsStatsID)
                .ForeignKey("dbo.LyricSegments", t => t.LyricSegmentID, cascadeDelete: true)
                .Index(t => t.LyricSegmentID);
            
            CreateTable(
                "dbo.LyricsUsers",
                c => new
                    {
                        LyricsUserID = c.Int(nullable: false, identity: true),
                        LyricSegmentID = c.Int(nullable: false),
                        Time = c.Int(nullable: false),
                        Lyrics = c.String(),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LyricsUserID)
                .ForeignKey("dbo.LyricSegments", t => t.LyricSegmentID, cascadeDelete: true)
                .Index(t => t.LyricSegmentID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.LyricsUsers", new[] { "LyricSegmentID" });
            DropIndex("dbo.LyricsStats", new[] { "LyricSegmentID" });
            DropForeignKey("dbo.LyricsUsers", "LyricSegmentID", "dbo.LyricSegments");
            DropForeignKey("dbo.LyricsStats", "LyricSegmentID", "dbo.LyricSegments");
            DropTable("dbo.LyricsUsers");
            DropTable("dbo.LyricsStats");
        }
    }
}
