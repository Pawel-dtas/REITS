namespace REITs.DataLayer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class _10071 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "REIT.Adjustments",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    AdjustmentCategory = c.String(nullable: false, maxLength: 100),
                    AdjustmentType = c.String(nullable: false, maxLength: 100),
                    AdjustmentName = c.String(nullable: false, maxLength: 100),
                    AdjustmentAmount = c.Double(nullable: false),
                    EntityId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("REIT.Entities", t => t.EntityId, cascadeDelete: true)
                .Index(t => t.EntityId);

            CreateTable(
                "REIT.Entities",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    EntityName = c.String(nullable: false, maxLength: 100),
                    EntityType = c.String(nullable: false, maxLength: 100),
                    EntityUTR = c.String(maxLength: 10),
                    InterestPercentage = c.Double(nullable: false),
                    Jurisdiction = c.String(nullable: false, maxLength: 50),
                    TaxExempt = c.Boolean(nullable: false),
                    CustomerReference = c.String(maxLength: 20),
                    REITId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("REIT.REITs", t => t.REITId, cascadeDelete: true)
                .Index(t => t.REITId);

            CreateTable(
                "REIT.REITs",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    ParentId = c.Guid(nullable: false),
                    REITName = c.String(nullable: false, maxLength: 100),
                    PrincipalUTR = c.String(nullable: false, maxLength: 10),
                    AccountPeriodEnd = c.DateTime(nullable: false),
                    PreviousAccountPeriodEnd = c.DateTime(nullable: false),
                    REITNotes = c.String(),
                    XMLDateSubmitted = c.DateTime(nullable: false),
                    XMLVersion = c.Int(nullable: false),
                    CreatedBy = c.String(nullable: false, maxLength: 7, fixedLength: true),
                    DateCreated = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("REIT.REITParents", t => t.ParentId)
                .Index(t => t.ParentId);

            CreateTable(
                "REIT.Reconciliations",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    ReconciliationType = c.String(nullable: false, maxLength: 100),
                    ReconciliationName = c.String(nullable: false, maxLength: 100),
                    ReconciliationAmount = c.Double(nullable: false),
                    REITId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("REIT.REITs", t => t.REITId, cascadeDelete: true)
                .Index(t => t.REITId);

            CreateTable(
                "REIT.REITTotals",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    IncomeTestPercentage = c.Double(),
                    IncomeTestResult = c.String(nullable: false, maxLength: 10),
                    AssetTestPercentage = c.Double(),
                    AssetTestResult = c.String(maxLength: 10),
                    PIDDistribution90Amount = c.Double(),
                    PIDDistribution100Amount = c.Double(),
                    InterestCoverRatioTestPercentage = c.Double(),
                    InterestCoverRatioTestResult = c.String(maxLength: 10),
                    PaidDividendScheduleConfirmed = c.String(maxLength: 10),
                    BalanaceOfBusinessPercentage = c.Double(),
                    BalanaceOfBusinessResult = c.String(maxLength: 10),
                    PropCombinedRevenueLessCostOfSales = c.Double(),
                    PropUKRevenueLessCostOfSales = c.Double(),
                    PropNonUKRevenueLessCostOfSales = c.Double(),
                    PropCombinedPIDs = c.Double(),
                    PropUKPIDs = c.Double(),
                    PropNonUKPIDs = c.Double(),
                    PropCombinedOtherIncomeOrExpenseAmount = c.Double(),
                    PropUKOtherIncomeOrExpenseAmount = c.Double(),
                    PropNonUKOtherIncomeOrExpenseAmount = c.Double(),
                    PropCombinedNetFinanceCosts = c.Double(),
                    PropUKNetFinanceCosts = c.Double(),
                    PropNonUKNetFinanceCosts = c.Double(),
                    PropCombinedOtherAdjustmentsAmount = c.Double(),
                    PropUKOtherAdjustmentsAmount = c.Double(),
                    PropNonUKOtherAdjustmentsAmount = c.Double(),
                    PropCombinedNonCurrentAssets = c.Double(),
                    PropUKNonCurrentAssets = c.Double(),
                    PropNonUKNonCurrentAssets = c.Double(),
                    PropCombinedCurrentAssets = c.Double(),
                    PropUKCurrentAssets = c.Double(),
                    PropNonUKCurrentAssets = c.Double(),
                    ResCombinedRevenueLessCostOfSales = c.Double(),
                    ResUKRevenueLessCostOfSales = c.Double(),
                    ResNonUKRevenueLessCostOfSales = c.Double(),
                    ResCombinedBeneficialInterestsIncome = c.Double(),
                    ResUKBeneficialInterestsIncome = c.Double(),
                    ResNonUKBeneficialInterestsIncome = c.Double(),
                    ResCombinedOtherIncomeOrExpenseAmount = c.Double(),
                    ResUKOtherIncomeOrExpenseAmount = c.Double(),
                    ResNonUKOtherIncomeOrExpenseAmount = c.Double(),
                    ResCombinedNetFinanceCosts = c.Double(),
                    ResUKNetFinanceCosts = c.Double(),
                    ResNonUKNetFinanceCosts = c.Double(),
                    ResCombinedOtherAdjustmentsAmount = c.Double(),
                    ResUKOtherAdjustmentsAmount = c.Double(),
                    ResNonUKOtherAdjustmentsAmount = c.Double(),
                    ResCombinedNonCurrentAssets = c.Double(),
                    ResUKNonCurrentAssets = c.Double(),
                    ResNonUKNonCurrentAssets = c.Double(),
                    ResCombinedCurrentAssets = c.Double(),
                    ResUKCurrentAssets = c.Double(),
                    ResNonUKCurrentAssets = c.Double(),
                    TaxExCombinedPRBProfitsBeforeTax = c.Double(nullable: false),
                    TaxExUKPRBProfitsBeforeTax = c.Double(nullable: false),
                    TaxExNonUKPRBProfitsBeforeTax = c.Double(nullable: false),
                    TaxExCombinedPRBIntAndFCsReceivable = c.Double(),
                    TaxExUKPRBIntAndFCsReceivable = c.Double(),
                    TaxExNonUKPRBIntAndFCsReceivable = c.Double(),
                    TaxExCombinedPRBIntAndFCsPayable = c.Double(),
                    TaxExUKPRBIntAndFCsPayable = c.Double(),
                    TaxExNonUKPRBIntAndFCsPayable = c.Double(),
                    TaxExCombinedPRBHedgingDerivatives = c.Double(),
                    TaxExUKPRBHedgingDerivatives = c.Double(),
                    TaxExNonUKPRBHedgingDerivatives = c.Double(),
                    TaxExCombinedPRBResidualIncome = c.Double(),
                    TaxExUKPRBResidualIncome = c.Double(),
                    TaxExNonUKPRBResidualIncome = c.Double(),
                    TaxExCombinedPRBResidualExpenses = c.Double(),
                    TaxExUKPRBResidualExpenses = c.Double(),
                    TaxExNonUKPRBResidualExpenses = c.Double(),
                    TaxExCombinedPBTAdjustments = c.Double(),
                    TaxExUKPBTAdjustments = c.Double(),
                    TaxExNonUKPBTAdjustments = c.Double(),
                    TaxExCombinedUKPRBProfits = c.Double(),
                    TaxExUKUKPRBProfits = c.Double(),
                    TaxExNonUKUKPRBProfits = c.Double(),
                    TaxExCombinedPRBFinanceCosts = c.Double(),
                    TaxExUKPRBFinanceCosts = c.Double(),
                    TaxExNonUKPRBFinanceCosts = c.Double(),
                    TaxExCombinedIntAndFCsReceivable = c.Double(),
                    TaxExUKIntAndFCsReceivable = c.Double(),
                    TaxExNonUKIntAndFCsReceivable = c.Double(),
                    TaxExCombinedIntAndFCsPayable = c.Double(),
                    TaxExUKIntAndFCsPayable = c.Double(),
                    TaxExNonUKIntAndFCsPayable = c.Double(),
                    TaxExCombinedHedgingDerivatives = c.Double(),
                    TaxExUKHedgingDerivatives = c.Double(),
                    TaxExNonUKHedgingDerivatives = c.Double(),
                    TaxExCombinedOtherClaims = c.Double(),
                    TaxExUKOtherClaims = c.Double(),
                    TaxExNonUKOtherClaims = c.Double(),
                    TaxExCombinedCapitalAllowances = c.Double(),
                    TaxExUKCapitalAllowances = c.Double(),
                    TaxExNonUKCapitalAllowances = c.Double(),
                    TaxExCombinedOtherTaxAdjustments = c.Double(),
                    TaxExUKOtherTaxAdjustments = c.Double(),
                    TaxExNonUKOtherTaxAdjustments = c.Double(),
                    TaxExCombinedUKPropertyBroughtFwd = c.Double(),
                    TaxExUKUKPropertyBroughtFwd = c.Double(),
                    TaxExNonUKUKPropertyBroughtFwd = c.Double(),
                    TaxExCombinedProfitsExREITSInvProfits = c.Double(),
                    TaxExUKProfitsExREITSInvProfits = c.Double(),
                    TaxExNonUKProfitsExREITSInvProfits = c.Double(),
                    TaxExCombinedREITSInvProfits = c.Double(),
                    TaxExUKREITSInvProfits = c.Double(),
                    TaxExNonUKREITSInvProfits = c.Double(),
                    PBTReconsToAuditedFinancialStatement = c.Double(),
                    ReconsToAuditedFinancialStatement = c.Double(),
                    REITId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("REIT.REITs", t => t.REITId, cascadeDelete: true)
                .Index(t => t.REITId);

            CreateTable(
                "REIT.AdjustmentTypes",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    AdjustmentCategory = c.String(nullable: false, maxLength: 100),
                    AdjustmentName = c.String(nullable: false, maxLength: 100),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "REIT.REITParentReviewFS",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    ParentId = c.Guid(nullable: false),
                    FSAPEYear = c.DateTime(nullable: false),
                    FSDue = c.DateTime(),
                    PIDRecDate = c.DateTime(),
                    FSRecDate = c.DateTime(),
                    BOBRecDate = c.DateTime(),
                    FSReviewPlannedDate = c.DateTime(),
                    FSActualReviewedDate = c.DateTime(),
                    Comments = c.String(),
                    DateCreated = c.DateTime(nullable: false),
                    CreatedBy = c.String(nullable: false, maxLength: 7, fixedLength: true),
                    DateUpdated = c.DateTime(nullable: false),
                    UpdatedBy = c.String(nullable: false, maxLength: 7, fixedLength: true),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "REIT.REITParentReviewRFS",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    ParentId = c.Guid(nullable: false),
                    RFSAPEYear = c.DateTime(nullable: false),
                    FSReviewedAPEDate = c.DateTime(),
                    RiskStatus = c.String(maxLength: 10),
                    OnBRRTT = c.String(maxLength: 20),
                    InternalBRRDueDate = c.DateTime(),
                    RFSReviewedDate = c.DateTime(),
                    RAPlanMeetDate = c.DateTime(),
                    ReviewedDate = c.DateTime(),
                    NextReviewDate = c.DateTime(),
                    Comments = c.String(),
                    DateCreated = c.DateTime(nullable: false),
                    CreatedBy = c.String(nullable: false, maxLength: 7, fixedLength: true),
                    DateUpdated = c.DateTime(nullable: false),
                    UpdatedBy = c.String(nullable: false, maxLength: 7, fixedLength: true),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "REIT.REITParents",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    PrincipalCustomerName = c.String(nullable: false, maxLength: 100),
                    PrincipalUTR = c.String(nullable: false, maxLength: 10),
                    TaxExemptUTR = c.String(maxLength: 10),
                    APEDate = c.DateTime(nullable: false),
                    ConversionDate = c.DateTime(),
                    LastBRRDate = c.DateTime(),
                    NewReit = c.String(maxLength: 5),
                    NextBRRDate = c.DateTime(),
                    MarketsListedOn = c.String(maxLength: 250),
                    MarketCapital = c.String(maxLength: 20),
                    MarketInfo = c.String(maxLength: 250),
                    CCM = c.String(maxLength: 7, fixedLength: true),
                    CoOrd = c.String(maxLength: 7, fixedLength: true),
                    CTHO = c.String(maxLength: 7, fixedLength: true),
                    InformedConsent = c.String(maxLength: 5),
                    SAO = c.String(maxLength: 5),
                    CAFReview = c.String(maxLength: 5),
                    Notes = c.String(),
                    DateCreated = c.DateTime(nullable: false),
                    CreatedBy = c.String(nullable: false, maxLength: 7, fixedLength: true),
                    DateUpdated = c.DateTime(nullable: false),
                    UpdatedBy = c.String(nullable: false, maxLength: 7, fixedLength: true),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "REIT.SystemAdmins",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    AdminType = c.String(nullable: false, maxLength: 30),
                    Description = c.String(nullable: false, maxLength: 50),
                    Position = c.String(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    DateCreated = c.DateTime(nullable: false),
                    DateUpdated = c.DateTime(nullable: false),
                    CreatedBy = c.String(nullable: false, maxLength: 7, fixedLength: true),
                    UpdatedBy = c.String(nullable: false, maxLength: 7, fixedLength: true),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "REIT.SystemUsers",
                c => new
                {
                    Id = c.Guid(nullable: false, identity: true),
                    PINumber = c.String(nullable: false, maxLength: 7),
                    TelephoneNumber = c.String(nullable: false, maxLength: 30),
                    Team = c.String(maxLength: 50),
                    JobRole = c.String(maxLength: 50),
                    AccessLevel = c.String(nullable: false, maxLength: 20),
                    CreatedBy = c.String(nullable: false, maxLength: 7, fixedLength: true),
                    DateCreated = c.DateTime(),
                    UpdatedBy = c.String(nullable: false, maxLength: 7, fixedLength: true),
                    DateUpdated = c.DateTime(),
                    IsActive = c.Boolean(nullable: false),
                    Forename = c.String(nullable: false, maxLength: 30),
                    Surname = c.String(nullable: false, maxLength: 30),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropForeignKey("REIT.REITs", "REITParent_Id", "REIT.REITParents");
            DropForeignKey("REIT.REITTotals", "REITId", "REIT.REITs");
            DropForeignKey("REIT.Reconciliations", "REITId", "REIT.REITs");
            DropForeignKey("REIT.Entities", "REITId", "REIT.REITs");
            DropForeignKey("REIT.Adjustments", "EntityId", "REIT.Entities");
            DropIndex("REIT.REITTotals", new[] { "REITId" });
            DropIndex("REIT.Reconciliations", new[] { "REITId" });
            DropIndex("REIT.REITs", new[] { "REITParent_Id" });
            DropIndex("REIT.Entities", new[] { "REITId" });
            DropIndex("REIT.Adjustments", new[] { "EntityId" });
            DropTable("REIT.SystemUsers");
            DropTable("REIT.SystemAdmins");
            DropTable("REIT.REITParents");
            DropTable("REIT.REITParentReviewRFS");
            DropTable("REIT.REITParentReviewFS");
            DropTable("REIT.AdjustmentTypes");
            DropTable("REIT.REITTotals");
            DropTable("REIT.Reconciliations");
            DropTable("REIT.REITs");
            DropTable("REIT.Entities");
            DropTable("REIT.Adjustments");
        }
    }
}