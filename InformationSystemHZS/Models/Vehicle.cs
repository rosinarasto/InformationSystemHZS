namespace InformationSystemHZS.Models;

public class Vehicle(string name, string type, double fuelConsumption, int speed, int capacity)
{
    public string Name { get; set; } = name;
    public string Type { get; set; } = type;
    public double FuelConsumption { get; set; } = fuelConsumption;
    public int Speed { get; set; } = speed;
    public int Capacity { get; set; } = capacity;

    public static Dictionary<string, List<string>> TypeUsage { get; } = new Dictionary<string, List<string>>
    {
        ["FIRE_ENGINE"] = ["FIRE", "ACCIDENT"],
        ["TECHNICAL_VEHICLE"] = [ "DISASTER","TECHNICAL"],
        ["ANTI_GAS_VEHICLE"] = [ "HAZARD" ],
        ["RESCUE_VEHICLE"] = [ "ACCIDENT", "RESCUE" ],
        ["CRANE_TRUCK"] = [ "TECHNICAL", "RESCUE" ]
    };
}
