namespace EnclosuresASP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guidConcurrency4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TypicalBlocks", "RowVersion", c => c.DateTime(nullable: false, precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TypicalBlocks", "RowVersion");
        }
    }
}
