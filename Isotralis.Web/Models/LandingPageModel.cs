namespace Isotralis.Web.Models;

public sealed record LandingPageModel
{
    public IEnumerable<CardModel> Cards { get; init; } = [];
}