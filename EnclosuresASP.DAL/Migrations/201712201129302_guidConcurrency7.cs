namespace EnclosuresASP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guidConcurrency7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TypicalBlocks", "RowVersion", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TypicalBlocks", "RowVersion");
        }
    }
}
