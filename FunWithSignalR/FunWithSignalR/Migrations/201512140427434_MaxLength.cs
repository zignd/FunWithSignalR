namespace FunWithSignalR.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaxLength : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Connections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConnectionId = c.String(),
                        UserName = c.String(maxLength: 20),
                        IsOnline = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Connections", new[] { "UserName" });
            DropTable("dbo.Connections");
        }
    }
}
