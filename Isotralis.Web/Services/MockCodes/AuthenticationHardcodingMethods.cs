using System.DirectoryServices.AccountManagement;

namespace Isotralis.Web.Services.MockCodes;

#pragma warning disable CA1416 // Validate platform compatibility

public sealed class AuthenticationHardcodingMethods
{
    // Mock method to return hardcoded authorization groups
    public static IEnumerable<Principal> GetHardcodedAuthorizationGroups()
    {
        // Create a PrincipalContext (can be mocked or null for this example)
        var principalContext = new PrincipalContext(ContextType.Machine);

        // Create mock Principal objects (representing AD groups)

        var group1 = new GroupPrincipal(principalContext)
        {
            Name = Constants.TechnicianUserRole
        };

        var group2 = new GroupPrincipal(principalContext)
        {
            Name = Constants.SupervisorUserRole
        };

        var group3 = new GroupPrincipal(principalContext)
        {
            Name = Constants.GeneralUserRole
        };

        // Return as a list of Principals
        return new List<Principal> { group1, group2, group3 };
    }


}

// Mock PrincipalContext to simulate domain functionality
public class MockPrincipalContext : IDisposable
{
    public string ContextType { get; }
    public string Domain { get; }

    public MockPrincipalContext(string contextType, string domain)
    {
        ContextType = contextType;
        Domain = domain;

        Console.WriteLine($"MockPrincipalContext created for context type: {contextType}, domain: {domain}");
    }

    public bool ValidateCredentials(string username, string password)
    {
        Console.WriteLine($"Validating credentials for user: {username}");
        // Simulate credential validation (hardcoded)
        return username == "TestUser" && password == "TestPassword";
    }

    public void Dispose()
    {
        Console.WriteLine("MockPrincipalContext disposed.");
    }
}

// Mock UserPrincipal to simulate user identity retrieval
public class MockUserPrincipal : IDisposable
{
    public string SamAccountName { get; set; }
    public string UserName { get; set; }
    public string DisplayName { get; set; }
    public string EmailAddress { get; set; }

    private MockUserPrincipal(string samAccountName, string userName, string displayName, string emailAddress)
    {
        SamAccountName = samAccountName;
        UserName = userName;
        DisplayName = displayName;
        EmailAddress = emailAddress;
    }

    public static MockUserPrincipal FindByIdentity(MockPrincipalContext context, string username)
    {
        Console.WriteLine($"Retrieving user information for: {username}");
        // Simulate user retrieval (hardcoded)
        if (username == "TestUser")
        {
            return new MockUserPrincipal(
                samAccountName: "TestSAM",
                userName: "TestUser",
                displayName: "TestDisplay",
                emailAddress: "bdurange0525@gmail.com"
            );
        }

        throw new InvalidOperationException($"User {username} not found.");
    }

    public void Dispose()
    {
        Console.WriteLine("MockUserPrincipal disposed.");
    }
}
#pragma warning restore CA1416 // Validate platform compatibility