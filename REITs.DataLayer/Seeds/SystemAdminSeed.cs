using Domain;
using REITs.Domain.Enums;
using REITs.Domain.Models;
using System.Data.Entity.Migrations;

namespace REITs.DataLayer.Seeds
{
    public static class SystemAdminSeed
    {
        public static void Seed(Contexts.ApplicationContext context)
        {
            context.SystemAdmins.AddOrUpdate(
                new SystemAdmin
                {
                    AdminType = "Test AdminType",
                    Description = "Test Description",
                    Position = "1",
                    IsActive = true,
                    DateCreated = System.Convert.ToDateTime("10/08/2018"),
                    DateUpdated = System.Convert.ToDateTime("10/08/2018"),
                    CreatedBy = "5305136",
                    UpdatedBy = "5305136"

                }
                );
        }
    }
}
