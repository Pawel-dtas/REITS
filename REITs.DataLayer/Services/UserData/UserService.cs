using REITs.DataLayer.Contexts;
using REITs.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace REITs.DataLayer.Services
{
	public class UserDataService : IUserDataService
	{
		#region System Users

		public ICollection<SystemUser> GetAllActiveSystemUsers()
		{
			using (var context = new ApplicationContext())
			{
				return context.SystemUsers.Where(x => x.IsActive == true).OrderBy(x => x.Surname).ToList();
			}
		}

		public SystemUser GetSystemUser(Guid id)
		{
			throw new NotImplementedException();
		}

		public SystemUser GetSystemUser(string userPID)
		{
			using (var context = new ApplicationContext())
			{
				return context.SystemUsers.Where(a => a.PINumber == userPID).FirstOrDefault();

			}
		}

		public bool SaveSystemUser(SystemUser tempUser)
		{
            //tempUser.UpdatedBy = "7209233";         // SessionUserInfo - update
            //tempUser.DateUpdated = DateTime.Now;

            tempUser.CreatedBy = Environment.UserName;          // SessionUserInfo - update
            tempUser.DateCreated = DateTime.Now;

            using (var context = new ApplicationContext())
			{
				context.Entry(tempUser).State = EntityState.Added;

				return context.SaveChanges() == 1;
			}
		}

		public bool UpdateSystemUser(SystemUser tempUser)
		{
			//tempUser.UpdatedBy = "7209233";          // SessionUserInfo - update
			//tempUser.DateUpdated = DateTime.Now;

			tempUser.CreatedBy = Environment.UserName;          // SessionUserInfo - update
			tempUser.DateCreated = DateTime.Now;

			using (var context = new ApplicationContext())
			{
				context.Entry(tempUser).State = EntityState.Modified;

				return context.SaveChanges() == 1;
			}
		}

		public string GetUserName(string userPid)
		{
			string username = string.Empty;

			if (!string.IsNullOrEmpty(userPid))
			{
				using (var context = new ApplicationContext())
				{
					if (context.SystemUsers.Count() > 0)
					{
						var tempData = context.SystemUsers.Where(p => p.PINumber == userPid).FirstOrDefault();

						username = tempData.Forename + ", " + tempData.Surname;

						return username;
					} else
					{
						return string.Empty;
					}
				}
			} else
			{
				using (var context = new ApplicationContext())
				{
                    var tempData = context.SystemUsers.Where(p => p.PINumber == System.Environment.UserName).FirstOrDefault();
                    //var tempData = context.SystemUsers.Where(p => p.PINumber == "7209233").FirstOrDefault();

                    username = tempData.Forename + ", " + tempData.Surname;

					return username;
				}
			}
		}

		#endregion System Users
	}

	public class StaticUserList
	{
		public static IList<SystemUser> StaticListOfUsers { get; private set; }

		public static void PopulateListOfSystemUsers()
		{
			using (var context = new ApplicationContext())
			{
				StaticListOfUsers = context.SystemUsers.Where(x => x.IsActive == true).OrderBy(x => x.Surname).ToList();
			}
		}
	}
}