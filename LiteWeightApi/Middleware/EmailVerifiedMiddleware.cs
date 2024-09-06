using LiteWeightAPI.Errors.Exceptions.BaseExceptions;

namespace LiteWeightAPI.Middleware;

public class EmailVerifiedMiddleware(RequestDelegate next)
{
	public async Task InvokeAsync(HttpContext context)
	{
		var emailVerified = context.User.Claims.ToList().FirstOrDefault(x => x.Type == "email_verified");
		if (emailVerified == null || !bool.Parse(emailVerified.Value))
		{
			throw new ForbiddenException("Email not verified");
		}

		await next(context);
	}
}