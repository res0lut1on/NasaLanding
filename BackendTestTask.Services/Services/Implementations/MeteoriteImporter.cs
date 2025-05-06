using BackendTestTask.Database;
using BackendTestTask.Database.Entities;
using BackendTestTask.Services.Models.Meteorite;
using BackendTestTask.Services.Models.Settings;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BackendTestTask.Services.Services.Implementations;

public class MeteoriteImporter
{
    private readonly HttpClient _httpClient;
    private readonly BackendTestTaskContext _db;
    private readonly ILogger<MeteoriteImporter> _logger;
    private readonly string _dataUrl;

    public MeteoriteImporter(HttpClient httpClient, BackendTestTaskContext db, ILogger<MeteoriteImporter> logger, IOptions<MeteoriteDataSettings> settings)
    {
        _httpClient = httpClient;
        _db = db;
        _logger = logger;
        _dataUrl = settings.Value.SourceUrl;
    }

    public async Task SyncMeteoritesAsync()
    {
        try
        {
            string json = await _httpClient.GetStringAsync(_dataUrl);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiData = JsonSerializer.Deserialize<List<MeteoriteApiModel>>(json, options) ?? new();

            var remoteEntities = apiData
                .Where(x => int.TryParse(x.Id, out _))
                .Select(MapToEntity)
                .ToList();

            var remoteIds = remoteEntities.Select(x => x.Id).ToHashSet();
            var dbMeteorites = await _db.Meteorites.AsNoTracking().ToListAsync();
            var dbIds = dbMeteorites.Select(x => x.Id).ToHashSet();

            var toAdd = remoteEntities.Where(x => !dbIds.Contains(x.Id)).ToList();
            var toRemove = dbMeteorites.Where(x => !remoteIds.Contains(x.Id)).ToList();
            var toUpdate = remoteEntities.Where(x => dbIds.Contains(x.Id)).ToList();


            if (toRemove.Any())
                await _db.BulkDeleteAsync(toRemove);

            if (toAdd.Any())
                await _db.BulkInsertAsync(toAdd);

            if (toUpdate.Any())
                await _db.BulkUpdateAsync(toUpdate); 

            _logger.LogInformation($"✅ Sync complete: Added {toAdd.Count}, Updated {toUpdate.Count}, Removed {toRemove.Count} meteorites.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error syncing meteorite data");
            throw;
        }
    }


    private static Meteorite MapToEntity(MeteoriteApiModel dto)
    {
        _ = int.TryParse(dto.Id, out int id);
        _ = double.TryParse(dto.Mass, out double mass);
        _ = DateTime.TryParse(dto.Year, out DateTime yearDate);
        _ = double.TryParse(dto.Reclat, out double reclat);
        _ = double.TryParse(dto.Reclong, out double reclong);

        double? geoLon = null;
        double? geoLat = null;
        if (dto.GeoLocation?.Coordinates?.Count == 2)
        {
            geoLon = dto.GeoLocation.Coordinates[0];
            geoLat = dto.GeoLocation.Coordinates[1];
        }

        return new Meteorite
        {
            Id = id,
            Name = dto.Name,
            Class = dto.Class,
            Mass = mass,
            Year = yearDate.Year,
            Fall = dto.Fall,
            Reclat = reclat,
            Reclong = reclong,
            GeoType = dto.GeoLocation?.Type,
            GeoLongitude = geoLon,
            GeoLatitude = geoLat
        };
    }
}
