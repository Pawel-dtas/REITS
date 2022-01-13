namespace REITs.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _17071 : DbMigration
    {
        public override void Up()
        {
            AddColumn("REIT.REITParents", "LeftRegime", c => c.DateTime());
            DropColumn("REIT.REITParentReviewFS", "BOBRecDate");
            DropColumn("REIT.REITParentReviewFS", "FSReviewPlannedDate");
            DropColumn("REIT.REITParentReviewFS", "FSActualReviewedDate");
            DropColumn("REIT.REITParents", "CAFReview");
        }
        
        public override void Down()
        {
            AddColumn("REIT.REITParents", "CAFReview", c => c.String(maxLength: 5));
            AddColumn("REIT.REITParentReviewFS", "FSActualReviewedDate", c => c.DateTime());
            AddColumn("REIT.REITParentReviewFS", "FSReviewPlannedDate", c => c.DateTime());
            AddColumn("REIT.REITParentReviewFS", "BOBRecDate", c => c.DateTime());
            DropColumn("REIT.REITParents", "LeftRegime");
        }
    }
}
