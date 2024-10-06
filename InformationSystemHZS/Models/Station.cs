using InformationSystemHZS.Collections;
using InformationSystemHZS.Models.Interfaces;
using InformationSystemHZS.Services;

namespace InformationSystemHZS.Models;

public class Station(Position position, string callsign, string name) : IBaseModel
{
    public string Callsign { get; set; } = callsign;
    public Position Position { get; set; } = position;
    public string Name { get; set; } = name;
    public CallsignEntityMap<Unit> Units { get; set; } = new("J");

    public double DistanceFromStation(int x, int y)
    {
        return DistanceService.CalculateDistance(x, y, Position.X, Position.Y);
    }
}
