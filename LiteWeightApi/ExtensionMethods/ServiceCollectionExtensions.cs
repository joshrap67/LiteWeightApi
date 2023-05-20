using System.Reflection;
using LiteWeightAPI.Errors.Exceptions;
using LiteWeightAPI.Errors.Responses;
using LiteWeightAPI.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetCore.AutoRegisterDi;
using NodaTime;

namespace LiteWeightAPI.ExtensionMethods;

public static class ServiceCollectionExtensions
{
	public static void SetupApi(this IServiceCollection services)
	{
		services.AddControllers();
		services.AddHttpContextAccessor();
		services.AddRouting(options => options.LowercaseUrls = true);
		services.Configure<ApiBehaviorOptions>(options =>
		{
			// anytime models are incorrectly bound or have validation error, throw an error that ensures an explicit, defined response
			options.InvalidModelStateResponseFactory = ModelBindingErrorHandler;
		});
	}

	public static void SetupDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAutoMapper(typeof(Program));
		services.AddSingleton<IClock>(SystemClock.Instance);
		// auto registers all classes that inherit from an interface as transient
		services.RegisterAssemblyPublicNonGenericClasses().AsPublicImplementedInterfaces();
	}

	public static void SetupSwagger(this IServiceCollection services)
	{
		services.AddSwaggerGen(options =>
		{
			// generate docs from xml comments on methods/models
			var assemblyXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
			var assemblyXmlPath = Path.Combine(AppContext.BaseDirectory, assemblyXml);
			options.IncludeXmlComments(assemblyXmlPath);

			// generate default responses for all endpoints
			options.OperationFilter<DefaultResponsesOperationFilter>();

			// append any error types that are attributed to each endpoint
			options.OperationFilter<AppendErrorTypesOperationFilter>();

			// append an indicator of which actions send a push notification
			options.OperationFilter<AppendPushNotificationIndicatorOperationFilter>();

			const string bearerDefinition = "BearerDefinition";
			options.AddSecurityDefinition(bearerDefinition, new OpenApiSecurityScheme
			{
				BearerFormat = "JWT",
				Description =
					"Token authentication. \n\n 'Bearer TOKEN'\n\nBearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
				In = ParameterLocation.Header,
				Name = "Authorization",
				Scheme = "Bearer",
				Type = SecuritySchemeType.Http
			});
		});
	}

	public static void SetupAuthentication(this IServiceCollection services)
	{
		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(jwt =>
		{
			jwt.Authority = "https://securetoken.google.com/liteweight-faa1a"; // todo env var on config
			jwt.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateLifetime = true,
				ValidateAudience = false
			};
		});
	}

	private static IActionResult ModelBindingErrorHandler(ActionContext context)
	{
		var errors = context.ModelState.Keys
			.SelectMany(key => context.ModelState[key]?.Errors.Select(x => new ModelBindingError
			{
				Property = key,
				Message = x.ErrorMessage
			}))
			.ToList();
		throw new InvalidRequestException("Invalid request", errors);
	}
}