namespace REITs.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _24073 : DbMigration
    {
        public override void Up()
        {
            AddColumn("REIT.REITParentReviewFS", "PIDDueDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("REIT.REITParentReviewFS", "PIDDueDate");
        }
    }
}
