namespace Isotralis.App;

internal static class Constants
{
    internal const string NimsConnectionString = "Nims";

    internal const string GeneralUserRole = "Isotralis.Users.General";
    internal const string TechnicianUserRole = "Isotralis.Users.Technician";
    internal const string SupervisorUserRole = "Isotralis.Users.Supervisor";
    internal static readonly List<string> AllRoles = new List<string>
{
    GeneralUserRole,
    TechnicianUserRole,
    SupervisorUserRole
};
}
