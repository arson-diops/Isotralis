using Dapper;
using Isotralis.Infrastructure.Repositories.Queries;
using Isotralis.Domain.ValueObjects;
using NLog;

namespace Isotralis.Infrastructure.Repositories.Nims;

public sealed class NimsPersonsRepository : NimsBaseRepository
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public NimsPersonsRepository(IConfiguration config) : base(config.GetConnectionString("Nims")) { }

    public async Task<IEnumerable<Person>> GetPersonsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            CommandDefinition cmd = new(PersonsQueries.GetPersons, cancellationToken: cancellationToken);
            IEnumerable<Person> persons = await GetQueryResultsAsync<Person>(cmd);

            foreach (Person person in persons)
            {
                try
                {
                    IEnumerable<PersonAlias>? aliases = await GetPersonAliasesAsync(person.PersonId, cancellationToken);
                    person.Aliases.AddRange(aliases);
                }
                catch (RepositoryException ex)
                {
                    Logger.Warn(ex, $"Failed to retrieve aliases for PersonId: {person.PersonId}. Proceeding with partial data.");
                }
            }

            return persons;
        }
        catch (RepositoryException ex)
        {
            Logger.Error(ex, "Error occurred while retrieving all persons.");
            throw;
        }
    }

    public async Task<Person?> GetPersonByPerDbIdAsync(long perDbId, CancellationToken cancellationToken = default)
    {
        try
        {
            var parameters = new { perDbId };
            CommandDefinition cmd = new(PersonsQueries.GetPersonByDbId, parameters, cancellationToken: cancellationToken);

            Person? person = await GetSingleQueryResultAsync<Person>(cmd);

            if (person == null)
            {
                Logger.Warn($"Person with PerDbId: {perDbId} not found.");
                return null;
            }

            try
            {
                IEnumerable<PersonAlias>? aliases = await GetPersonAliasesAsync(person.PersonId, cancellationToken);
                person.Aliases.AddRange(aliases);
            }
            catch (RepositoryException ex)
            {
                Logger.Warn(ex, $"Failed to retrieve aliases for PersonId: {person.PersonId}. Proceeding with partial data.");
            }

            return person;
        }
        catch (RepositoryException ex)
        {
            Logger.Error(ex, $"Error occurred while retrieving person with PerDbId: {perDbId}.");
            throw;
        }
    }

    public async Task<IEnumerable<PersonAlias>?> GetPersonAliasesAsync(long perDbId, CancellationToken cancellationToken = default)
    {
        var parameters = new { perDbId };
        CommandDefinition cmd = new(PersonsQueries.GetPersonAliases, parameters, cancellationToken: cancellationToken);

        try
        {
            return await GetQueryResultsAsync<PersonAlias>(cmd);
        }
        catch (RepositoryException ex)
        {
            Logger.Error(ex, $"Error occurred while retrieving aliases for PerDbId: {perDbId}.");
            throw;
        }
    }

    public async Task<Person> GetPersonByNimsUserIdAsync(string nimsUserId, CancellationToken cancellationToken = default)
    {
        var parameters = new { nimsUserId };
        CommandDefinition cmd = new(PersonsQueries.GetPersonByNimsUserId, parameters, cancellationToken: cancellationToken);
        try
        {
            
            Person? person = await GetSingleQueryResultAsync<Person>(cmd);
            if (person == null)
            {
                Logger.Warn($"Person with NimsUserId: {nimsUserId} not found.");
                throw new RepositoryException($"Person with NimsUserId: {nimsUserId} not found.", new InvalidDataException());
            }
            try
            {
                IEnumerable<PersonAlias>? aliases = await GetPersonAliasesAsync(person.PersonId, cancellationToken);
                person.Aliases.AddRange(aliases);
            }
            catch (RepositoryException ex)
            {
                Logger.Warn(ex, $"Failed to retrieve aliases for PersonId: {person.PersonId}. Proceeding with partial data.");
            }
            return person;
        }
        catch (RepositoryException ex)
        {
            Logger.Error(ex, $"Error occurred while retrieving person with NimsUserId: {nimsUserId}.");
            throw;
        }
    }
}
