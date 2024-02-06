namespace FlightReservation.MVC.Models;

public sealed class PlaneRoute
{
    public Guid PlaneId { get; set; } = Guid.NewGuid();
    public Guid RouteId { get; set; } = Guid.NewGuid();
}