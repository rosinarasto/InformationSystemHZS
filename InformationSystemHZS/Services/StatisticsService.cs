using InformationSystemHZS.Models;

namespace InformationSystemHZS.Services;

// IMPORTANT NOTE: For this part of code use only LINQ.
// NOTE: You can change the signature of functions to take a single parameter, i.e. something like List<Station>.

public static class StatisticsService
{
    /// <summary>
    /// Returns the total number of all 'FIRE_ENGINE' across all stations.
    /// </summary>
    public static int GetTotalFireEnginesCount(List<Station> stations)
    {
        var result = stations.SelectMany(station => station.Units.GetAllEntities())
                             .Select(unit => unit.Vehicle.Type).Where(type => type.Equals("FIRE_ENGINE"));
        return result.Count();
    }

    /// <summary>
    /// Returns the name of the station closest to the hospital at coordinates (45, 60).
    /// </summary>
    public static string GetClosestToHospital(List<Station> stations)
    {
        return stations.OrderBy(station => DistanceService.CalculateDistance(station.Position.X ,station.Position.Y, 45, 60))
                       .Select(x => x.Name).First();
    }
    
    /// <summary>
    /// Returns the callsign of the unit with the fastest vehicle. If no decision can be made, an error is printed.
    /// </summary>
    public static string GetFastestVehicleUnit(List<Station> stations)
    {
        var units = stations.SelectMany(station => station.Units.GetAllEntities())
                                    .OrderByDescending(unit => unit.Vehicle.Speed);

        if (units.Count() > 1 && units.ElementAt(0).Vehicle.Speed == units.ElementAt(1).Vehicle.Speed) {
            return "Can't be decided, which vehicle is the fastest.";
        }

        return units.First().Callsign;
    }

    /// <summary>
    /// Returns the callsign of the station that has the most firefighters under it.
    /// </summary>
    public static string GetStationWithMostPersonel(List<Station> stations)
    {
        return stations.OrderByDescending(station => station.Units.GetAllEntities()
                                          .Sum(unit => unit.Members.GetEntitiesCount()))
                       .First().Callsign;
    }

    /// <summary>
    /// Returns a list of all vehicle names sorted by fuel consumption (first with the lowest, last with the highest).
    /// Duplicate names must not appear in the list.
    /// </summary>
    public static List<string> GetVehiclesByFuelConsumption(List<Station> stations)
    {
        var vehicles = stations.SelectMany(station => station.Units.GetAllEntities())
                               .OrderBy(unit => unit.Vehicle.FuelConsumption).GroupBy(unit => unit.Vehicle.Name)
                               .Select(unit => unit.Key);
        return vehicles.ToList();
    }

    /// <summary>
    /// Returns the callsign of the unit that has historically resolved the highest number of events.
    /// </summary>
    public static string GetMostBusyUnit(List<Station> stations)
    {
        var units = stations.SelectMany(station => station.Units.GetAllEntities())
                            .OrderByDescending(unit => unit.IncidentHistory.Count);
        return units.First().Callsign;
    }

    /// <summary>
    /// Returns the callsign of the unit that has consumed the most fuel with its vehicle in the sum of all its historical events.
    /// </summary>
    public static string MostFuelConsumedUnit(List<Station> stations)
    {
        var units = stations.SelectMany(station => station.Units.GetAllEntities()
                                        .OrderBy(unit => unit.IncidentHistory
                                                 .Sum(incident => DistanceService.CalculateFuelConsumed(
                                                      DistanceService.CalculateDistance(incident.Location.X, incident.Location.Y,
                                                                                        station.Position.X, station.Position.Y),
                                                      unit.Vehicle.FuelConsumption))));

        return units.Last().Callsign;
    }
}
