
using Microsoft.EntityFrameworkCore;
using GobernanzaDigital.Data;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<GobernanzaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("GobernanzaDb")));

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();