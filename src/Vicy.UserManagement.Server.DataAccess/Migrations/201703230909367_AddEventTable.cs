namespace Vicy.UserManagement.Server.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEventTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Event",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DomainEvent = c.String(nullable: false, maxLength: 500),
                        EventType = c.String(nullable: false, maxLength: 2000),
                        IsPublished = c.Boolean(nullable: false),
                        CreateTimeUtc = c.DateTime(nullable: false),
                        UpdateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Event");
        }
    }
}
