using InformationSystemHZS.Services.Commands.Interfaces;
using InformationSystemHZS.Collections;
using InformationSystemHZS.Models;
using InformationSystemHZS.Services;
using System.Text;

namespace InformationSystemHZS.Services.Commands;

public class StatisticsAnswer(CallsignEntityMap<Station> data) : ICommandAnswer
{ 
    private readonly CallsignEntityMap<Station> data = data;

    public IEnumerable<string> ProcessAnswer()
    {
        yield return $"Total number of water tanks: {StatisticsService.GetTotalFireEnginesCount(data.GetAllEntities())}";
        yield return $"The closest station to the hospital: {StatisticsService.GetClosestToHospital(data.GetAllEntities())}";
        yield return $"Fastest unit: {StatisticsService.GetFastestVehicleUnit(data.GetAllEntities())}";
        yield return $"Station with the most firefighters: {StatisticsService.GetStationWithMostPersonel(data.GetAllEntities())}";

        var result = new StringBuilder("Vehicles by fuel consumption:");

        foreach (var vehicle in StatisticsService.GetVehiclesByFuelConsumption(data.GetAllEntities()))
        {
            result.Append(' ').Append(vehicle).Append(',');
        }

        yield return result.Remove(result.Length - 1, 1).ToString();

        yield return $"Busiest unit: {StatisticsService.GetMostBusyUnit(data.GetAllEntities())}";
        yield return $"Most fuel consumed: {StatisticsService.MostFuelConsumedUnit(data.GetAllEntities())}";    
    }
}
