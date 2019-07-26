namespace Practice.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isDeletedProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("Production.Product", "IsDeleted", c => c.Boolean(nullable: false));
            DropColumn("Production.ProductModelProductDescriptionCulture", "IsDeleted");
        }
        
        public override void Down()
        {
            AddColumn("Production.ProductModelProductDescriptionCulture", "IsDeleted", c => c.Boolean(nullable: false));
            DropColumn("Production.Product", "IsDeleted");
        }
    }
}
