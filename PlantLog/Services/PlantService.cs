using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using PlantLog.PlantModels.Dtos;
using PlantLog.PlantModels.Entities;

namespace PlantLog.Services
{
    public class PlantService
    {
        private readonly IMongoCollection<Plant> _plantsCollection;
        private readonly IGridFSBucket _gridFS;

        public PlantService(IMongoDatabase database)
        {
            _plantsCollection = database.GetCollection<Plant>("Plants");
            _gridFS = new GridFSBucket(database);
        }

        public async Task CreatePlantAsync(PlantCreateDto dto, Stream imageStream, string fileName, string userId)
        {
            // 1. Kép mentése a MongoDB GridFS-be
            var imageId = await _gridFS.UploadFromStreamAsync(fileName, imageStream);

            // 2. Új Plant entitás létrehozása (a hibrid kapcsolattal)
            var newPlant = new Plant
            {
                Nickname = dto.Name,
                SpeciesId = dto.Species, // A 150-es listából választott ID
                OwnerId = userId,        // <--- EZ AZ SQL IDENTITY USER ID-JA!
                ImageId = imageId.ToString(),
                Description = dto.Description,
                Location = dto.Location,
                LastWatered = dto.LastWatered,
                WateringFrequency = dto.WateringFrequency,
                History = new List<LogEntry> { new LogEntry { Action = "Létrehozva", Note = "Kezdeti regisztráció" } }
            };

            // 3. Mentés a MongoDB "Plants" kollekciójába
            await _plantsCollection.InsertOneAsync(newPlant);
        }

        // Lekérdezés a 150+ adat bemutatásához
        public async Task<List<PlantReadDto>> GetMyPlantsAsync(string userId)
        {
            var plants = await _plantsCollection.Find(p => p.OwnerId == userId).ToListAsync();

            return plants.Select(p => new PlantReadDto
            {
                Id = p.Id, // Itt a Mongo string ID-ja megy át
                Name = p.Nickname,
                Species = p.SpeciesId,
                LastWatered = p.LastWatered,
                IsThirsty = (DateTime.Now - p.LastWatered).TotalDays > p.WateringFrequency
                // ... többi mező map-elése
            }).ToList();
        }

        public async Task<string> GetPlantImageBase64Async(string imageId)
        {
            try
            {
                var id = new MongoDB.Bson.ObjectId(imageId);
                var bytes = await _gridFS.DownloadAsBytesAsync(id);
                return $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
            }
            catch
            {
                return "/images/no-image.png"; // Ha nincs kép, egy alapértelmezett
            }
        }

        // Csak az adott felhasználó növényeinek lekérése (HIBRID KAPCSOLAT)
        public async Task<List<Plant>> GetUserPlantsAsync(string userId)
        {
            return await _plantsCollection.Find(p => p.OwnerId == userId).ToListAsync();
        }
        public async Task AddLogEntryAsync(string plantId, string action, string note)
        {
            var filter = Builders<Plant>.Filter.Eq(p => p.Id, plantId);
            var update = Builders<Plant>.Update
                .Push(p => p.History, new LogEntry { Action = action, Note = note, Date = DateTime.Now })
                .Set(p => p.LastWatered, DateTime.Now); // Frissítjük az utolsó öntözés idejét is

            await _plantsCollection.UpdateOneAsync(filter, update);
        }

        public async Task<Plant> GetPlantByIdAsync(string id)
        {
            return await _plantsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task DeletePlantAsync(string plantId, string imageId)
        {
            // 1. Törlés a Plants kollekcióból
            await _plantsCollection.DeleteOneAsync(p => p.Id == plantId);

            // 2. Törlés a GridFS-ből (hogy ne foglalja a helyet a kép)
            if (!string.IsNullOrEmpty(imageId))
            {
                try
                {
                    await _gridFS.DeleteAsync(new MongoDB.Bson.ObjectId(imageId));
                }
                catch (MongoDB.Driver.GridFS.GridFSFileNotFoundException)
                {
                    // Ha a fájl már nem létezett, nem dobunk hibát
                }
            }
        }
        public async Task<(int Total, int Thirsty)> GetPlantStatsAsync(string userId)
        {
            var total = (int)await _plantsCollection.CountDocumentsAsync(p => p.OwnerId == userId);

            // "Szomjas" növények: az utolsó öntözés régebbi, mint a gyakoriság napjai
            var allPlants = await _plantsCollection.Find(p => p.OwnerId == userId).ToListAsync();
            var thirsty = allPlants.Count(p => (DateTime.Now - p.LastWatered).TotalDays > p.WateringFrequency);

            return (total, thirsty);
        }
        public async Task AddLogWithImageAsync(string plantId, string action, string note, Stream? imageStream, string? fileName)
        {
            string? newImageId = null;

            // 1. Ha van kép, feltöltjük a GridFS-be
            if (imageStream != null && !string.IsNullOrEmpty(fileName))
            {
                var id = await _gridFS.UploadFromStreamAsync(fileName, imageStream);
                newImageId = id.ToString();
            }

            // 2. Létrehozzuk a naplóbejegyzést
            var newEntry = new LogEntry
            {
                Action = action,
                Note = note,
                Date = DateTime.Now,
                ImageId = newImageId
            };

            // 3. Hozzáadjuk a növény History listájához
            var filter = Builders<Plant>.Filter.Eq(p => p.Id, plantId);
            var update = Builders<Plant>.Update.Push(p => p.History, newEntry);

            // Ha ez egy öntözés, frissítsük az utolsó öntözés dátumát is
            if (action.Contains("Öntözés"))
            {
                update = update.Set(p => p.LastWatered, DateTime.Now);
            }

            await _plantsCollection.UpdateOneAsync(filter, update);
        }
    }
}
