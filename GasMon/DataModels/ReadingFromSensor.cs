namespace GasMon
{
    public class ReadingFromSensor
    {
        public string LocationId { get; set; }
        public string EventId { get; set; }
        public double Value { get; set; }
        public long Timestamp { get; set; }
    }
}