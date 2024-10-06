using InformationSystemHZS.Services.Commands.Interfaces;

namespace InformationSystemHZS.Services.Commands;

public class RemoveAnswer(string[] args, string name) : ICommandAnswer
{
    private string[] args = args;
    private string name = name;

    public IEnumerable<string> ProcessAnswer()
    {
        yield return $"[processed]: {name} was removed from unit {args[3]}.";
    }
}
