using System.DirectoryServices.AccountManagement;

namespace Isotralis.App.Services.MockCodes;

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

        // Use switch to handle multiple users
        return username switch
        {
            "E210601" when password == "Password" => true,
            "E03994" when password == "Password" => true,
            "E202020" when password == "Password" => true,
            _ => false
        };
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

        // Use switch to handle multiple users
        return username switch
        {
            "E210601" => new MockUserPrincipal(
                samAccountName: "E210601",
                userName: "E210601",
                displayName: "Benjamin Durange",
                emailAddress: "bdurange0525@gmail.com"
            ),
            "E03994" => new MockUserPrincipal(
                samAccountName: "E03994",
                userName: "E03994",
                displayName: "Louis Durange",
                emailAddress: "bdurange0525@gmail.com"
            ),
            "E202020" => new MockUserPrincipal(
                samAccountName: "E202020",
                userName: "E202020",
                displayName: "Mike Butler",
                emailAddress: "bdurange0525@gmail.com"
            ),
            _ => throw new InvalidOperationException($"User {username} not found.")
        };
    }

    public void Dispose()
    {
        Console.WriteLine("MockUserPrincipal disposed.");
    }
}
#pragma warning restore CA1416 // Validate platform compatibility