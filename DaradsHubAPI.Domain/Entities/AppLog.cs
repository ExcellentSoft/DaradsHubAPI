namespace DaradsHubAPI.Domain.Entities;

public class ApplicationLogs
{
}
public class AppException(string message, bool isValidationException = true, bool isUnauthorizedException = false, IEnumerable<string>? errorItems = null) : Exception(message)
{
    public bool IsValidationException { get; } = isValidationException;
    public bool IsUnauthorizedException { get; } = isUnauthorizedException;
    public IEnumerable<string>? ErrorItems { get; } = errorItems;
}
public class ErrorVM
{
    public string Message { get; set; } = default!;
    public string Detail { get; set; } = default!;
    public IEnumerable<string>? ErrorItems { get; set; }
}
