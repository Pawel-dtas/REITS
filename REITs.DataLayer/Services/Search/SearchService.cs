using REITs.DataLayer.Contexts;
using REITs.Domain.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace REITs.DataLayer.Services
{
	public class SearchService : ISearchService
	{
		#region Public Methods

		public ICollection<REIT> GetREITSearchResults(SearchOptionModel searchOptions)
		{
			return PerformREITSearch(searchOptions);
		}

		public ICollection<REITParent> GetREITParentSearchResults(SearchOptionModel searchOptions)
		{
			return PerformREITParentSearch(searchOptions);
		}

		public ICollection<Entity> GetEntitySearchResults(SearchOptionModel searchOptions)
		{
			return PerformEntitySearch(searchOptions);
		}

		#endregion Public Methods

		#region Constructor

		public SearchService()
		{ }

		#endregion Constructor

		#region Private Methods

		private List<REITParent> PerformREITParentSearch(SearchOptionModel searchOptions)
		{
			List<REITParent> searchResults = new List<REITParent>();

			searchResults = REITParentSearch(searchOptions);

			if (searchOptions.SearchUTR != null)
				searchResults = searchResults.Where(r => r.PrincipalUTR == searchOptions.SearchUTR).ToList();

			if (searchResults.Count > 0)
			{
				if (searchOptions.SearchAPEFrom != null && searchOptions.SearchAPETo != null)
				{
					//searchResults = searchResults.Where(r => r.APEDate.Value.Year >= searchOptions.SearchAPEFrom.Value.Year
					//									&& r.APEDate.Value.Year <= searchOptions.SearchAPETo.Value.Year).ToList();

					searchResults = searchResults.Where(r => r.APEDate.Value.Month >= searchOptions.SearchAPEFrom.Value.Month
														&& r.APEDate.Value.Month <= searchOptions.SearchAPETo.Value.Month).ToList();
				}

				if (searchOptions.SearchPAPEFrom != null && searchOptions.SearchPAPETo != null)
				{
					//searchResults = searchResults.Where(r => r.APEDate.Value.Year >= searchOptions.SearchPAPEFrom.Value.Year
					//									&& r.APEDate.Value.Year <= searchOptions.SearchPAPETo.Value.Year).ToList();

					searchResults = searchResults.Where(r => r.APEDate.Value.Month >= searchOptions.SearchPAPEFrom.Value.Month
													&& r.APEDate.Value.Month <= searchOptions.SearchPAPETo.Value.Month).ToList();
				}
			}

			return searchResults;
		}

		private List<REIT> PerformREITSearch(SearchOptionModel searchOptions)
		{
			List<REIT> searchResults = new List<REIT>();

			if (searchOptions.SearchUTR != null)
			{
				searchResults = REITUTRSearch(searchOptions);
			} else
			{
				searchResults = REITNameSearch(searchOptions);
			}

			if (searchResults.Count > 0)
			{
				if (searchOptions.SearchAPEFrom != null && searchOptions.SearchAPETo != null)
				{
					if (searchOptions.SearchAPEFrom != null && searchOptions.SearchAPETo != null)
					{
						searchResults = searchResults.Where(r => r.AccountPeriodEnd.Year >= searchOptions.SearchAPEFrom.Value.Year
															&& r.AccountPeriodEnd.Year <= searchOptions.SearchAPETo.Value.Year).ToList();

						searchResults = searchResults.Where(r => r.AccountPeriodEnd.Month >= searchOptions.SearchAPEFrom.Value.Month
															&& r.AccountPeriodEnd.Month <= searchOptions.SearchAPETo.Value.Month).ToList();
					}

					if (searchOptions.SearchPAPEFrom != null && searchOptions.SearchPAPETo != null)
					{
						searchResults = searchResults.Where(r => r.PreviousAccountPeriodEnd.Year >= searchOptions.SearchPAPEFrom.Value.Year
															&& r.PreviousAccountPeriodEnd.Year <= searchOptions.SearchPAPETo.Value.Year).ToList();

						searchResults = searchResults.Where(r => r.PreviousAccountPeriodEnd.Month >= searchOptions.SearchPAPEFrom.Value.Month
														&& r.PreviousAccountPeriodEnd.Month <= searchOptions.SearchPAPETo.Value.Month).ToList();
					}
				}
			}

			return searchResults;
		}

		private List<Entity> PerformEntitySearch(SearchOptionModel searchOptions)
		{
			List<Entity> searchResults = new List<Entity>();

			searchResults = EntityNameSearch(searchOptions);

			if (searchOptions.SearchUTR != null)
				searchResults = EntityUTRSearch(searchOptions);

			if (searchOptions.SearchCustomerReference != null)
				searchResults = EntityCustomerReferenceSearch(searchOptions);

			return searchResults;
		}

		// REIT CONTEXT SEARCH METHODS

		private List<REIT> REITNameSearch(SearchOptionModel searchOptions)
		{
			List<REIT> searchResults = new List<REIT>();

			using (var context = new ApplicationContext())
			{
				if (string.IsNullOrEmpty(searchOptions.SearchName))
				{
					searchResults = context.REITs.OrderBy(a => a.REITName).ToList();
				} else
				{
					searchResults = context.REITs.Where(a => a.REITName.StartsWith(searchOptions.SearchName)).ToList();
				}
			}

			return searchResults;
		}

		private List<REIT> REITUTRSearch(SearchOptionModel searchOptions)
		{
			using (var context = new ApplicationContext())
			{
				return context.REITs.Where(a => a.PrincipalUTR == searchOptions.SearchUTR).ToList();
			}
		}

		private List<REITParent> REITParentSearch(SearchOptionModel searchOptions)
		{
			List<REITParent> searchResults = new List<REITParent>();

			using (var context = new ApplicationContext())
			{
				if (string.IsNullOrEmpty(searchOptions.SearchName))
				{
					searchResults = context.REITParents.OrderBy(a => a.PrincipalCustomerName).ToList();
				} else
				{
					searchResults = context.REITParents.Where(a => a.PrincipalCustomerName.StartsWith(searchOptions.SearchName)).ToList();
				}
			}

			return searchResults;
		}

		// ENTITY CONTEXT SEARCH METHODS

		private List<Entity> EntityNameSearch(SearchOptionModel searchOptions)
		{
			List<Entity> searchResults = new List<Entity>();

			using (var context = new ApplicationContext())
			{
				if (string.IsNullOrEmpty(searchOptions.SearchName))
				{
					searchResults = context.Entities.Include(a => a.REIT).OrderBy(a => a.EntityName).ToList();
				} else
				{
					searchResults = context.Entities.Include(a => a.REIT).Where(a => a.EntityName.StartsWith(searchOptions.SearchName)).ToList();
				}
			}

			return searchResults;
		}

		private List<Entity> EntityCustomerReferenceSearch(SearchOptionModel searchOptions)
		{
			using (var context = new ApplicationContext())
			{
				return context.Entities.Include(a => a.REIT).Where(x => x.CustomerReference == searchOptions.SearchCustomerReference).ToList();
			}
		}

		private List<Entity> EntityUTRSearch(SearchOptionModel searchOptions)
		{
			using (var context = new ApplicationContext())
			{
				return context.Entities.Include(a => a.REIT).Where(a => a.EntityUTR == searchOptions.SearchUTR).ToList();
			}
		}

		#endregion Private Methods
	}
}