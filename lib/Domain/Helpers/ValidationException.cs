namespace Bonjour.Domain.Helpers;

public class ValidationException : Exception
{
    public string Field { get; private set; }
    public ValidationException(string field, string message) : base(message)
    {
        Field = field;
    }
}