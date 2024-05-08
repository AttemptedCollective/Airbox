using Airbox.Api.Core.Storage;
using Airbox.Api.Users.Storage;
using Airbox.Api.Users.Storage.InMemory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var userLocationStorage = new InMemoryUserLocationStorage();
builder.Services.AddSingleton<IUserStorage>(userLocationStorage);
builder.Services.AddSingleton<ILocationStorage>(userLocationStorage);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
