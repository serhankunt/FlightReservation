namespace FlightReservation.MVC.Models;

public sealed class Route
{
    public Route()
    {
        Id = Guid.NewGuid();
    }
   public Guid Id { get; set; }
    public string Departure { get; set; } = string.Empty;//Kalkış noktası
    public string Arrival {  get; set; } = string.Empty;//Varış Noktası
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public Guid PlaneId { get; set; }
    public Plane? Plane { get; set; }

}
