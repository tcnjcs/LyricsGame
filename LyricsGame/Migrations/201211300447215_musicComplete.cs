namespace LyricsGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class musicComplete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Musics", "Complete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Musics", "Complete");
        }
    }
}
