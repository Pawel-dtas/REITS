using Domain.Models;
using REITs.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REITs.DataLayer.Services
{
	public interface IREITDataService
	{
		int CheckIfREITExists(string reitUTR, string reitName, DateTime apeDate);

		DateTime CheckIfREITExistsExactMatch(string reitUTR, string reitName, DateTime apeDate);

		REITParent CheckIfREITParentExists(string reitUTR, string reitParentName);

		Task<IList<REITParentReviewFS>> GetAllFSReviewsForParent(Guid parentGuid);

		Task<IList<REIT>> GetAllREITsForParent(Guid parentGuid);

		Task<IList<REITParentReviewRFS>> GetAllRFSReviewsForParent(Guid parentGuid);

		Task<REITParent> GetREITParentRecord(Guid guid);

		Task<REITParentReviewFS> GetREITParentReviewFSRecord(Guid guid);

		Task<REITParentReviewRFS> GetREITParentReviewRFSRecord(Guid guid);

		REIT GetREITRecord(Guid id, bool includeEntities = false);

		REITTotals GetREITTotalsRecord(Guid id);

		Task<string> GetUserFullnameAndPI(string pid);

		bool SaveREIT(REIT tempRIET);

		bool SaveREITParent(REITParent tempREITParent);

		bool SaveREITParentReviewFS(REITParentReviewFS tempREITParentReview);

		bool SaveREITParentReviewRFS(REITParentReviewRFS tempREITParentReviewRFS);

		bool SaveTotalsPIDScheduleConfirmed(REITTotals reitTotal);

		bool SaveUpdatedREITSubmissionDate(REIT reit);
	}
}