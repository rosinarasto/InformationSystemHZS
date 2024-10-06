using InformationSystemHZS.Models.HelperModels;
using InformationSystemHZS.Models.Interfaces;
using InformationSystemHZS.Exceptions;

namespace InformationSystemHZS.Services;

public static class DataValidator
{
    private static readonly List<string> ValidVehicleType = [ "FIRE_ENGINE", "TECHNICAL_VEHICLE", "ANTI_GAS_VEHICLE",
                                                              "RESCUE_VEHICLE", "CRANE_TRUCK" ];

    public static void CheckData(ScenarioObjectDto? data)
    {
        EmptyData(data);
        if (data != null)
        {
            MembersCount(data);
            InvalidVehicleType(data);
            CheckDuplicateId(data);
        }

        return;
    }

    private static void EmptyData(ScenarioObjectDto? data)
    {
        if (data == null)
        {
            throw new NullDataException();
        }
    }

    private static void MembersCount(ScenarioObjectDto data)
    {

        foreach (var station in data.Stations)
        {

            foreach (var unit in station.Units)
            {
                if (unit.Members.Count == 0)
                {
                    throw new InvalidMembersCountException($"Unit \"{unit.Callsign}\", from \"{station.Name}\" station," +
                        $" doesnt have any members.");
                }
                if (unit.Members.Count > unit.VehicleDto.Capacity)
                {
                    throw new InvalidMembersCountException($"Count of the unit members, in unit \"{unit.Callsign}\"," +
                        $"from \"{station.Name}\" station, is higher than vehicle capacity.");
                }
            }

        }

    }

    private static void InvalidVehicleType(ScenarioObjectDto data)
    {

        foreach (var station in data.Stations)
        {
            foreach (var unit in station.Units)
            {
                if (!ValidVehicleType.Contains(unit.VehicleDto.Type))
                {
                    throw new InvalidVehicleTypeException($"Vehicle \"{unit.VehicleDto.Name}\", " +
                        $"from unit \"{unit.Callsign}\", station \"{station.Name}\", has invalid type.");
                }
            }

        }

    }

    private static void CheckDuplicateId(ScenarioObjectDto data)
    {
        DuplicateId(data.Stations, "Duplicate ID: in stations list.");

        foreach (var station in data.Stations)
        {
            DuplicateId(station.Units, $"Duplicate ID in units list in station \"{station.Name}\".");

            foreach (var unit in station.Units)
            {
                DuplicateId(unit.Members, $"Duplicate ID in members list in unit \"{unit.Callsign}\"" +
                    $" in station \"{station.Name}\".");
            }

        }
    
    }

    private static void DuplicateId<T>(List<T> list, string message) where T : IBaseModel
    {

        foreach (var item in list)
        {
            if (list.Where(x => x.Callsign.Equals(item.Callsign)).Count() > 1)
            {
                throw new DuplicateIdException($"{message} ID: {item.Callsign}");
            }
        }

    }
}
