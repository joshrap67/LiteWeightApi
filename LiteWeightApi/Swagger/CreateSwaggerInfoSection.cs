using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NodaTime;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LiteWeightAPI.Swagger;

public class CreateSwaggerInfoSection : IConfigureOptions<SwaggerGenOptions>
{
	private readonly IWebHostEnvironment _environment;

	public CreateSwaggerInfoSection( IWebHostEnvironment environment)
	{
		_environment = environment;
	}

	public void Configure(SwaggerGenOptions options)
	{
		options.SwaggerDoc("v1", CreateApiInfo());
	}

	private OpenApiInfo CreateApiInfo()
	{
		var openApiInfo = new OpenApiInfo
		{
			Contact = new OpenApiContact
			{
				Email = "binary0010productions@gmail.com",
				Name = "Josh Rapoport",
				Url = new Uri("https://github.com/joshrap67")
			},
			Description = GetDescription(),
			Title = "LiteWeight API",
			Version = "v1"
		};

		return openApiInfo;
	}

	private string GetDescription()
	{
		var now = SystemClock.Instance.GetCurrentInstant();
		var lastUpdated = $"_Last updated {now.ToString()}_";

		var markdownPath = _environment.ContentRootFileProvider.GetFileInfo("Swagger/InfoDescription.md");
		var fileString = File.ReadAllText(markdownPath.PhysicalPath);
		return $"{fileString}\n\n\n{lastUpdated}";
	}
}