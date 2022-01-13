using Domain.Models;
using REITs.DataLayer.Contexts;
using REITs.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace REITs.DataLayer.Services
{
	public class REITDataService : IREITDataService
	{
		#region REITS

		public async Task<IList<REIT>> GetAllREITsForParent(Guid parentGuid)
		{
			using (var context = new ApplicationContext())
			{
				return await context.REITs.Where(x => x.ParentId == parentGuid).Distinct().OrderByDescending(x => x.DateCreated).OrderByDescending(x => x.XMLDateSubmitted).ToListAsync();
			}
		}

		public int CheckIfREITExists(string reitUTR, string reitName, DateTime apeDate)
		{
			int result = 0;

			using (var context = new ApplicationContext())
			{
				REIT tempReit = context.REITs.Where(a => a.PrincipalUTR == reitUTR && a.AccountPeriodEnd == apeDate).OrderByDescending(a => a.XMLVersion).FirstOrDefault();

				if (tempReit != null)
					result = tempReit.XMLVersion;

				tempReit = null;
			}

			return result;
		}

		public DateTime CheckIfREITExistsExactMatch(string reitUTR, string reitName, DateTime apeDate)
		{
			DateTime? result = DateTime.Now;

			using (var context = new ApplicationContext())
			{
				REIT tempReit = context.REITs.Where(a => a.PrincipalUTR == reitUTR && a.AccountPeriodEnd == apeDate).OrderByDescending(a => a.XMLVersion).FirstOrDefault();

				if (tempReit != null)
					result = tempReit.XMLDateSubmitted;

				tempReit = null;
			}

			return (DateTime)result;
		}

		public REIT GetREITRecord(Guid id, bool includeEntities = false)
		{
			using (var context = new ApplicationContext())
			{
				if (includeEntities == false)
				{
					var result = context.REITs.Where(a => a.Id == id).FirstOrDefault();
					return result;
				}
				else
				{
					var result = context.REITs.Where(a => a.Id == id).Include(e => e.Entities).Include(a => a.Reconciliations).Include(a => a.REITTotal).FirstOrDefault();
					result.Entities = context.Entities.Where(a => a.REITId == id).Include(s => s.Adjustments).ToList();

					return result;
				}
			}
		}

		public bool SaveREIT(REIT tempRIET)
		{
			bool saveSuccessful = false;

			using (var context = new ApplicationContext())
			{
				context.REITs.Add(tempRIET);

				context.Database.Log = Console.Write;

				try
				{
					saveSuccessful = (context.SaveChanges() > 0);
				}
				catch (DbEntityValidationException e)
				{
					ShowContextValidationErrors(e);
				}
				catch (Exception ex) { Debug.Print(ex.Message.ToString()); }

				return saveSuccessful;
			}
		}

		public bool SaveUpdatedREITSubmissionDate(REIT reit)
		{
			using (var context = new ApplicationContext())
			{
				context.REITs.Attach(reit);
				context.Entry(reit).Property(x => x.XMLDateSubmitted).IsModified = true;
				context.Entry(reit).Property(x => x.REITNotes).IsModified = true;

				try
				{
					return context.SaveChanges() > 0;
				}
				catch (Exception ex) { throw ex; }
			}
		}

		#endregion REITS

		#region REIT Parents

		public async Task<REITParent> GetREITParentRecord(Guid guid)
		{
			using (var context = new ApplicationContext())
			{
				return await context.REITParents.Where(a => a.Id == guid).FirstOrDefaultAsync();
			}
		}

		public async Task<string> GetUserFullnameAndPI(string pid)
		{
			using (var context = new ApplicationContext())
			{
				var tempUser = await context.SystemUsers.Where(s => s.PINumber == pid).FirstOrDefaultAsync();

				return tempUser?.FullNameAndPINumber;
			}
		}

		public REITParent CheckIfREITParentExists(string reitUTR, string reitParentName)
		{
			REITParent tempReitParent = null;

			using (var context = new ApplicationContext())
			{
				tempReitParent = context.REITParents.Where(a => a.PrincipalUTR == reitUTR).FirstOrDefault();
			}

			return tempReitParent;
		}

		public bool SaveREITParent(REITParent tempREITParent)
		{
			bool saveSuccessful = false;

			using (var context = new ApplicationContext())
			{
                tempREITParent.UpdatedBy = Environment.UserName;
                //tempREITParent.UpdatedBy = "7209233";
				tempREITParent.DateUpdated = DateTime.Now;

				if (string.IsNullOrEmpty(tempREITParent.CreatedBy))
				{
                    tempREITParent.CreatedBy = System.Environment.UserName;
                    //tempREITParent.CreatedBy = "7209233";
					tempREITParent.DateCreated = DateTime.Now;

					context.Entry(tempREITParent).State = EntityState.Added;
				}
				else
				{
					context.Entry(tempREITParent).State = EntityState.Modified;
				}

				try
				{
					saveSuccessful = (context.SaveChanges() > 0);
				}
				catch (DbEntityValidationException e)
				{
					ShowContextValidationErrors(e);
				}

				return saveSuccessful;
			}
		}

		private void ShowContextValidationErrors(DbEntityValidationException e)
		{
			foreach (var val in e.EntityValidationErrors)
			{
				Debug.Print("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
					val.Entry.Entity.GetType().Name, val.Entry.State);

				foreach (var ve in val.ValidationErrors)
				{
					Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
						ve.PropertyName, ve.ErrorMessage);
				}
			}
		}

		#endregion REIT Parents

		#region FS Reviews

		public async Task<REITParentReviewFS> GetREITParentReviewFSRecord(Guid guid)
		{
			using (var context = new ApplicationContext())
			{
				return await context.REITParentReviewsFS.Where(a => a.Id == guid).FirstOrDefaultAsync();
			}
		}

		private void SetEntityValue<T>(T currentEntity, string propertyName, object value)
		{
			currentEntity.GetType().GetProperty(propertyName).SetValue(currentEntity, value, null);
		}

		private object GetEntityValue<T>(T currentEntity, string propertyName)
		{
			return currentEntity.GetType().GetProperty(propertyName).GetValue(currentEntity, null);
		}

		private bool GenericSaveRecord(object currentEntity)
		{
			DateTime currentTime = DateTime.Now;

			bool saveSuccessful = false;

			using (var context = new ApplicationContext())
			{
                SetEntityValue(currentEntity, "UpdatedBy", Environment.UserName);
                //SetEntityValue(currentEntity, "UpdatedBy", "7209233");
				SetEntityValue(currentEntity, "DateUpdated", DateTime.Now);

				if (GetEntityValue(currentEntity, "CreatedBy") == null)
				{
                    SetEntityValue(currentEntity, "CreatedBy", Environment.UserName);
                    //SetEntityValue(currentEntity, "CreatedBy", "7209233");
					SetEntityValue(currentEntity, "DateCreated", DateTime.Now);

					context.Entry(currentEntity).State = EntityState.Added;
				}
				else
				{
					context.Entry(currentEntity).State = EntityState.Modified;
				}

				try
				{
					saveSuccessful = (context.SaveChanges() == 1);
				}
				catch (Exception ex) { Debug.Print(ex.Message.ToString()); }

				return saveSuccessful;
			}
		}

		public bool SaveREITParentReviewFS(REITParentReviewFS tempREITParentReview)
		{
			return GenericSaveRecord(tempREITParentReview);
		}

		public async Task<IList<REITParentReviewFS>> GetAllFSReviewsForParent(Guid parentGuid)
		{
			using (var context = new ApplicationContext())
			{
				return await context.REITParentReviewsFS.Where(x => x.ParentId == parentGuid).Where(X => X.IsActive == true).Distinct().OrderByDescending(x => x.FSAPEYear).ToListAsync();
			}
		}

		#endregion FS Reviews

		#region RFS Reviews

		public async Task<IList<REITParentReviewRFS>> GetAllRFSReviewsForParent(Guid parentGuid)
		{
			using (var context = new ApplicationContext())
			{
				return await context.REITParentReviewsRFS.Where(x => x.ParentId == parentGuid).Where(X => X.IsActive == true).Distinct().OrderByDescending(x => x.RFSAPEYear).ToListAsync();
			}
		}

		public async Task<REITParentReviewRFS> GetREITParentReviewRFSRecord(Guid guid)
		{
			using (var context = new ApplicationContext())
			{
				return await context.REITParentReviewsRFS.Where(a => a.Id == guid).FirstOrDefaultAsync();
			}
		}

		public bool SaveREITParentReviewRFS(REITParentReviewRFS tempREITParentReviewRFS)
		{
			return GenericSaveRecord(tempREITParentReviewRFS);
		}

		#endregion RFS Reviews

		#region REITTotal

		public REITTotals GetREITTotalsRecord(Guid id)
		{
			using (var context = new ApplicationContext())
			{
				var result = context.REITTotals.Where(a => a.REITId == id).FirstOrDefault();
				return result;
			}
		}

		public bool SaveTotalsPIDScheduleConfirmed(REITTotals reitTotal)
		{
			using (var context = new ApplicationContext())
			{
				context.REITTotals.Attach(reitTotal);
				context.Entry(reitTotal).Property(x => x.PaidDividendScheduleConfirmed).IsModified = true;

				try
				{
					return context.SaveChanges() == 1;
				}
				catch (Exception ex) { throw ex; }
			}
		}

		#endregion REITTotal
	}
}