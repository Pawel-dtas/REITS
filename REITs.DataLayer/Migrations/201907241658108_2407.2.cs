namespace REITs.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _24072 : DbMigration
    {
        public override void Up()
        {
            AddColumn("REIT.REITParents", "CTG7", c => c.String(maxLength: 7, fixedLength: true));
        }
        
        public override void Down()
        {
            DropColumn("REIT.REITParents", "CTG7");
        }
    }
}
