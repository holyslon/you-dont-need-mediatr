using System.Runtime.Serialization;

namespace Example.App.Validation;

public class ValidationException : Exception
{
    public string Path { get; }
    public string ValidationMessage { get; }

    public ValidationException(string path, string validationMessage) :base($"Fail to validate {path}. Reason: {validationMessage}")
    {
        Path = path;
        ValidationMessage = validationMessage;
    }

    public ValidationException(string path, string validationMessage, Exception? innerException) : base(validationMessage, innerException)
    {
        Path = path;
        ValidationMessage = validationMessage;
    }
}