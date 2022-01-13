namespace REITs.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _24074 : DbMigration
    {
        public override void Up()
        {
            AddColumn("REIT.REITParentReviewFS", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("REIT.REITParentReviewRFS", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("REIT.REITParents", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("REIT.REITParents", "IsActive");
            DropColumn("REIT.REITParentReviewRFS", "IsActive");
            DropColumn("REIT.REITParentReviewFS", "IsActive");
        }
    }
}
