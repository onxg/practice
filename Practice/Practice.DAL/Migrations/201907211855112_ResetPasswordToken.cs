namespace Practice.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResetPasswordToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ResetPasswordToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ResetPasswordToken");
        }
    }
}
