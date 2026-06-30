using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebAPIDemo.Models;

namespace WebAPIDemo.Services;

public class SensorDataService
{
    private readonly IOptions<MongoDbSettings> _settings;
    private readonly ILogger<SensorDataService> _logger;
    private IMongoCollection<SensorData>? _collection;
    private readonly object _lockObject = new object();

    public SensorDataService(IOptions<MongoDbSettings> settings, ILogger<SensorDataService> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    private IMongoCollection<SensorData> GetCollection()
    {
        if (_collection != null)
            return _collection;

        lock (_lockObject)
        {
            if (_collection != null)
                return _collection;

            try
            {
                _logger.LogInformation("Connecting to MongoDB at {ConnectionString}", _settings.Value.ConnectionString);
                var client = new MongoClient(_settings.Value.ConnectionString);
                var database = client.GetDatabase(_settings.Value.DatabaseName);
                _collection = database.GetCollection<SensorData>(_settings.Value.CollectionName);
                _logger.LogInformation("Successfully connected to MongoDB");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to MongoDB");
                throw;
            }

            return _collection;
        }
    }

    public async Task<List<SensorData>> GetAsync() =>
        await GetCollection().Find(_ => true).ToListAsync();

    public async Task<SensorData?> GetAsync(string id) =>
        await GetCollection().Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(SensorData data) =>
        await GetCollection().InsertOneAsync(data);

    public async Task UpdateAsync(string id, SensorData data) =>
        await GetCollection().ReplaceOneAsync(x => x.Id == id, data);

    public async Task DeleteAsync(string id) =>
        await GetCollection().DeleteOneAsync(x => x.Id == id);
}