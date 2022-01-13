namespace REITs.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _02101 : DbMigration
    {
        public override void Up()
        {
            AddColumn("REIT.REITParents", "SectorsFlag", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("REIT.REITParents", "SectorsFlag");
        }
    }
}
