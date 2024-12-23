using Isotralis.Domain.ValueObjects;

namespace Isotralis.Domain.Entities;

public sealed class User
{
    public required UserInformation UserInformation { get; set; } = new UserInformation(string.Empty, string.Empty, Array.Empty<string>());

    public required Person NimsIdentity { get; set; }
}
