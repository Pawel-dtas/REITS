using Domain;
using REITs.Domain.Enums;
using REITs.Domain.Models;
using System.Data.Entity.Migrations;

namespace REITs.DataLayer.Seeds
{
    public static class AdjustmentTypeSeed
    {
        public static void Seed(Contexts.ApplicationContext context)
        {
            context.AdjustmentTypes.AddOrUpdate(new AdjustmentType
                                                {
                                                    AdjustmentCategory = "TestType",
                                                    AdjustmentName = "TestName"
                                                });
        }
    }
}
