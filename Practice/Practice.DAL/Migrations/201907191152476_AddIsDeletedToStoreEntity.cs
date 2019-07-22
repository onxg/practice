namespace Practice.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsDeletedToStoreEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("Sales.Store", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Sales.Store", "IsDeleted");
        }
    }
}
