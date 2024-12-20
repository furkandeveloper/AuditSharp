using System.Net;
using System.Net.Http.Json;
using AuditSharp.Sample.Entities;

namespace AuditSharp.Tests;

public class PersonControllerIntegrationTests
{
    private readonly HttpClient _client;
    private readonly Guid _personId = Guid.NewGuid();

    public PersonControllerIntegrationTests()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:5000");
    }

    [Fact]
    public async Task PersonTests()
    {
        // Arrange
        var newPerson = new Person
        {
            Id = _personId,
            Name = "John",
            Surname = "Doe",
            BirthDate = DateTime.Now,
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/Person", newPerson);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdPerson = await response.Content.ReadFromJsonAsync<Person>();
        Assert.NotNull(createdPerson);
        Assert.Equal(newPerson.Name, createdPerson.Name);
        Assert.Equal(newPerson.Surname, createdPerson.Surname);
        Assert.Equal(newPerson.BirthDate, createdPerson.BirthDate);
        Assert.Equal(newPerson.Description, createdPerson.Description);

        // Act
        var getResponse = await _client.GetAsync("/Person");

        // Assert
        getResponse.EnsureSuccessStatusCode();
        var persons = await getResponse.Content.ReadFromJsonAsync<List<Person>>();
        Assert.NotNull(persons);
        Assert.NotEmpty(persons);

        // Arrange
        var id = _personId;
        var updatedPerson = new Person
        {
            Id = id,
            Name = "Jane",
            Surname = "Doe",
            BirthDate = DateTime.Now,
            Description = "Updated"
        };

        // Act
        var updateResponse = await _client.PutAsJsonAsync($"/Person/{id}", updatedPerson);

        // Assert
        updateResponse.EnsureSuccessStatusCode();
        var returnedPerson = await updateResponse.Content.ReadFromJsonAsync<Person>();
        Assert.NotNull(returnedPerson);
        Assert.Equal(updatedPerson.Name, returnedPerson.Name);
        Assert.Equal(updatedPerson.Surname, returnedPerson.Surname);
        Assert.Equal(updatedPerson.BirthDate, returnedPerson.BirthDate);
        Assert.Equal(updatedPerson.Description, returnedPerson.Description);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/Person/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}