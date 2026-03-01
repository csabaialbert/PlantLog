namespace PlantLog.PlantModels.Dtos
{
    public class PlantReadDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public string Description { get; set; }
        public DateTime LastWatered { get; set; }
        public int WateringFrequency { get; set; }
        public string Location { get; set; }
        public DateTime NextWateringDate { get; set; }
        public bool IsThirsty { get; set; }

    }
}
