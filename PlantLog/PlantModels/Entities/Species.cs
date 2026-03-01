using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PlantLog.PlantModels.Entities
{
    public class Species
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string LatinName { get; set; }
        public string Description { get; set; }
        public string CareInstructions { get; set; }

        // NoSQL extra: bármilyen plusz tulajdonság (pl. "Fényigény")
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
