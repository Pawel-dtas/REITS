using Domain;
using REITs.Domain.Enums;
using REITs.Domain.Models;
using System.Data.Entity.Migrations;


namespace REITs.DataLayer.Seeds
{
    public static class SystemUserSeed
    {
        public static void Seed(Contexts.ApplicationContext context)
        {
            context.SystemUsers.AddOrUpdate(
                new SystemUser
                {
                    Forename = "Andrew",
                    Surname = "Eskdale",
                    PINumber = "5305136",
                    TelephoneNumber = "03000 555555",
                    Team = "Team 1",                   
                    AccessLevel = BaseEnumExtension.GetDescriptionFromEnum(AccessLevels.Admin),
                    CreatedBy = "5305136",
                    UpdatedBy = "5305136"
                },
                new SystemUser
                {
                    Forename = "Sam",
                    Surname = "Constant",
                    PINumber = "7222369",
                    TelephoneNumber = "03000 444444",
                    Team = "Team 2",
                    AccessLevel = BaseEnumExtension.GetDescriptionFromEnum(AccessLevels.Admin),
                    CreatedBy = "5305136",
                    UpdatedBy = "5305136"
                }
            );
        }
    }
}
