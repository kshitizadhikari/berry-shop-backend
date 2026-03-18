namespace Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entity, object key) : base($"{entity} with id '{key}' was not found.")
    {
    }
}

public class BadRequestException(string message) : Exception(message);

public class UnauthorizedException(string message = "Unauthorized") : Exception(message);

public class ConflictException(string message) : Exception(message);