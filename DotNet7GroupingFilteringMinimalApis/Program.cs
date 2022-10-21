var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var apiGroup = app.MapGroup("api");
var personGroup = apiGroup.MapGroup("people");

var people = new Person[] { new(1, "Michael"), new(2, "Shannan"), new(3, "Caleb"), new(4, "Ethan"), };

personGroup.MapGet("", () => people).AddEndpointFilter<PeopleFilter>();
personGroup.MapGet("{id:int}", (int id) => people.FirstOrDefault(person => person.Id == id));

app.Run();

internal record Person(int Id, string Name);

internal class PeopleFilter : IEndpointFilter
{
    public virtual async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var result = await next(context);
        return result is Person[] persons
            ? persons.Where(person => person is not { Name: "Michael" })
            : result;
    }
}
