namespace Isotralis.Web.Models;

public sealed record UserInformation(string Username, string Name, IEnumerable<string> Roles);
