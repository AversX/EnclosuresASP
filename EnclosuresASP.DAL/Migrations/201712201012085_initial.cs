namespace EnclosuresASP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blocks",
                c => new
                    {
                        BlockID = c.Int(nullable: false, identity: true),
                        UID = c.String(unicode: false),
                        EnclosureID = c.Int(nullable: false),
                        BlockGuid = c.Guid(nullable: false),
                        Version = c.Guid(nullable: false),
                        BlockName_TypicalBlockID = c.Int(),
                    })
                .PrimaryKey(t => t.BlockID)
                .ForeignKey("dbo.TypicalBlocks", t => t.BlockName_TypicalBlockID)
                .ForeignKey("dbo.Enclosures", t => t.EnclosureID, cascadeDelete: true)
                .Index(t => t.EnclosureID)
                .Index(t => t.BlockName_TypicalBlockID);
            
            CreateTable(
                "dbo.TypicalBlocks",
                c => new
                    {
                        TypicalBlockID = c.Int(nullable: false, identity: true),
                        BlockName = c.String(unicode: false),
                        Version = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.TypicalBlockID);
            
            CreateTable(
                "dbo.Employes",
                c => new
                    {
                        EmployeID = c.Int(nullable: false, identity: true),
                        FullName = c.String(unicode: false),
                        Version = c.Guid(nullable: false),
                        EmpPosition_PositionID = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeID)
                .ForeignKey("dbo.Positions", t => t.EmpPosition_PositionID)
                .Index(t => t.EmpPosition_PositionID);
            
            CreateTable(
                "dbo.Positions",
                c => new
                    {
                        PositionID = c.Int(nullable: false, identity: true),
                        PosName = c.String(unicode: false),
                        Version = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.PositionID);
            
            CreateTable(
                "dbo.EnclosureFiles",
                c => new
                    {
                        EnclosureFileID = c.Int(nullable: false, identity: true),
                        Bytes = c.Binary(),
                        Filename = c.String(unicode: false),
                        MimeType = c.String(unicode: false),
                        Temporary = c.Boolean(nullable: false),
                        Username = c.String(unicode: false),
                        EnclosureID = c.Int(nullable: false),
                        Version = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.EnclosureFileID)
                .ForeignKey("dbo.Enclosures", t => t.EnclosureID, cascadeDelete: true)
                .Index(t => t.EnclosureID);
            
            CreateTable(
                "dbo.Enclosures",
                c => new
                    {
                        EnclosureID = c.Int(nullable: false, identity: true),
                        Temporary = c.Boolean(nullable: false),
                        Number = c.String(nullable: false, unicode: false),
                        RootLogin = c.String(unicode: false),
                        RootPassword = c.String(unicode: false),
                        Username = c.String(unicode: false),
                        Version = c.Guid(nullable: false),
                        Supervisor_EmployeID = c.Int(),
                    })
                .PrimaryKey(t => t.EnclosureID)
                .ForeignKey("dbo.Employes", t => t.Supervisor_EmployeID)
                .Index(t => t.Supervisor_EmployeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Enclosures", "Supervisor_EmployeID", "dbo.Employes");
            DropForeignKey("dbo.EnclosureFiles", "EnclosureID", "dbo.Enclosures");
            DropForeignKey("dbo.Blocks", "EnclosureID", "dbo.Enclosures");
            DropForeignKey("dbo.Employes", "EmpPosition_PositionID", "dbo.Positions");
            DropForeignKey("dbo.Blocks", "BlockName_TypicalBlockID", "dbo.TypicalBlocks");
            DropIndex("dbo.Enclosures", new[] { "Supervisor_EmployeID" });
            DropIndex("dbo.EnclosureFiles", new[] { "EnclosureID" });
            DropIndex("dbo.Employes", new[] { "EmpPosition_PositionID" });
            DropIndex("dbo.Blocks", new[] { "BlockName_TypicalBlockID" });
            DropIndex("dbo.Blocks", new[] { "EnclosureID" });
            DropTable("dbo.Enclosures");
            DropTable("dbo.EnclosureFiles");
            DropTable("dbo.Positions");
            DropTable("dbo.Employes");
            DropTable("dbo.TypicalBlocks");
            DropTable("dbo.Blocks");
        }
    }
}
