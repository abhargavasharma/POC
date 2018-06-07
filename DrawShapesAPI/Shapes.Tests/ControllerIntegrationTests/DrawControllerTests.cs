using Microsoft.Owin.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using ShapesApi;
using ShapesApi.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Shapes.Tests
{
	[TestFixture]
	public class DrawControllerTests
	{
		private TestServer _testServer;

		[OneTimeSetUp]
		public void Setup()
		{
			_testServer = TestServer.Create<Startup>();
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			_testServer.Dispose();
		}


		[TestCase("Draw a rectangle with a width of 250 and a height of 400", "Rectangle", HttpStatusCode.OK)]
		[TestCase("Draw a circle with a radius of 100", "Circle", HttpStatusCode.OK)]
		[TestCase("Draw a square with a side length of 200", "Square", HttpStatusCode.OK)]
		[TestCase("Draw an equilateral triangle with a height of 100 and a width of 190", "Equilateral", HttpStatusCode.OK)]
		[TestCase("Draw a oval with a width of 250 and a length of 400", "Oval", HttpStatusCode.OK)]
		[TestCase("Draw a square with a side length of 200", "Square", HttpStatusCode.OK)]
		public async Task GetShape_WithValidRequest_ReturnsExpected(string freeText, string expectedShape, HttpStatusCode expectedHttpStatusCode)
		{
			//arrange
			var expectedShapeName = expectedShape;
			var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/GetImageDetails/{freeText}");

			//act
			var getResponse = await _testServer.HttpClient.SendAsync(requestMessage);

			//assert
			NUnit.Framework.Assert.AreEqual(expectedHttpStatusCode, getResponse.StatusCode);

			var body = await getResponse.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeObject<Shape>(body);

			NUnit.Framework.Assert.AreEqual(result.Name, expectedShapeName);
		}
	}
}
