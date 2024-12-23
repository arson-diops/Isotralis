using Isotralis.App.Services.MockCodes;
using Isotralis.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;

namespace Isotralis.App.Services;

public sealed class WindowsAuthenticationService
{
    private readonly ILogger<WindowsAuthenticationService> _logger;

    public WindowsAuthenticationService(ILogger<WindowsAuthenticationService> logger)
    {
        _logger = logger;
    }

    public bool TryAuthenticateUser(string username, string password, [NotNullWhen(true)] out UserInformation? userInformation)
    {
        userInformation = null;

#pragma warning disable CA1416 // Validate platform compatibility
        //using PrincipalContext pContext = new(ContextType.Domain, "CORP");
        using MockPrincipalContext pContext = new("Domain", "CORP");

        _logger.LogInformation("Validating credentials for user '{Username}'.", username);

        if (!pContext.ValidateCredentials(username, password))
        {
            _logger.LogWarning("Failed to validate credentials.");
            return false;
        }

        _logger.LogInformation("Credential validation succeeded. Retrieving user information.");

        //using UserPrincipal user = UserPrincipal.FindByIdentity(pContext, username);
        using MockUserPrincipal user = MockUserPrincipal.FindByIdentity(pContext, username);

        if (user is null)
        {
            _logger.LogWarning("Unable to retrieve user information.");
            return false;
        }

        _logger.LogInformation("Retrieved user information. Collecting Isotralis groups.");


        //using PrincipalSearchResult<Principal>? authGroups = user.GetAuthorizationGroups();
        var authGroups = AuthenticationHardcodingMethods.GetHardcodedAuthorizationGroups();

        if (authGroups is null || !authGroups.Any())
        {
            _logger.LogWarning("User does not belong to any AD auth groups.");
            return false;
        }

        List<string> isotralisGroups = authGroups
            .Where(static group => group.Name.StartsWith("Isotralis", StringComparison.OrdinalIgnoreCase))
            .Select(static group => group.Name)
            .ToList();

        _logger.LogInformation("Detected {Count} Isotralis groups.", isotralisGroups.Count);

        userInformation = new(user.SamAccountName, user.DisplayName, isotralisGroups);
#pragma warning restore CA1416 // Validate platform compatibility

        return true;
    }
}
