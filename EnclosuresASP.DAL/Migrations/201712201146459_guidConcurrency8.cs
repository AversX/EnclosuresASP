namespace EnclosuresASP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guidConcurrency8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TypicalBlocks", "Version", c => c.Guid(nullable: false));
            DropColumn("dbo.TypicalBlocks", "RowVersion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TypicalBlocks", "RowVersion", c => c.Guid(nullable: false));
            DropColumn("dbo.TypicalBlocks", "Version");
        }
    }
}
