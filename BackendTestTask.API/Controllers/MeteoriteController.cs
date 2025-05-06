using BackendTestTask.Database.Entities;
using BackendTestTask.Services.Models.BaseModels;
using BackendTestTask.Services.Services.Generic.Interfaces;
using BackendTestTask.Services.Services.Interfaces;
using BackendTestTask.Services.Services.SearchInterfaces;
using Microsoft.AspNetCore.Mvc;
using BackendTestTask.API.Models;
using BackendTestTask.Services.Models.JournalEvent;
using BackendTestTask.Services.Models;
using BackendTestTask.Services.Models.Meteorite;
using BackendTestTask.Services.Models.Meteorite.NewFolder;

namespace BackendTestTask.Controllers
{
    [ApiController]
    [Route("api/meteorite")]
    public class MeteoriteController : ControllerBase
    {
        private readonly IMeteoriteSearchService _searchService;
        private readonly ICustomGenericService _genericService;
        private readonly IMeteoriteService _meteoriteService;

        public MeteoriteController(
            ICustomGenericService genericService,
            IMeteoriteSearchService searchService,
            IMeteoriteService meteoriteService)
        {
            _genericService = genericService;
            _searchService = searchService;
            _meteoriteService = meteoriteService;
        }

        /// <summary>
        /// Get list of meteorites with pagination and optional search filter
        /// </summary>
        /// <param name="searchOptions">Skip, Take, Search</param>
        /// <returns>Paginated list of meteorites</returns>
        [HttpGet("list")]
        public async Task<ResponseListModelWithTotalCount<ResponseMeteoriteListModel>> GetList([FromQuery] MeteoriteSearchModel searchOptions)
        {
            var result = await _genericService.SearchWithTotalCount<Meteorite, ResponseMeteoriteListModel, MeteoriteSearchModel, string?>(
                _searchService, searchOptions);
             
            return result;
        }

        /// <summary>
        /// Get a grouped list of meteorites by year
        /// </summary>
        /// <param name="searchOptions">Search and filter options</param>
        /// <returns>Grouped list of meteorites by year with totals</returns>
        [HttpGet("grouped")]
        public async Task<ResponseListModelWithTotalCount<MeteoriteYearGroupModel>> GetGroupedList([FromQuery] MeteoriteSearchModel searchOptions)
        {
            var result = await _genericService.SearchWithTotalCount<Meteorite, MeteoriteYearGroupModel, MeteoriteSearchModel, string?>(
                _searchService, searchOptions);

            return result;
        }

        [HttpGet("metadata")]
        public async Task<MeteoriteMetadata> GetMetedata()
        {
            var result = await _meteoriteService.GetMeteoriteMetadata();

            return result;
        }
    }
}
