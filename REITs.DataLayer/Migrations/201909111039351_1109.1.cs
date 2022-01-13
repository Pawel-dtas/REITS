namespace REITs.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _11091 : DbMigration
    {
        public override void Up()
        {
            DropColumn("REIT.REITTotals", "BalanceOfBusinessPercentage");
            DropColumn("REIT.REITTotals", "BalanceOfBusinessResult");
        }
        
        public override void Down()
        {
            AddColumn("REIT.REITTotals", "BalanceOfBusinessResult", c => c.String(maxLength: 10));
            AddColumn("REIT.REITTotals", "BalanceOfBusinessPercentage", c => c.Double());
        }
    }
}
