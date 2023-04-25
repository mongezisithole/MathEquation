using MathEquation.Business.Data;
using MathEquation.Services.Implementations;
using MathEquation.Services.Interfaces;
using System.Net.NetworkInformation;
using System.Security.AccessControl;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(policy => {

    policy.AddPolicy("Policy_Name", builder =>
      builder.WithOrigins("*")
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
 );
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqliteConnectionFactory(
        builder.Configuration.GetValue<string>("Database:ConnectionString")));
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<IMathEquationService, MathEquationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Policy_Name");
app.UseHttpsRedirection();


app.MapPost("/calcEquation", async (string equation, double x_value, IMathEquationService mathEquationService) =>
{
    if (string.IsNullOrEmpty(equation))
    {
        return Results.BadRequest("Value cannot be null or empty!!");
    }

    var calcEquation = mathEquationService.CalculateEquation(equation, x_value);

    if(calcEquation.GetType() == typeof(List<string>))
    {
        return Results.BadRequest(calcEquation);
    }

    var mathEquation = calcEquation as MathEquation.Common.MathEquation;

    if(mathEquation == null)
    {
        return Results.BadRequest("Unknown error!!");
    }

    var savedEquation = await mathEquationService.SaveEquationAsync(mathEquation);

    if (!savedEquation)
    {
        return Results.BadRequest("Unknown error!!");
    }

    return Results.Ok(mathEquation);
})
.WithName("CalcEquation");

app.MapGet("/", async (IMathEquationService mathEquationService) =>
{
    var mathEquations = await mathEquationService.GetAllAsync();

    return Results.Ok(mathEquations);
});

var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.Run();