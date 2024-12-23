namespace Isotralis.Domain.ValueObjects;

public sealed record UserInformation(string Username, string Name, IEnumerable<string> Roles);
