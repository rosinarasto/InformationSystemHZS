using InformationSystemHZS.Collections;
using InformationSystemHZS.Models.Interfaces;
using InformationSystemHZS.Services;

namespace InformationSystemHZS.Models;

public class Unit(string callsign, string stationCallsign, string state, Vehicle vehicle) : IBaseModel
{
    public string Callsign { get; set; } = callsign;
    public string StationCallsign { get; set; } = stationCallsign;
    public string State { get; set; } = state;
    public Vehicle Vehicle { get; set; } = vehicle;
    public Incident? ActiveIncident { get; set; } = null;
    public CallsignEntityMap<Member> Members { get; set; } = new("H");
    public List<Incident> IncidentHistory { get; set; } = [];
    public int IncidentTime { get; set; } = 0;

    public void AdjustState(CallsignEntityMap<Station> stations)
    {
        var distance = stations.GetEntity(StationCallsign).DistanceFromStation(ActiveIncident.Location.X, ActiveIncident.Location.Y);

        if (IncidentTime == (int) DistanceService.CalculateTimeTaken(distance, Vehicle.Speed))
        {
            State = "ON_SITE";
        }

        if (IncidentTime == (int) DistanceService.CalculateTimeTaken(distance, Vehicle.Speed) + ActiveIncident.GetSolutionTime())
        {
            State = "RETURNING";
        }

        if (IncidentTime == 2 * (int) DistanceService.CalculateTimeTaken(distance, Vehicle.Speed) + ActiveIncident.GetSolutionTime())
        {
            State = "AVAILABLE";
            IncidentTime = 0;
            IncidentHistory.Add(ActiveIncident);
            ActiveIncident = null;
        }
    }
}
