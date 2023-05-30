using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
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
FirebaseApp.Create(new AppOptions
{
	Credential = GoogleCredential.GetApplicationDefault(),
	ProjectId = builder.Configuration["LiteWeight_Firebase:ProjectId"]
});

app.UseCustomExceptionHandling();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<EmailVerifiedMiddleware>();
app.MapControllers();

app.Run();