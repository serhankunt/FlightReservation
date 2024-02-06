namespace FlightReservation.MVC.Models;

public sealed class Plane
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TailNumber { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public string SeatConfiguration { get; set; } = string.Empty;
}
