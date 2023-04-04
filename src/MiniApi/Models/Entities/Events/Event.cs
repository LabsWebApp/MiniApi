namespace MiniApi.Models.Entities.Events;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public double Ratio { get; set; }
}