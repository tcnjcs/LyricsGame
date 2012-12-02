namespace LyricsGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeCount : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.LyricsStats", "Count");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LyricsStats", "Count", c => c.Int(nullable: false));
        }
    }
}
