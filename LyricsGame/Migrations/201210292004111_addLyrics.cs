namespace LyricsGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addLyrics : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LyricSegments",
                c => new
                    {
                        LyricSegmentID = c.Int(nullable: false, identity: true),
                        MusicID = c.Int(nullable: false),
                        Start = c.Int(nullable: false),
                        End = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LyricSegmentID)
                .ForeignKey("dbo.Musics", t => t.MusicID, cascadeDelete: true)
                .Index(t => t.MusicID);
            
            AddColumn("dbo.Musics", "MusicID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Musics", "Title", c => c.String());
            AlterColumn("dbo.Musics", "FilePath", c => c.String());
            DropPrimaryKey("dbo.Musics", new[] { "ID" });
            AddPrimaryKey("dbo.Musics", "MusicID");
            DropColumn("dbo.Musics", "ID");
            DropColumn("dbo.Musics", "AllLyrics");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Musics", "AllLyrics", c => c.String());
            AddColumn("dbo.Musics", "ID", c => c.Int(nullable: false, identity: true));
            DropIndex("dbo.LyricSegments", new[] { "MusicID" });
            DropForeignKey("dbo.LyricSegments", "MusicID", "dbo.Musics");
            DropPrimaryKey("dbo.Musics", new[] { "MusicID" });
            AddPrimaryKey("dbo.Musics", "ID");
            AlterColumn("dbo.Musics", "FilePath", c => c.String(nullable: false));
            AlterColumn("dbo.Musics", "Title", c => c.String(nullable: false));
            DropColumn("dbo.Musics", "MusicID");
            DropTable("dbo.LyricSegments");
        }
    }
}
