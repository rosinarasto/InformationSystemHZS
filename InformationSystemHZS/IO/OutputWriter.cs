using InformationSystemHZS.IO.Helpers.Interfaces;
using InformationSystemHZS.Services.Commands.Interfaces;

namespace InformationSystemHZS.IO;

public class OutputWriter(IConsoleManager consoleManager)
{
    private readonly IConsoleManager _consoleManager = consoleManager;

    public void PrintMessage(string message)
    {
        _consoleManager.WriteLine(message);
    }

    public void PrintCommandAnswer(ICommandAnswer commandAnswer)
    {

        foreach (var answer in commandAnswer.ProcessAnswer())
        {
            _consoleManager.WriteLine(answer);
        }

    }
}