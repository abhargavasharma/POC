using Owin;
using System.Web.Http;
using Serilog;
using Serilog.Exceptions;

namespace OAuthTokenBasedRestService
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);

			//Install serilog using bewlow commands
			//Install-Package Serilog
			//Install-Package Serilog.Exceptions
			//Install - Package Serilog.Sinks.File

			//Log configuration in  .cs file (not recommended)
			//Log.Logger = new LoggerConfiguration()
			//.Enrich.WithExceptionDetails()
			//.WriteTo.File(path: @"C:\Logs\OauthTokenBasedRestService\OauthTokenBasedRestService-.txt"
			// , restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose
			// ,outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
			// ,rollingInterval: RollingInterval.Day
			// ,retainedFileCountLimit: 10)
			//.CreateLogger();

			//Preferred way
			//if you install AppSetting configuration using below command then use appsettings in web.config to configure Serilog
			//Install-Package Serilog.Settings.AppSettings
			//Install-Package Serilog.Sinks.MSSqlServer --> for sql server logging
			Log.Logger = new LoggerConfiguration()
				.Enrich.WithExceptionDetails()
			.ReadFrom.AppSettings()
			.CreateLogger();

			//sql server logging using serilog need below table to be created manually in your databse
			//CREATE TABLE[Logs] (
			//[Id] int IDENTITY(1,1) NOT NULL,
			//[Message] nvarchar(max) NULL,
			//[MessageTemplate] nvarchar(max) NULL,
			//[Level] nvarchar(128) NULL,
			//[TimeStamp] datetimeoffset(7) NOT NULL,  -- use datetime for SQL Server pre-2008
			//[Exception] nvarchar(max) NULL,
			//[Properties]
			//xml NULL,
			//[LogEvent] nvarchar(max) NULL
			//CONSTRAINT[PK_Logs]
			//PRIMARY KEY CLUSTERED([Id] ASC)
			//WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
			//ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
			//ON[PRIMARY]
			//) ON[PRIMARY];

			HttpConfiguration config = new HttpConfiguration();

			WebApiConfig.Register(config);
		}

	}
}