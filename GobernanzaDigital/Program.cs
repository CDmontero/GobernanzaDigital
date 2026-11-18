
using Microsoft.EntityFrameworkCore;
using GobernanzaDigital.Data;
using GobernanzaDigital.Interfaces;
using GobernanzaDigital.Services;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddOpenApi();

builder.Services.AddDbContext<GobernanzaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("GobernanzaDb")));

builder.Services.AddScoped<IServicioService, ServicioService>();

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