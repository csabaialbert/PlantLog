using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PlantLog.PlantModels.Entities
{
    public class Plant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Nickname { get; set; }
        public string Description { get; set; }

        // Kapcsolat az MSSQL-hez (IdentityUser.Id)
        public string OwnerId { get; set; }

        // Kapcsolat a Species kollekcióhoz
        public string SpeciesId { get; set; }

        // Kapcsolat a GridFS képhez (csak az ID-t tároljuk!)
        public string ImageId { get; set; }

        public string Location { get; set; }
        public DateTime LastWatered { get; set; }
        public int WateringFrequency { get; set; }

        // Beágyazott lista (nem külön tábla!)
        public List<LogEntry> History { get; set; } = new();
    }
}
