using Isotralis.App.Services.MockCodes;
using Isotralis.Domain.Entities;
using Isotralis.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Isotralis.App.Services;

#pragma warning disable CA1416 // Validate platform compatibility
public sealed class WindowsAuthenticationService
{
    private readonly ILogger<WindowsAuthenticationService> _logger;

    public WindowsAuthenticationService(ILogger<WindowsAuthenticationService> logger)
    {
        _logger = logger;
    }

    public bool TryAuthenticateUser(string username, string password, [NotNullWhen(true)] out User? user)
    {
        user = null;

        using var pContext = new MockPrincipalContext("Domain", "CORP");

        _logger.LogInformation("Validating credentials for user '{Username}'.", username);

        if (!pContext.ValidateCredentials(username, password))
        {
            _logger.LogWarning("Failed to validate credentials.");
            return false;
        }

        _logger.LogInformation("Credential validation succeeded. Retrieving user information.");

        using var principalUser = MockUserPrincipal.FindByIdentity(pContext, username);

        if (principalUser is null)
        {
            _logger.LogWarning("Unable to retrieve user information.");
            return false;
        }

        _logger.LogInformation("Retrieved user information. Collecting Isotralis groups.");
        var authGroups = AuthenticationHardcodingMethods.GetHardcodedAuthorizationGroups();

        if (authGroups is null || !authGroups.Any())
        {
            _logger.LogWarning("User does not belong to any AD auth groups.");
            return false;
        }

        var isotralisGroups = authGroups
            .Where(group => group.Name.StartsWith("Isotralis", StringComparison.OrdinalIgnoreCase))
            .Select(group => group.Name)
            .ToList();

        _logger.LogInformation("Detected {Count} Isotralis groups.", isotralisGroups.Count);

        // Construct UserInformation
        var userInformation = new UserInformation(principalUser.SamAccountName, principalUser.DisplayName, isotralisGroups);

        // Lookup associated Person and Aliases (mocked here for example)
        var person = GetPersonByUsername(principalUser.SamAccountName);

        if (person == null)
        {
            _logger.LogWarning("No associated Person found for user '{Username}'.", username);
            return false;
        }

        _logger.LogInformation("Associated Person found. Creating User object.");

        // Construct the User object
        user = new User
        {
            UserInformation = userInformation,
            NimsIdentity = person
        };

        return true;
    }

    private Person GetPersonByUsername(string username)
    {
        // Mocked retrieval; replace with actual database/service call
        return username switch
        {
            "E210601" => new Person
            {
                PersonId = 6000210601,
                FirstName = "Benjamin",
                LastName = "Durange",
                NimsName = "BENJAMIN*C*DURANGE",
                Aliases = new List<PersonAlias>
                {
                    new PersonAlias { AliasId = "E210601", AliasType = "EMP", IsPrimaryAlias = true, PersonId = 6000210601 },
                    new PersonAlias { AliasId = "111-11-1111", AliasType = "SSN", IsPrimaryAlias = false, PersonId = 6000210601 }
                }
            },
            "E202020" => new Person
            {
                PersonId = 6000202020,
                FirstName = "Michael",
                LastName = "Butler",
                NimsName = "MICHAEL*BUTLER",
                Aliases = new List<PersonAlias>
                {
                    new PersonAlias { AliasId = "E202020", AliasType = "EMP", IsPrimaryAlias = true, PersonId = 6000202020 },
                    new PersonAlias { AliasId = "111-11-2222", AliasType = "SSN", IsPrimaryAlias = false, PersonId = 6000202020 }
                }
            },
            "E03994" => new Person
            {
                PersonId = 6000003994,
                FirstName = "Louis",
                LastName = "Durange",
                NimsName = "LOUIS*G*DURANGE",
                Aliases = new List<PersonAlias>
                {
                    new PersonAlias { AliasId = "E03994", AliasType = "EMP", IsPrimaryAlias = true, PersonId = 6000003994 },
                    new PersonAlias { AliasId = "111-11-3333", AliasType = "SSN", IsPrimaryAlias = false, PersonId = 6000003994 }
                }
            }
        };
    }
}
#pragma warning restore CA1416 // Validate platform compatibility