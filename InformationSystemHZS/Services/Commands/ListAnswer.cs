using InformationSystemHZS.Collections;
using InformationSystemHZS.Models;
using InformationSystemHZS.Exceptions;
using InformationSystemHZS.Services.Commands.Interfaces;

namespace InformationSystemHZS.Services.Commands;

public class ListAnswer(CallsignEntityMap<Station> data, string command) : ICommandAnswer
{
    private readonly CallsignEntityMap<Station> data = data;
    private readonly string command = command;

    public IEnumerable<string> ProcessAnswer()
    {
        IEnumerable<string> answer = command switch
        {
            "stations" => ListStations(),
            "units" => ListUnits(),
            "incidents" => ListIncidents(),
            _ => throw new InvalidCommandException(),
        };

        foreach (var message in answer)
        {
            yield return $">>> {message}";
        }

    }

    private IEnumerable<string> ListStations()
    {
        if (data.GetEntitiesCount() > 0)
        {
            int space = data.GetAllEntities().Select(station => station.Name).Max(station => station.Length);

            foreach (var station in data.GetAllEntities())
            {
                yield return string.Format($"{station.Callsign} | {{0, {-space}}} | {station.Units.GetEntitiesCount()} |" +
                                           $" ({{1:D2}}, {{2:D2}})", station.Name, station.Position.X, station.Position.Y);
            }

        }
    }

    private IEnumerable<string> ListUnits()
    {
        var units = data.GetAllEntities().SelectMany(station => station.Units.GetAllEntities());

        if (units.Any())
        {
            var vehicleNameSpace = units.Max(unit => unit.Vehicle.Name.Length);
            var stateSpace = units.Max(unit => unit.State.Length);

            foreach (var unit in units)
            {
                yield return string.Format($"{unit.StationCallsign} | {unit.Callsign} | {{0, {-vehicleNameSpace}}} |" +
                                           $" {unit.Members.GetEntitiesCount()}/{unit.Vehicle.Capacity} |" +
                                           $" {{1, {-stateSpace}}}" + ((unit.IncidentTime == 0) ? "" : $" | {{2:D2}}:{{3:D2}}"),
                                           unit.Vehicle.Name, unit.State, unit.IncidentTime / 60, unit.IncidentTime % 60);
            }

        }
    }

    private IEnumerable<string> ListIncidents()
    {
        var incidents = data.GetAllEntities()
            .SelectMany(station => station.Units.GetAllEntities().Select(unit => unit.ActiveIncident)
                                   .Where(incident => incident != null).OrderBy(incident => incident.IncidentStartTIme));

        if (incidents.Any())
        {
            var typeSpace = incidents.Max(incident => incident.Type.Length);
            var decsriptionSpace = incidents.Max(incident => incident.Description.Length);

            foreach (var incident in incidents)
            {
                yield return string.Format($"{{0, {-typeSpace}}} | ({{1:D2}}, {{2:D2}}) | {incident.IncidentStartTIme} |" +
                                           $" {{3, {-decsriptionSpace}}} | {incident.AssignedStation} | {incident.AssignedUnit}",
                                           incident.Type, incident.Location.X, incident.Location.Y, incident.Description);
            }

        }
    }
}
