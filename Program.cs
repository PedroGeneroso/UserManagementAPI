var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

var users = new List<User>
{
    new User { Id = 1, Name = "Brennan Lee Mulligan", UserAge = 38 },
    new User { Id = 2, Name = "Isabella Roland", UserAge = 31 }
};

app.MapGet("/users", () => users)
    .WithName("GetUsers");

app.MapPost("/users", (User user) =>
{
    if (string.IsNullOrWhiteSpace(user.Name))
    {
        return Results.BadRequest("Name is required.");
    }
    if (user.UserAge < 0)
    {
        return Results.BadRequest("UserAge must be a non-negative integer.");
    }
    if (users.Any(u => u.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase)))
    {
        return Results.Conflict("A user with the same name already exists.");
    }
    user.Id = users.Count +1;
    users.Add(user);
    return Results.Created($"/users/{user.Id}", user);
}).WithName("CreateUser");

app.MapPut("/users/{id}", (int id, User updatedUser) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null)
    {
        return Results.NotFound();
    }
    if (string.IsNullOrWhiteSpace(updatedUser.Name))
    {
        return Results.BadRequest("Name is required.");
    }
    if (updatedUser.UserAge < 0)
    {
        return Results.BadRequest("UserAge must be a non-negative integer.");
    }
    if (users.Any(u => u.Id != id && u.Name.Equals(updatedUser.Name, StringComparison.OrdinalIgnoreCase)))
    {
        return Results.Conflict("A user with the same name already exists.");
    }
    user.Name = updatedUser.Name;
    user.UserAge = updatedUser.UserAge;
    return Results.Ok(user);
})
.WithName("UpdateUser");

app.MapDelete("/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null)
    {
        return Results.NotFound();
    }
    users.Remove(user);
    return Results.NoContent();
}).WithName("DeleteUser");

app.Run();

public class User
{
    public int Id {get; set;}
    public string Name {get; set;}
    public int? UserAge {get; set;}
}

