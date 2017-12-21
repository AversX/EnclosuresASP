namespace EnclosuresASP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guidConcurrency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blocks", "Version", c => c.Guid(nullable: false));
            AddColumn("dbo.Employes", "Version", c => c.Guid(nullable: false));
            AddColumn("dbo.Positions", "Version", c => c.Guid(nullable: false));
            AddColumn("dbo.EnclosureFiles", "Version", c => c.Guid(nullable: false));
            AddColumn("dbo.Enclosures", "Version", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enclosures", "Version");
            DropColumn("dbo.EnclosureFiles", "Version");
            DropColumn("dbo.Positions", "Version");
            DropColumn("dbo.Employes", "Version");
            DropColumn("dbo.Blocks", "Version");
        }
    }
}
