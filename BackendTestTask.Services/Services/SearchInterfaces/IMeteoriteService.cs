using BackendTestTask.Database.Entities;
using BackendTestTask.Services.Models.BaseModels;
using BackendTestTask.Services.Models.Meteorite;
using BackendTestTask.Services.Models.Meteorite.NewFolder;
using BackendTestTask.Services.Services.Generic.Interfaces;

namespace BackendTestTask.Services.Services.SearchInterfaces
{
    public interface IMeteoriteSearchService : ISearchService<Meteorite, MeteoriteSearchModel> {}
    
}
