using InformationSystemHZS.Collections;
using InformationSystemHZS.Exceptions;
using InformationSystemHZS.IO;
using InformationSystemHZS.IO.Helpers.Interfaces;
using InformationSystemHZS.Models.HelperModels;
using InformationSystemHZS.Services;
using InformationSystemHZS.Models;
using Timer = System.Timers.Timer;

namespace InformationSystemHZS;

public class Runner
{
    private static Timer? _updateTimer;
    private static CallsignEntityMap<Station> _data;
    private static bool isActive = true;

    public static Task Main(IConsoleManager consoleManager, string entryFileName = "Brnoslava.json")
    {
        // ---- DO NOT TOUCH ----
        var commandParser = new CommandParser(consoleManager);
        var outputWriter = new OutputWriter(consoleManager);
        StartUpdateFunction();
        // ^^^^^ DO NOT TOUCH ^^^^^

        ScenarioObjectDto? data;
        commandParser.LogCommand += Logger.OnInputGiven;

        try
        {
            // Load initial data from JSON
            data = ScenarioLoader.GetInitialScenarioData(entryFileName);
            DataValidator.CheckData(data);
        }
        catch (Exception e)
        {
            outputWriter.PrintMessage($"[import]: {e.Message}");
            return Task.CompletedTask;
        }

        _data = DataLoader.GetValidatedData(data);
        var command = new Command(_data);

        while (isActive) 
        {
            try
            {
                var commandAnswer = command.ResolveCommand(commandParser.ReadInput(_data));
                outputWriter.PrintCommandAnswer(commandAnswer);
            }
            catch (HZSSystemException e)
            {
                outputWriter.PrintMessage(e.Message);
                continue;
            }
            catch (Exception)
            {
                throw;
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Starts the update function to update function on program start.
    /// DO NOT CHANGE THIS METHOD.
    /// </summary>
    private static void StartUpdateFunction()
    {
        // Set up a timer to call the Update function every second
        _updateTimer = new Timer(TimeSpan.FromSeconds(1));
        _updateTimer.Elapsed += (sender, e) => UpdateFunction();
        _updateTimer.Start();
    }

    /// <summary>
    /// Is called every second to update the state of the system and its objects.
    /// </summary>
    private static void UpdateFunction()
    {
        var busyUnits = _data.GetAllEntities()
                             .SelectMany(station => station.Units.GetAllEntities().Where(unit => !unit.State.Equals("AVAILABLE")));

        foreach (var unit in busyUnits)
        {
            unit.IncidentTime++;
            unit.AdjustState(_data);
        }
    
    }
}
