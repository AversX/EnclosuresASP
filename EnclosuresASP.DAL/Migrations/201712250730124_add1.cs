namespace EnclosuresASP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enclosures", "Object", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enclosures", "Object");
        }
    }
}
