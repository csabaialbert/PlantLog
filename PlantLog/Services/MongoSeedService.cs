using MongoDB.Driver;
using PlantLog.PlantModels.Entities;

namespace PlantLog.Services
{
    public class MongoSeedService
    {
        private readonly IMongoCollection<Species> _speciesCollection;

        public MongoSeedService(IMongoDatabase database)
        {
            _speciesCollection = database.GetCollection<Species>("Species");
        }

        public async Task SeedSpeciesAsync()
        {
            // Csak akkor töltünk, ha még üres
            if (await _speciesCollection.CountDocumentsAsync(_ => true) == 0)
            {
                var speciesList = new List<Species>();
                string[] plantNames = { "Kaktusz", "Páfrány", "Fikusz", "Anyósnyelv", "Orchidea", "Bonsai", "Aloe Vera", "Levendula" };
                string[] lightLevels = { "Alacsony", "Közepes", "Magas", "Közvetlen napfény" };

                for (int i = 1; i <= 150; i++)
                {
                    var type = plantNames[i % plantNames.Length];
                    speciesList.Add(new Species
                    {
                        Name = $"{type} - {i}. variáns",
                        LatinName = $"Plantae {type}us v{i}",
                        Description = $"{i}. sorszámú különleges növényfajta a gyűjteményből.",
                        Metadata = new Dictionary<string, string>
                    {
                        { "Fényigény", lightLevels[i % lightLevels.Length] },
                        { "Veszélyesség", i % 10 == 0 ? "Mérgező" : "Biztonságos" }
                    }
                    });
                }
                await _speciesCollection.InsertManyAsync(speciesList);
            }
        }
    }
}
