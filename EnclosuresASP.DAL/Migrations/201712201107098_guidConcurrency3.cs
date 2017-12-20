namespace EnclosuresASP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guidConcurrency3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Blocks", "Version");
            DropColumn("dbo.Employes", "Version");
            DropColumn("dbo.Positions", "Version");
            DropColumn("dbo.EnclosureFiles", "Version");
            DropColumn("dbo.Enclosures", "Version");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Enclosures", "Version", c => c.Guid(nullable: false));
            AddColumn("dbo.EnclosureFiles", "Version", c => c.Guid(nullable: false));
            AddColumn("dbo.Positions", "Version", c => c.Guid(nullable: false));
            AddColumn("dbo.Employes", "Version", c => c.Guid(nullable: false));
            AddColumn("dbo.Blocks", "Version", c => c.Guid(nullable: false));
        }
    }
}
