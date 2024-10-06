using InformationSystemHZS.Collections;
using InformationSystemHZS.Models;
using InformationSystemHZS.Exceptions;
using InformationSystemHZS.Services.Commands.Interfaces;
using InformationSystemHZS.Services.Commands;

namespace InformationSystemHZS.Services;

public class Command(CallsignEntityMap<Station> data)
{
	private readonly CallsignEntityMap<Station> data = data;

    public ICommandAnswer ResolveCommand(string[] args)
	{
        switch (args[0])
        {
            case "list": return new ListAnswer(data, args[1]);
            case "add":
                AddMember(args);
                return new AddAnswer(args);
            case "remove":
                var name = RemoveMember(args);
                return new RemoveAnswer(args, name);
            case "reassign":
                var callsign = ReassingItem(args);
                return new ReassingAnswer(args, callsign);
            case "statistics":
                return new StatisticsAnswer(data);
            case "report":
                (string stationCallsign, string unitCallsign) = ReportCallsigns(args);
                AssignIncidentToUnit(stationCallsign, unitCallsign, args);
                return new ReportAnswer(stationCallsign, unitCallsign);
            default:
                throw new InvalidCommandException();
        }
    }

    private void AssignIncidentToUnit(string stationCallsign, string unitCallsign, string[] args)
    {
        var unit = data.GetEntity(stationCallsign).Units.GetEntity(unitCallsign);
        unit.ActiveIncident = new Incident("", args[3], new Position(int.Parse(args[1]), int.Parse(args[2])), args[4],
                                           string.Format("{0:dd.MM.yyyy hh:mm:ss}", DateTime.Now), stationCallsign, unitCallsign);
        unit.State = "EN_ROUTE";
    }

    private (string stationCallsign, string unitCallsign) ReportCallsigns(string[] args)
    {
        var suitableUnits = data.GetAllEntities().SelectMany(station => station.Units.GetAllEntities())
                                .Where(unit => Vehicle.TypeUsage[unit.Vehicle.Type].Contains(args[3]) && unit.State.Equals("AVAILABLE"));

        if (!suitableUnits.Any())
        {
            throw new CapacityException("All units are currently busy. Try again later.");
        }

        suitableUnits = suitableUnits.OrderBy(unit => data.GetEntity(unit.StationCallsign)
                                                          .DistanceFromStation(int.Parse(args[1]), int.Parse(args[2])) * unit.Vehicle.Speed);
        var min = data.GetEntity(suitableUnits.ElementAt(0).StationCallsign);
        suitableUnits = suitableUnits.TakeWhile(unit => data.GetEntity(unit.StationCallsign) == min);

        if (suitableUnits.Count() == 1)
        {
            return (suitableUnits.ElementAt(0).StationCallsign, suitableUnits.ElementAt(0).Callsign);
        }

        suitableUnits = suitableUnits.OrderBy(unit => unit.StationCallsign);
        var stationCallsign = suitableUnits.ElementAt(0).StationCallsign;

        if (suitableUnits.Where(unit => unit.StationCallsign.Equals(stationCallsign)).Count() == 1)
        {
            return (suitableUnits.ElementAt(0).StationCallsign, suitableUnits.ElementAt(0).Callsign);
        }

        suitableUnits = suitableUnits.TakeWhile(unit => unit.StationCallsign.Equals(stationCallsign)).OrderBy(unit => unit.Callsign);

        return (suitableUnits.ElementAt(0).StationCallsign, suitableUnits.ElementAt(0).Callsign);
    }

    private string ReassingItem(string[] args)
    {
        var oldStation = data.GetEntity(args[2]);
        
        switch (args[1])
        {
            case "member":
                var member = oldStation?.Units.GetEntity(args[3])?.Members.GetEntity(args[4]);
                oldStation?.Units.GetEntity(args[3])?.Members.SafelyRemoveEntity(args[4]);

                if (data.GetEntity(args[5])?.Units.GetEntity(args[6])?.Members.GetEntitiesCount() ==
                    data.GetEntity(args[5])?.Units.GetEntity(args[6]) ?.Vehicle.Capacity)
                {
                    throw new CapacityException("Vehicle capacity is full.");
                }

                data.GetEntity(args[5])?.Units.GetEntity(args[6])?.Members.SafelyAddEntity(member);
                return member.Callsign;

            case "unit":
                var unit = oldStation.Units.GetEntity(args[3]);
                oldStation?.Units.SafelyRemoveEntity(args[3]);
                data.GetEntity(args[4])?.Units.SafelyAddEntity(unit);
                return unit.Callsign;
            
            default:
                throw new InvalidCommandException();
        }
    }

    private string RemoveMember(string[] args)
    {
        Unit? unit = (data.GetEntity(args[2])?.Units.GetEntity(args[3])) ?? throw new NullReferenceException();

        if (unit.Members.GetEntitiesCount() == 1)
        {
            throw new CapacityException("Unit would have 0 members.");
        }

        var name = unit.Members?.GetEntity(args[4])?.Name ?? "";
        unit?.Members?.SafelyRemoveEntity(args[4]);

        return name;
    }

    private void AddMember(string[] args)
    {
        Unit? unit = (data.GetEntity(args[2])?.Units.GetEntity(args[3])) ?? throw new NullReferenceException();
        
        if (unit.Members.GetEntitiesCount() == unit.Vehicle.Capacity)
        {
            throw new CapacityException("Vehicle capacity is full.");
        }

        unit.Members.SafelyAddEntity(new Member("", unit.Callsign, args[4]));
    }
}
