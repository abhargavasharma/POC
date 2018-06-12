using System.Web.Http;
using WebActivatorEx;
using ACMEFlightsAPI;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Configuration;
using System;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace ACMEFlightsAPI
{
	public class SwaggerConfig
	{
		public static void Register()
		{
			var enableSwagger = ConfigurationManager.AppSettings["EnableSwagger"];
			if (Convert.ToBoolean(enableSwagger))
			{
                GlobalConfiguration.Configuration
                    .EnableSwagger(c =>
                        {
                        c.SingleApiVersion("v1", "ACMEFlightsAPI");
                        c.OperationFilter<AddRequiredAuthorizationHeaderParameter>();
                        c.DescribeAllEnumsAsStrings(true);
                        c.IncludeXmlComments(string.Format(@"{0}\bin\ACMEFlightsAPI.XML",
                           System.AppDomain.CurrentDomain.BaseDirectory));
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
            var isAuthorizationHeaderRequired = Convert.ToBoolean(ConfigurationManager.AppSettings["IsAuthorizationHeaderRequired"]);

            if (operation.parameters == null)
				operation.parameters = new List<Parameter>();

			operation.parameters.Add(new Parameter
			{
				name = "Authorization",
				@in = "header",
				type = "string",
				required = isAuthorizationHeaderRequired,
				description = "OAuth Access token"
			});
		}
	}

}
