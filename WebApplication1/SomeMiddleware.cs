using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1
{
	public class SomeMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly string _options;

		public SomeMiddleware(RequestDelegate next, string options)
		{
			_next = next;
			_options = options;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			if (!RequestCheck(httpContext.Request))
			{
				await _next(httpContext);
				return;
			}

			RespondWithIndexHtml(httpContext.Response);
		}

		private bool RequestCheck(HttpRequest request)
		{
			var indexPath = string.IsNullOrEmpty(_options) ? "/" : $"/{_options}/";
			return (request.Method == "GET" && request.Path == indexPath);
		}

		private async void RespondWithIndexHtml(HttpResponse response)
		{
			response.StatusCode = 200;
			response.ContentType = "text/html";
			var some_html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>About - WebApplication1</title>
	<link rel='stylesheet' href='/lib/bootstrap/dist/css/bootstrap.css' />
	<link rel='stylesheet' href='/css/site.css' />
</head>
<body>

blablabla {_options}
 
</body>
</html>";
			//bug
			await response.WriteAsync(some_html, Encoding.UTF8);

			//Fixup
			//var bytes = Encoding.UTF8.GetBytes(some_html.ToString());
			//response.Body.Write(bytes, 0, bytes.Length);
		}
	}

	public static class SomeMiddlewareExtensions
	{
		public static IApplicationBuilder UseSomeMiddleware(this IApplicationBuilder app, Func<string, string> setupAction)
		{
			string options = "";
			options = setupAction?.Invoke(options);

			app.UseMiddleware<SomeMiddleware>(options);

			return app;
		}
	}
}
