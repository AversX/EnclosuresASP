namespace EnclosuresASP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guidConcurrency1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TypicalBlocks", "RowVersion", c => c.DateTime(nullable: false, precision: 0, storeType: "timestamp"));
            DropColumn("dbo.TypicalBlocks", "Version");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TypicalBlocks", "Version", c => c.Guid(nullable: false));
            DropColumn("dbo.TypicalBlocks", "RowVersion");
        }
    }
}
