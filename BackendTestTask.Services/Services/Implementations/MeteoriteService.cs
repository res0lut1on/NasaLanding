using BackendTestTask.Common.CustomExceptions;
using BackendTestTask.Database;
using BackendTestTask.Database.Entities;
using BackendTestTask.Services.Models.JournalEvent;
using BackendTestTask.Services.Models.Meteorite.NewFolder;
using BackendTestTask.Services.Services.Generic.Interfaces;
using BackendTestTask.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BackendTestTask.Services.Services.Implementations
{
    public class MeteoriteService : IMeteoriteService, IMappingsService
    {
        private readonly IRepository<BackendTestTaskContext> _repository;
        private readonly IMemoryCache _cache;

        public MeteoriteService(IRepository<BackendTestTaskContext> repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<MeteoriteMetadata> GetMeteoriteMetadata()
        {
            const string cacheKey = "meteorite_metadata";

            if (_cache.TryGetValue(cacheKey, out MeteoriteMetadata metadata))
                return metadata;

            var years = await _repository.Query<Meteorite>()
                .AsNoTracking()
                .Select(m => m.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToListAsync();

            var classes = await _repository.Query<Meteorite>()
                .AsNoTracking()
                .Select(m => m.Class)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            metadata = new MeteoriteMetadata
            {
                Years = years,
                Recclasses = classes
            };

            _cache.Set(cacheKey, metadata, TimeSpan.FromMinutes(30));

            return metadata;
        }

        public IQueryable<TModel> Map<TEntity, TModel>(IQueryable<TEntity> query) where TModel : class
        {
            if (typeof(TModel) == typeof(ResponseMeteoriteListModel))
            {
                var result = MapToResponseListModel((IQueryable<Meteorite>)query);
                return result.Cast<TModel>();
            }

            throw new NotImplementedException($"Mapping for {typeof(TModel).Name} is not implemented.");
        }

        private IQueryable<ResponseMeteoriteListModel> MapToResponseListModel(IQueryable<Meteorite> query)
        {
            return query.Select(q => new ResponseMeteoriteListModel
            {
                Id = q.Id,
                Name = q.Name,
                Year = q.Year,
                Class = q.Class,
                Mass = q.Mass
            });
        }
    }
}
