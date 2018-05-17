using System.Web.Http;
using WebActivatorEx;
using OAuthTokenBasedRestService;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Configuration;
using System;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace OAuthTokenBasedRestService
{
	public class SwaggerConfig
	{
		public static void Register()
		{
			var thisAssembly = typeof(SwaggerConfig).Assembly;
			var enableSwagger = ConfigurationManager.AppSettings["EnableSwagger"];
			if (Convert.ToBoolean(enableSwagger))
			{
				GlobalConfiguration.Configuration
					.EnableSwagger(c =>
						{
							c.SingleApiVersion("v1", "OAuthTokenBasedRestService");
							c.OperationFilter<AddRequiredAuthorizationHeaderParameter>();
						})
					.EnableSwaggerUi(c =>
						{
							c.EnableApiKeySupport("apiKey", "header");
						});
			}
		}
	}

	public class AddRequiredAuthorizationHeaderParameter : IOperationFilter
	{
		public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
		{
			if (operation.parameters == null)
				operation.parameters = new List<Parameter>();

			operation.parameters.Add(new Parameter
			{
				name = "Authorization",
				@in = "header",
				type = "string",
				required = false,
				description = "access token"
			});
		}
	}

}
