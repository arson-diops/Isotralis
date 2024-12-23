using Dapper;
using Isotralis.Infrastructure.Repositories.Queries;
using Isotralis.Domain.ValueObjects;

namespace Isotralis.Infrastructure.Repositories.Nims;

public sealed class NimsPersonsRepository : NimsBaseRepository
{
    public NimsPersonsRepository(IConfiguration config) : base(config.GetConnectionString("Nims")) { }

    public async Task<IEnumerable<Person>> GetPersonsAsync(CancellationToken cancellationToken = default)
    {
        CommandDefinition cmd = new(PersonsQueries.GetPersons, cancellationToken: cancellationToken);
        IEnumerable<Person> persons = await GetQueryResultsAsync<Person>(cmd);
        foreach (Person person in persons)
        {
            IEnumerable<PersonAlias>? aliases = await GetPersonAliasesAsync(person.PersonId, cancellationToken);
            person.Aliases.AddRange(aliases);
        }
        return persons;
    }

    public async Task<Person?> GetPersonByPerDbIdAsync(long perDbId, CancellationToken cancellationToken = default)
    {
        var parameters = new
        {
            perDbId = perDbId
        };
        CommandDefinition cmd = new(PersonsQueries.GetPersonByDbId, parameters, cancellationToken: cancellationToken);
        Person? person = await GetSingleQueryResultAsync<Person>(cmd);

        if (person != null)
        {
            IEnumerable<PersonAlias>? aliases = await GetPersonAliasesAsync(person.PersonId, cancellationToken);
            person.Aliases.AddRange(aliases);
        }
        return person;
    }

    public async Task<IEnumerable<PersonAlias>?> GetPersonAliasesAsync(long perDbId, CancellationToken cancellationToken = default)
    {
        var parameters = new
        {
            perDbId = perDbId
        };
        CommandDefinition cmd = new(PersonsQueries.GetPersonAliases, parameters, cancellationToken: cancellationToken);
        return await GetQueryResultsAsync<PersonAlias>(cmd);
    }
}
