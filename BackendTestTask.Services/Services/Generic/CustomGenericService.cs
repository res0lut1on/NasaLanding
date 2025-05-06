using BackendTestTask.Database;
using BackendTestTask.Database.Entities;
using BackendTestTask.Services.Models;
using BackendTestTask.Services.Models.BaseModels;
using BackendTestTask.Services.Models.JournalEvent;
using BackendTestTask.Services.Models.Meteorite;
using BackendTestTask.Services.Services.Generic.Interfaces;
using BackendTestTask.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTestTask.Services.Services.Generic
{
    public class CustomGenericService : ICustomGenericService
    {
        private readonly IRepository<BackendTestTaskContext> _repository;
        private readonly IMeteoriteService _meteoriteService;

        public CustomGenericService(IRepository<BackendTestTaskContext> repository, IMeteoriteService meteoriteService)
        {
            _meteoriteService = meteoriteService;
            _repository = repository;
        }

        public async Task<List<TModel>> Search<TEntity, TModel, TSearchModel, T>(
            ISearchService<TEntity, TSearchModel> searchService, TSearchModel searchOptions) where TEntity : class
            where TModel : class
            where TSearchModel : BaseSearchModel<T>
        {
            var query = _repository.Query<TEntity>();

            query = searchService.SearchLogic(query, searchOptions);

            return await GetModels<TModel, TEntity, TSearchModel>(searchOptions, query);
        }

        public async Task<ResponseListModelWithTotalCount<TModel>> SearchWithTotalCount<TEntity, TModel, TSearchModel, T>(
            ISearchService<TEntity, TSearchModel> searchService, TSearchModel searchOptions)
            where TEntity : class
            where TModel : class
            where TSearchModel : BaseSearchModel<T>
        {
            searchOptions.Take ??= 20;

            var query = _repository.Query<TEntity>();

            query = searchService.SearchLogic(query, searchOptions);

            if (typeof(TEntity) == typeof(Meteorite) && typeof(TModel) == typeof(MeteoriteYearGroupModel))
            {
                return await GetGroupedMeteorite<TEntity, TModel, TSearchModel, T>(searchOptions, query);
            }

            var totalCount = await query.CountAsync();

            var models = await GetModels<TModel, TEntity, TSearchModel>(searchOptions, query);

            var result = new ResponseListModelWithTotalCount<TModel>
            {
                Items = models,
                Count = totalCount
            };

            return result;
        }

        private static async Task<ResponseListModelWithTotalCount<TModel>> GetGroupedMeteorite<TEntity, TModel, TSearchModel, T>(
    TSearchModel searchOptions, IQueryable<TEntity> query)
    where TEntity : class
    where TModel : class
    where TSearchModel : BaseSearchModel<T>
        {
            if (searchOptions is not MeteoriteSearchModel meteoriteSearch)
                throw new InvalidOperationException("Expected MeteoriteSearchModel");

            var filteredQuery = query.Cast<Meteorite>();

            // 🔍 Фильтрация
            if (!string.IsNullOrWhiteSpace(meteoriteSearch.Search))
            {
                var term = meteoriteSearch.Search.ToLower();
                filteredQuery = filteredQuery.Where(x => x.Name.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(meteoriteSearch.MeteoriteClass))
            {
                filteredQuery = filteredQuery.Where(x => x.Class == meteoriteSearch.MeteoriteClass);
            }

            if (meteoriteSearch.YearFrom.HasValue)
            {
                filteredQuery = filteredQuery.Where(x => x.Year >= meteoriteSearch.YearFrom.Value);
            }

            if (meteoriteSearch.YearTo.HasValue)
            {
                filteredQuery = filteredQuery.Where(x => x.Year <= meteoriteSearch.YearTo.Value);
            }

            // ⚙️ Группировка
            var grouped = await filteredQuery
                .GroupBy(m => m.Year)
                .Select(g => new MeteoriteYearGroupModel
                {
                    Year = g.Key,
                    TotalCount = g.Count(),
                    TotalMass = g.Sum(x => x.Mass),
                    Meteorites = g
                        .OrderBy(x => x.Name) // вложенная сортировка для читаемости
                        .Select(m => new ResponseMeteorite
                        {
                            Id = m.Id,
                            Name = m.Name,
                            Class = m.Class,
                            Year = m.Year,
                            Mass = m.Mass
                        }).ToList()
                })
                .ToListAsync();

            // 🧠 Сортировка по агрегированным данным — уже после группировки
            var groupedSorted = meteoriteSearch.SortBy?.ToLower() switch
            {
                "year" => (meteoriteSearch.SortDescending ?? false)
                    ? grouped.OrderByDescending(x => x.Year)
                    : grouped.OrderBy(x => x.Year),

                "totalcount" => (meteoriteSearch.SortDescending ?? false)
                    ? grouped.OrderByDescending(x => x.TotalCount)
                    : grouped.OrderBy(x => x.TotalCount),

                "totalmass" => (meteoriteSearch.SortDescending ?? false)
                    ? grouped.OrderByDescending(x => x.TotalMass)
                    : grouped.OrderBy(x => x.TotalMass),

                _ => grouped.OrderBy(x => x.Year) // default
            };

            // 📦 Пагинация
            var skip = searchOptions.Skip ?? 0;
            var take = searchOptions.Take ?? 20;

            var resultItems = groupedSorted
                .Skip(skip)
                .Take(take)
                .ToList();

            return new ResponseListModelWithTotalCount<TModel>
            {
                Items = resultItems.Cast<TModel>().ToList(),
                Count = grouped.Count
            };
        }


        private async Task<List<TModel>> GetModels<TModel, TEntity, TSearchModel>(TSearchModel searchOptions, IQueryable<TEntity> query)
            where TSearchModel : PaginationParams
            where TEntity : class
            where TModel : class
        {
            if (searchOptions.Skip.HasValue)
            {
                query = query.Skip(searchOptions.Skip.Value);
            }
            if (searchOptions.Take.HasValue)
            {
                query = query.Take(searchOptions.Take.Value);
            }

            List<TModel> models;

            if (typeof(TEntity) == typeof(Meteorite))
            {
                models = await _meteoriteService.Map<TEntity, TModel>(query).ToListAsync();
            } else
            { 
                throw new NotImplementedException();
            }

            return models;
        }

        
    }
}
