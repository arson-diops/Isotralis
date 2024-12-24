namespace Isotralis.Infrastructure.Repositories;

public sealed class RepositoryException : Exception
{
    public RepositoryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
