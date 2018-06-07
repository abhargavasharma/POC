using NUnit.Framework;
using ShapesApi.Services;

namespace Shapes.Tests.Moq
{
	[TestFixture]
	class ShapesServiceTests
    {
		private IShapeService _shapeService;
		public ShapesServiceTests()
		{
			_shapeService = new ShapeService();
		}

		[TestCase("Draw a rectangle with a width of 250 and a height of 400", "Rectangle")]
		[TestCase("Draw a pentagon with a side length of 90", "Pentagon")]
		[TestCase("Draw a circle with a radius of 100", "Circle")]
		[TestCase("Draw a square with a side length of 200", "Square")]
		[TestCase("Draw an equilateral triangle with a height of 100 and a width of 190", "Equilateral")]
		[TestCase("Draw a oval with a width of 250 and a length of 400", "Oval")]
		[TestCase("Draw a square with a side length of 200", "Square")]
		public void DrawShape_WithValidInput_ReturnsExpected(string inputText, string shapeName)
		{
			var result = _shapeService.DrawShape(inputText);
			Assert.IsTrue(result.Result.Name == shapeName);
		}

		[TestCase("Draw a", "Invalid input")]
		[TestCase("Draw a pentagon with a side length of  ", "Invalid input")]
		[TestCase("Draw a circle with a diameter of 100", "Invalid input")]
		[TestCase("Draw a square with a width length of 200", "Invalid input")]
		[TestCase("", "Invalid input")]
		[TestCase("Draw a oval with a width of '250' and a length of '400'", "Invalid input")]
		public void DrawShape_WithInValidInput_ReturnsExpected(string inputText, string outputMessage)
		{
			var result = _shapeService.DrawShape(inputText);
			Assert.IsTrue(result.Result.Name == outputMessage);
		}

	}
}
