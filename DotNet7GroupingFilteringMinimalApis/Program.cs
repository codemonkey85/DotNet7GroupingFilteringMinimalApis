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

var people = new Person[] { new("Michael"), new("Shannan"), new("Caleb"), new("Ethan"), };

personGroup.MapGet("", () => people).AddEndpointFilter<PeopleFilter>();
personGroup.MapGet("{id:int}", (int id) => people.FirstOrDefault());

app.Run();

internal record Person(string Name);

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
