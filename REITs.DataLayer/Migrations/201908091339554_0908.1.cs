namespace REITs.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _09081 : DbMigration
    {
        public override void Up()
        {
            AddColumn("REIT.REITTotals", "BalanceOfBusinessPercentage", c => c.Double());
            AddColumn("REIT.REITTotals", "BalanceOfBusinessResult", c => c.String(maxLength: 10));
            DropColumn("REIT.REITTotals", "BalanaceOfBusinessPercentage");
            DropColumn("REIT.REITTotals", "BalanaceOfBusinessResult");
        }
        
        public override void Down()
        {
            AddColumn("REIT.REITTotals", "BalanaceOfBusinessResult", c => c.String(maxLength: 10));
            AddColumn("REIT.REITTotals", "BalanaceOfBusinessPercentage", c => c.Double());
            DropColumn("REIT.REITTotals", "BalanceOfBusinessResult");
            DropColumn("REIT.REITTotals", "BalanceOfBusinessPercentage");
        }
    }
}
