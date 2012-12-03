namespace LyricsGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userSegmentVotes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserSegmentVotes",
                c => new
                    {
                        UserSegmentVotesID = c.Int(nullable: false, identity: true),
                        LyricSegmentID = c.Int(nullable: false),
                        LyricsStatsID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserSegmentVotesID)
                .ForeignKey("dbo.LyricsStats", t => t.LyricsStatsID, cascadeDelete: true)
                .Index(t => t.LyricsStatsID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserSegmentVotes", new[] { "LyricsStatsID" });
            DropForeignKey("dbo.UserSegmentVotes", "LyricsStatsID", "dbo.LyricsStats");
            DropTable("dbo.UserSegmentVotes");
        }
    }
}
