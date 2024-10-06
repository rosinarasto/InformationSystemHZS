namespace InformationSystemHZS.Exceptions;

public class CapacityException(string message) : HZSSystemException($"[capacity]: Not possible. {message}")
{
}
