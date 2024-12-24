namespace Isotralis.Infrastructure.Repositories.Queries;

public static class PersonsQueries
{
    public const string GetPersons = @"
        SELECT 
            per_db_id AS PersonId,
            first_name AS FirstName,
            last_name AS LastName,
            middle_initial AS MiddleInitial,
            name AS NimsName,
            site_badge AS SiteBadgeNumber
        FROM nims.persons
        ";
    public const string GetPersonByDbId = @"
        SELECT 
            per_db_id AS PersonId,
            first_name AS FirstName,
            last_name AS LastName,
            middle_initial AS MiddleInitial,
            name AS NimsName,
            site_badge AS SiteBadgeNumber
        FROM nims.persons
        WHERE per_db_id = :perDbId
        ";

    public const string GetPersonAliases = @"
        SELECT 
            alias_id AS AliasId,
            alias_type AS AliasType,
            per_db_id AS PersonId,
            CASE WHEN primary_flag = 'Y' THEN 1 ELSE 0 END AS IsPrimaryAlias,
            CASE WHEN nrc_primary_flag = 'Y' THEN 1 ELSE 0 END AS NrcPrimaryFlag
        FROM nims.person_aliases
        WHERE per_db_id = :perDbId
        ";

    public const string GetPersonByNimsUserId = @"
        SELECT 
            per_db_id AS PersonId,
            first_name AS FirstName,
            last_name AS LastName,
            middle_initial AS MiddleInitial,
            name AS NimsName,
            site_badge AS SiteBadgeNumber
        FROM nims.persons
        WHERE nims_user_id = :nimsUserId
        ";
}
