using LiteWeightAPI.ExtensionMethods;
using LiteWeightAPI.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, config) => config.WriteTo.Console());

builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureDependencies();
builder.Services.ConfigureApi();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureOptions(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// todo rate throttling

app.UseCustomExceptionHandling();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<EmailVerifiedMiddleware>();
app.MapControllers();

app.Run();