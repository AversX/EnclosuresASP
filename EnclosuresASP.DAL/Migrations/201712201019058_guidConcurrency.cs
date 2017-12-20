namespace EnclosuresASP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guidConcurrency : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TypicalBlocks", "Version", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TypicalBlocks", "Version", c => c.Guid(nullable: false));
        }
    }
}
