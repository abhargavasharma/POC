using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ShapesApi
{
    public class Middleware
    {
		private readonly RequestDelegate _next;

		public Middleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			// Do something with context near the beginning of request processing.

			//if (!TerminateRequest())
				await _next.Invoke(context);

			// Clean up.
		}
	}

	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<Middleware>();
		}
	}
}
