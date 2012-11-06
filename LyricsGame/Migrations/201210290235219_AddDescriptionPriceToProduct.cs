namespace LyricsGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionPriceToProduct : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Musics", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.Musics", "FilePath", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Musics", "FilePath", c => c.String());
            AlterColumn("dbo.Musics", "Title", c => c.String());
        }
    }
}
