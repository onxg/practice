namespace Practice.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveResetPasswordToken : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "ResetPasswordToken");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "ResetPasswordToken", c => c.String());
        }
    }
}
