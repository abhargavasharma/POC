using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using ShapesApi.Configurations;
using Swashbuckle.AspNetCore.Swagger;

namespace ShapesApi
{
	public class Startup
	{
		public static IConfiguration Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();

			//Swagger API Page config
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info
				{
					Version = "v1",
					Title = "Shapes API",
					Description = "Draw Shapes Web API",
					TermsOfService = "None"
				});
			});

			Configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables()
			.Build();

			services.AddCors(o => o.AddPolicy("ShapesApiPolicy", builder =>
			{
				builder.AllowAnyOrigin()
					   .AllowAnyMethod()
					   .AllowAnyHeader();
			}));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();
			app.UseCors("ShapesApiPolicy");
			app.UseMiddleware<Middleware>();//Calling my middleware

			//Unity config
			UnityConfig.RegisterComponents();

			//Serilog config
			Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			.Enrich.FromLogContext()
			.WriteTo.File($"{Configuration["SerilogLogFilePath"]}\\DrawShapes-.txt",
							fileSizeLimitBytes: 1_000_000,
							rollOnFileSizeLimit: true,
							rollingInterval: RollingInterval.Day,
							shared: true,
							flushToDiskInterval: TimeSpan.FromSeconds(10))
			.CreateLogger();

			if (Convert.ToBoolean(Configuration["EnableSwagger"]))
			{
				//Swagger config
				app.UseSwagger(c =>
				{
					c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
				});
				//Swagger UI config
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shapes API V1");
				});
			}

			app.Run(async (context) =>
			{
				//@ Application start log file creation
				Log.Error(new Exception("Defined URI is not working properly"), $"Application Accidentally Called @:{DateTime.Now}");
				await context.Response.WriteAsync("Please Contact administrator for accessing this site.");
			});
		}
	}
}
