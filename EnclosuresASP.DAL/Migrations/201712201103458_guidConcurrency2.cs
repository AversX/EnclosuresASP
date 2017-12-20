namespace EnclosuresASP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guidConcurrency2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TypicalBlocks", "RowVersion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TypicalBlocks", "RowVersion", c => c.DateTime(nullable: false, precision: 0, storeType: "timestamp"));
        }
    }
}
