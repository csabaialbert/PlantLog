namespace PlantLog.PlantModels.Entities
{
    public class LogEntry
    {
        public string Action { get; set; } = "";
        public string Note { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Now;
        public string? ImageId { get; set; } // Opcionális kép a bejegyzéshez
    }
}
