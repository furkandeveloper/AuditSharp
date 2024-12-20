using AuditSharp.Sample.DataAccess;
using AuditSharp.Sample.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuditSharp.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase
{
    private readonly ExampleDbContext _dbContext;
    private readonly ILogger<PersonController> _logger;

    public PersonController(ILogger<PersonController> logger, ExampleDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet(Name = "GetPersons")]
    public async Task<List<Person>> Get()
    {
        return await _dbContext.Persons.ToListAsync();
    }

    /// <summary>
    ///     HTTP POST endpoint to create a new Person entity.
    /// </summary>
    /// <param name="person">The Person entity to be created, provided in the request body.</param>
    /// <returns>The created Person entity.</returns>
    [HttpPost(Name = "CreatePerson")]
    public async Task<Person> Create([FromBody] Person person)
    {
        // Adds the new Person entity to the DbContext.
        var entity = await _dbContext.Persons.AddAsync(person);

        // Saves the changes to the database.
        await _dbContext.SaveChangesAsync();

        // Returns the created Person entity.
        return entity.Entity;
    }

    /// <summary>
    ///     HTTP PUT endpoint to update an existing Person entity.
    /// </summary>
    /// <param name="id">The ID of the Person entity to be updated.</param>
    /// <param name="person">The updated Person entity, provided in the request body.</param>
    /// <returns>The updated Person entity.</returns>
    [HttpPut("{id}", Name = "UpdatePerson")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Person person)
    {
        var existingPerson = await _dbContext.Persons.FindAsync(id);
        if (existingPerson == null) return NotFound();

        existingPerson.Name = person.Name;
        existingPerson.Surname = person.Surname;
        existingPerson.BirthDate = person.BirthDate;
        existingPerson.Description = person.Description;

        await _dbContext.SaveChangesAsync();

        return Ok(existingPerson);
    }

    /// <summary>
    ///     HTTP DELETE endpoint to delete an existing Person entity.
    /// </summary>
    /// <param name="id">The ID of the Person entity to be deleted.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpDelete("{id}", Name = "DeletePerson")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existingPerson = await _dbContext.Persons.FindAsync(id);
        if (existingPerson == null) return NotFound();

        _dbContext.Persons.Remove(existingPerson);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}