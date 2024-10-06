using InformationSystemHZS.Exceptions;
using InformationSystemHZS.IO.Helpers.Interfaces;
using InformationSystemHZS.Collections;
using InformationSystemHZS.Models;

namespace InformationSystemHZS.IO;

public class CommandParser(IConsoleManager consoleManager)
{

    private readonly IConsoleManager _consoleManager = consoleManager;

    public string[] ReadInput(CallsignEntityMap<Station> data)
    {
        var input = _consoleManager.ReadLine() ?? throw new EmptyInputException();
        LogCommand?.Invoke(this, input);
        var start = input.IndexOf('"');
        var end = input.LastIndexOf('"');
        string[] inputArray;

        if (start != -1) 
        {
            string text = input[(start + 1)..end];
            input = input[..start];
            input = input.Trim();
            inputArray = new string[input.Split(" ").Length + 1];
            input.Split(" ").CopyTo(inputArray, 0);
            inputArray[^1] = text;
        } else
        {
            input = input.Trim();
            inputArray = input.Split(" ");
        }

        CheckCommandForm(inputArray, data);

        var result = new string[inputArray[0].Split("-").Length + inputArray.Length - 1];
        inputArray[0].Split("-").CopyTo(result, 0);
        
        if (inputArray.Length > 1)
        {
            inputArray[1..].CopyTo(result, inputArray[0].Split("-").Length);
        }

        return result;
    }

    private static void CheckCommandForm(string[] input, CallsignEntityMap<Station> data)
    {
        switch (input[0])
        {
            case "list-incidents": case "list-units": case "list-stations": case "statistics":
                CommandArgsLength(input, 1);
                break;
            case "add-member":
                CommandArgsLength(input, 4);
                UnitExistance(input[1], input[2], data);
                break;
            case "remove-member":
                CommandArgsLength(input, 4);
                MemberExistence(input[1], input[2], input[3], data);
                break;
            case "reassign-member":
                CommandArgsLength(input, 6);
                MemberExistence(input[1], input[2], input[3], data);
                UnitExistance(input[4], input[5], data);
                break;
            case "reassign-unit":
                CommandArgsLength(input, 4);
                UnitExistance(input[1], input[2], data);
                StationExistance(input[3], data);
                break;
            case "report":
                CommandArgsLength(input, 5);
                CheckReportCommand(input);
                break;
            default:
                throw new InvalidCommandException();
        }
    }

    private static void CheckReportCommand(string[] input)
    {

        if (!int.TryParse(input[1], out int num) || num < 0 || num > 99)
        {
            throw new InvalidArgumentsException("The incident is out of our response bounds.");
        }

        if (!int.TryParse(input[2], out num) || num < 0 || num > 99)
        {
            throw new InvalidArgumentsException("The incident is out of our response bounds.");
        }

        foreach (var type in Incident.SolutionTime.Keys)
        {
            if (input[3].Equals(type))
            {
                return;
            }
        }

        throw new InvalidArgumentsException("Invalid incident type.");
    }

    private static void MemberExistence(string station, string unit, string member, CallsignEntityMap<Station> data)
    {
        StationExistance(station, data);
        UnitExistance(station, unit, data);

        if (data.GetEntity(station)?.Units.GetEntity(unit)?.Members.GetEntity(member) == null)
        {
            throw new InvalidArgumentsException($"This member [{member}] doest not exist.");
        }
    }

    private static void UnitExistance(string station, string unit, CallsignEntityMap<Station> data)
    {
        StationExistance(station, data);

        if (data.GetEntity(station)?.Units.GetEntity(unit) == null)
        {
            throw new InvalidArgumentsException($"This unit [{unit}] does not exist.");
        }
    }

    private static void StationExistance(string station, CallsignEntityMap<Station> data)
    {
        if (data.GetEntity(station) == null)
        {
            throw new InvalidArgumentsException($"This station [{station}] does not exist.");
        }
    }

    private static void CommandArgsLength(string[] input, int expectedLength)
    {
        if (input.Length != expectedLength)
        {
            throw new InvalidArgumentsException("Invalid arguments.");
        }
    }

    public event EventHandler<string>? LogCommand;
}