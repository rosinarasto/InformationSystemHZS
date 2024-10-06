namespace InformationSystemHZS.Exceptions;

public class InvalidArgumentsException(string message) : HZSSystemException($"[invalid]: {message}")
{
}
