namespace EnclosuresASP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guidConcurrency5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TypicalBlocks", "RowVersion", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TypicalBlocks", "RowVersion", c => c.DateTime(nullable: false, precision: 0));
        }
    }
}
