namespace Practice.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnIsDeletedToTableEmployeeDepartmentHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("HumanResources.EmployeeDepartmentHistory", "isDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HumanResources.EmployeeDepartmentHistory", "isDeleted");
        }
    }
}
