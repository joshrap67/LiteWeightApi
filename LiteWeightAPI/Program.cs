using LiteWeightAPI.ExtensionMethods;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, config) => config
	.WriteTo.Console());

builder.Services.SetupAuthentication();
builder.Services.SetupDependencies(builder.Configuration);
builder.Services.SetupApi();
builder.Services.SetupSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCustomExceptionHandling();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();