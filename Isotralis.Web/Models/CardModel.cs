namespace Isotralis.Web.Models;

public record CardModel(
    string Title,
    string Description,
    string? Action = null,
    string? Controller = null,
    string? Area = null
    );