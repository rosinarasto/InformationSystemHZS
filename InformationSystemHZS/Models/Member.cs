using InformationSystemHZS.Models.Interfaces;

namespace InformationSystemHZS.Models;

public class Member(string callsign, string unitCallsign, string name) : IBaseModel
{
    public string Callsign { get; set; } = callsign;
    public string UnitCallsign { get; set; } = unitCallsign;
    public string Name { get; set; } = name;
}
