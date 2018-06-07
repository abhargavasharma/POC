using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShapesApi.Services;
using ShapesApi.DependencyInjection;
using Serilog;
using Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using Microsoft.AspNetCore.Cors;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShapesApi
{
    public class DrawController : Controller
    {
		private readonly IShapeService _shapeService;

		public DrawController()
		{
			_shapeService = ObjectFactory.Resolve<IShapeService>();
		}

		//http://localhost:58489/api/Draw/GetImageDetails/Draw an isosceles triangle with a height of 200 and a width of 100

		[HttpGet]
		[Route("api/draw/getimagedetails/{text}")]
		[EnableCors("ShapesApiPolicy")]
		[SwaggerResponse((int)HttpStatusCode.BadRequest)]
		[SwaggerResponse((int)HttpStatusCode.InternalServerError)]
		[SwaggerResponse((int)HttpStatusCode.OK, typeof(Model.Shape))]
		public async Task<IActionResult> GetImageDetails(string text)
		{
			try
			{
				var shape = await _shapeService.DrawShape(text);

				return Ok(shape);
			}
			catch (FormatException ex)
			{
				Log.Error(ex, $"Input is in invalid format: {text}");
				return BadRequest("Provided input is invalid, corret format is: Draw a(n) <shape> with a(n) <measurement> of <amount> (and a(n)< measurement > of<amount>)");
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Internal error: {text}");
				return StatusCode((int)HttpStatusCode.InternalServerError, "Please contact the administrator.");
			}
		}
	}
}
