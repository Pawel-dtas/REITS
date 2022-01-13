using REITs.Domain.Models;
using System.Collections.Generic;

namespace REITs.DataLayer.Services
{
    public interface ISearchService
    {
        ICollection<REIT> GetREITSearchResults(SearchOptionModel searchOptions);

        ICollection<REITParent> GetREITParentSearchResults(SearchOptionModel searchOptions);

        ICollection<Entity> GetEntitySearchResults(SearchOptionModel searchOptions);
    }
}