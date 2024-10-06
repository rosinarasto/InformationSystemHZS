using InformationSystemHZS.Collections;
using InformationSystemHZS.Models;
using InformationSystemHZS.Models.HelperModels;

namespace InformationSystemHZS.Services;

public static class DataLoader
{
    public static CallsignEntityMap<Station> GetValidatedData(ScenarioObjectDto data)
    {
        var validData = new CallsignEntityMap<Station>("S");

        foreach (var station in data.Stations)
        {
            _ = validData.SafelyAddEntity(new Station(new Position(station.PositionDto.X, station.PositionDto.Y),
                                                      station.Callsign, station.Name), station.Callsign);

            foreach (var unit in station.Units)
            {
                var stationMap = validData.GetEntity(station.Callsign);
                _ = stationMap?.Units.SafelyAddEntity(new Unit(unit.Callsign, station.Callsign, "AVAILABLE",
                                                               new Vehicle(unit.VehicleDto.Name, unit.VehicleDto.Type, 
                                                                           unit.VehicleDto.FuelConsumption, unit.VehicleDto.Speed,
                                                                           unit.VehicleDto.Capacity)), unit.Callsign);
                
                foreach (var member in unit.Members)
                {
                    var unitMap = stationMap?.Units.GetEntity(unit.Callsign);
                    _ = unitMap?.Members.SafelyAddEntity(new Member(member.Callsign, unit.Callsign, member.Name), member.Callsign);
                }
            
            }

        }

        foreach (var incident in data.IncidentsHistory)
        {
            validData.GetEntity(incident.AssignedStation).Units.GetEntity(incident.AssignedUnit)
                     .IncidentHistory.Add(new Incident("", incident.Type, new Position(incident.Location.X, incident.Location.Y),
                                                                  incident.Description, incident.IncidentStartTIme,
                                                                  incident.AssignedStation, incident.AssignedUnit));
        }

        return validData;
    }
}
