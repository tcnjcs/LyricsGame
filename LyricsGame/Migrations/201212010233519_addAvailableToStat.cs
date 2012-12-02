namespace LyricsGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAvailableToStat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LyricsStats", "Available", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LyricsStats", "Available");
        }
    }
}
