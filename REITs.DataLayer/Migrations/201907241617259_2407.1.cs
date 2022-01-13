namespace REITs.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _24071 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("REIT.REITParents", "MarketCapital", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("REIT.REITParents", "MarketCapital", c => c.String(maxLength: 20));
        }
    }
}
