namespace Isotralis.Domain.ValueObjects;

public sealed record PersonAlias
{
    public string AliasId { get; init; } = string.Empty;

    public string AliasType { get; init; } = string.Empty;

    public long PersonId { get; init; }

    public bool IsPrimaryAlias { get; init; }

    public bool NrcPrimaryFlag { get; init; }
}
