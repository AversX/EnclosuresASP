namespace EnclosuresASP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enclosures", "ElisNumber", c => c.String(unicode: false));
            AddColumn("dbo.Enclosures", "AcceptanceDate", c => c.String(unicode: false));
            AddColumn("dbo.Enclosures", "Lvl1Password", c => c.String(unicode: false));
            AddColumn("dbo.Enclosures", "Lvl2Password", c => c.String(unicode: false));
            AddColumn("dbo.Enclosures", "Lvl3Password", c => c.String(unicode: false));
            AddColumn("dbo.Enclosures", "Lvl4Password", c => c.String(unicode: false));
            AddColumn("dbo.Enclosures", "Lvl5Password", c => c.String(unicode: false));
            AlterColumn("dbo.Enclosures", "Number", c => c.String(unicode: false));
            DropColumn("dbo.Enclosures", "RootLogin");
            DropColumn("dbo.Enclosures", "RootPassword");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Enclosures", "RootPassword", c => c.String(unicode: false));
            AddColumn("dbo.Enclosures", "RootLogin", c => c.String(unicode: false));
            AlterColumn("dbo.Enclosures", "Number", c => c.String(nullable: false, unicode: false));
            DropColumn("dbo.Enclosures", "Lvl5Password");
            DropColumn("dbo.Enclosures", "Lvl4Password");
            DropColumn("dbo.Enclosures", "Lvl3Password");
            DropColumn("dbo.Enclosures", "Lvl2Password");
            DropColumn("dbo.Enclosures", "Lvl1Password");
            DropColumn("dbo.Enclosures", "AcceptanceDate");
            DropColumn("dbo.Enclosures", "ElisNumber");
        }
    }
}
