namespace Practice.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class all : DbMigration
    {
        public override void Up()
        {
            AddColumn("HumanResources.EmployeeDepartmentHistory", "isDeleted", c => c.Boolean(nullable: false));
            AddColumn("Production.ProductModelProductDescriptionCulture", "IsDeleted", c => c.Boolean(nullable: false));
           
        }
        
        public override void Down()
        {
            
            DropColumn("Production.ProductModelProductDescriptionCulture", "IsDeleted");
            DropColumn("HumanResources.EmployeeDepartmentHistory", "isDeleted");
        }
    }
}
