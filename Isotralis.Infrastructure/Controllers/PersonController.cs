using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Isotralis.Infrastructure.Repositories.Nims;
using Isotralis.Domain.ValueObjects;

namespace Isotralis.Infrastructure.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PersonController : ControllerBase
{
    private readonly ILogger<PersonController> _logger;
    private readonly NimsPersonsRepository _nimsPersonsRepository;

    public PersonController(ILogger<PersonController> logger, NimsPersonsRepository nimsPersonsRepository)
    {
        _logger = logger;
        _nimsPersonsRepository = nimsPersonsRepository;
    }

    /// <summary>
    /// Gets the entire persons database.
    /// </summary>
    /// <returns>A list of person entities.</returns>
    [HttpGet("all", Name = "GetAllPersons")]
    public async Task<IActionResult> GetPersonsAsync()
    {
        IEnumerable<Person> persons = await _nimsPersonsRepository.GetPersonsAsync();
        return Ok(persons);
    }

    /// <summary>
    /// Gets a person object based of a database ID.
    /// </summary>
    /// <returns>A single person entity.</returns>
    [HttpGet("{perDbId:long}", Name = "GetPersonByDatabaseId")]
    public async Task<IActionResult> GetPersonByDbIdAsync([FromQuery] long perDbId)
    {   
        Person? person = await _nimsPersonsRepository.GetPersonByPerDbIdAsync(perDbId);
        return Ok(person);
    }

    [HttpGet("{nimsUserId}", Name = "GetPersonByNimsUserId")]
    public async Task<IActionResult> GetPersonByNimsUserIdAsync([FromQuery] string nimsUserId)
    {
        Person? person = await _nimsPersonsRepository.GetPersonByNimsUserIdAsync(nimsUserId);
        Console.WriteLine("Hello World!");
        return Ok(person);
    }
}
