using InformationSystemHZS.Services.Commands.Interfaces;

namespace InformationSystemHZS.Services.Commands;

public class ReportAnswer(string stationCallsign, string unitCallsign) : ICommandAnswer
{
	private string stationCallsign = stationCallsign;
	private string unitCallsign = unitCallsign;

    public IEnumerable<string> ProcessAnswer()
    {
        yield return $"[proceed]:  Unit {unitCallsign} from station {stationCallsign} was assigned to the incident.";
    }
}
