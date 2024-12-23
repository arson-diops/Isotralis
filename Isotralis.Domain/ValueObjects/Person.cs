namespace Isotralis.Domain.ValueObjects;

public sealed record Person
{
    public long PersonId { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string? MiddleInitial { get; init; }

    public string NimsName { get; init; } = string.Empty;

    public int SiteBadgeNumber { get; init; }

    public List<PersonAlias> Aliases { get; set; } = [];
}
