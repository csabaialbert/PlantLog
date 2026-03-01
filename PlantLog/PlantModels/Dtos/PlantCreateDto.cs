using System.Runtime.CompilerServices;

namespace PlantLog.PlantModels.Dtos
{
    public class PlantCreateDto
    {
        public string Name { get; set; }
        public string Species { get; set; }
        public string Description { get; set; }
        public DateTime LastWatered { get; set; }
        public int WateringFrequency { get; set; }
        public string Location { get; set; }
    }
}
