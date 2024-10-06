using InformationSystemHZS.Models.Interfaces;

namespace InformationSystemHZS.Models;

public class Incident(string callsign, string type, Position location, string desc,
                      string incidentStartTIme, string assignedStation, string assignedUnit) : IBaseModel
{
    public string Callsign { get; set; } = callsign;
    public string Type { get; set; } = type;
    public Position Location { get; set; } = location;
    public string Description { get; set; } = desc;
    public string IncidentStartTIme { get; set; } = incidentStartTIme;
    public string AssignedStation { get; set; } = assignedStation;
    public string AssignedUnit { get; set; } = assignedUnit;
    
    public static Dictionary<string, int> SolutionTime { get; } = new Dictionary<string, int>
    {
        ["FIRE"] = 10,
        ["ACCIDENT"] = 6,
        ["DISASTER"] = 8,
        ["HAZARD"] = 10,
        ["TECHNICAL"] = 4,
        ["RESCUE"] = 6
    };

    public int GetSolutionTime()
    {
        return SolutionTime[Type];
    }
}
