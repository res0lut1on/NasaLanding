using BackendTestTask.Database.Entities;
using BackendTestTask.Services.Models.Meteorite;
using BackendTestTask.Services.Models.Meteorite.NewFolder;
using BackendTestTask.Services.Services.SearchInterfaces;

namespace BackendTestTask.Services.Services.SearchImplementations
{
    public class MeteoriteSearchService : IMeteoriteSearchService
    {

        public IQueryable<Meteorite> SearchLogic(IQueryable<Meteorite> query, MeteoriteSearchModel search)
        {
            if (!string.IsNullOrWhiteSpace(search.Search))
            {
                var term = search.Search.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(search.MeteoriteClass))
            {
                query = query.Where(x => x.Class == search.MeteoriteClass);
            }

            if (search.YearFrom.HasValue)
            {
                query = query.Where(x => x.Year >= search.YearFrom.Value);
            }

            if (search.YearTo.HasValue)
            {
                query = query.Where(x => x.Year <= search.YearTo.Value);
            }

            var descending = search.SortDescending ?? false;

            if (!string.IsNullOrWhiteSpace(search.SortBy))
            {
                switch (search.SortBy.Trim().ToLower())
                {
                    case "name":
                        query = descending
                            ? query.OrderByDescending(x => x.Name)
                            : query.OrderBy(x => x.Name);
                        break;
                    case "year":
                        query = descending
                            ? query.OrderByDescending(x => x.Year)
                            : query.OrderBy(x => x.Year);
                        break;
                    case "mass":
                        query = descending
                            ? query.OrderByDescending(x => x.Mass)
                            : query.OrderBy(x => x.Mass);
                        break;
                    default:
                        query = query.OrderBy(x => x.Name); // default fallback
                        break;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Name);
            }

            return query;
        }
    }
}
