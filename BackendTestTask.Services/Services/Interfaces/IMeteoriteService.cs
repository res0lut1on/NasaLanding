using BackendTestTask.Services.Models.Meteorite.NewFolder;
using BackendTestTask.Services.Services.Generic.Interfaces;

namespace BackendTestTask.Services.Services.Interfaces
{
    public interface IMeteoriteService : IMappingsService
    {
        Task<MeteoriteMetadata> GetMeteoriteMetadata();
    }
}
