namespace _10xPV.Models;

public class WeatherReading
{
    public Guid Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public double? CloudCover { get; set; }
}