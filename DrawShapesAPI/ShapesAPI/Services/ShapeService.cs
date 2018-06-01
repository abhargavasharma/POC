using System;
using System.Threading.Tasks;
using ShapesApi.Model;
using ShapesApi.Strategies;
using System.Text.RegularExpressions;

namespace ShapesApi.Services
{
	public interface IShapeService
	{
		Task<Shape> DrawShape(string text);
	}

	public class ShapeService : IShapeService
	{
		public Task<Shape> DrawShape(string inputText)
		{
			IShapeStrategy shapeStrategy = null;
			var regex = "^[a-z ]* [0-9]*( [a-z ]* [0-9]*)?$";// can be configured for time being hard coded
			inputText = inputText.ToLower();
			//Draw a(n) <shape> with a(n) <measurement> of <amount> (and a(n)< measurement > of<amount>)
			if (IsValidFreeText(inputText, regex))
			{
				var shapes = Enum.GetNames(typeof(ShapeType));
				var desiredShape = string.Empty;

				foreach (var shape in shapes)
				{
					if (inputText.Contains(shape.ToLower()))
					{
						desiredShape = shape;
						break;
					}
				}

				ShapeType desiredShapeType = (ShapeType)Enum.Parse(typeof(ShapeType), desiredShape);

				switch (desiredShapeType)
				{
					case ShapeType.Rectangle:
						shapeStrategy = new RectangleStrategy();
						return shapeStrategy.DrawShape(inputText, Constants.Rectangle);
					case ShapeType.Circle:
						shapeStrategy = new CircleStrategy();
						return shapeStrategy.DrawShape(inputText, Constants.Circle);
					case ShapeType.Oval:
						shapeStrategy = new OvalStrategy();
						return shapeStrategy.DrawShape(inputText, Constants.Oval);
					case ShapeType.Square:
						shapeStrategy = new SquareStrategy();
						return shapeStrategy.DrawShape(inputText, Constants.Square);
					case ShapeType.Parallelogram:
						shapeStrategy = new RectangleStrategy();
						return shapeStrategy.DrawShape(inputText, Constants.Parallelogram);
					case ShapeType.Pentagon:
						shapeStrategy = new SquareStrategy();
						return shapeStrategy.DrawShape(inputText, Constants.Pentagon);
					case ShapeType.Hexagon:
						shapeStrategy = new SquareStrategy();
						return shapeStrategy.DrawShape(inputText, Constants.Hexagon);
					case ShapeType.Heptagon:
						shapeStrategy = new SquareStrategy();
						return shapeStrategy.DrawShape(inputText, Constants.Heptagon);
					case ShapeType.Octagon:
						shapeStrategy = new SquareStrategy();
						return shapeStrategy.DrawShape(inputText, Constants.Octagon);
					case ShapeType.Equilateral:
						shapeStrategy = new RectangleStrategy();
						return shapeStrategy.DrawShape(inputText, Constants.Equilateral);
					case ShapeType.Isosceles:
						shapeStrategy = new RectangleStrategy();
						return shapeStrategy.DrawShape(inputText, Constants.Isosceles);
					case ShapeType.Scalene:
						shapeStrategy = new RectangleStrategy();
						return shapeStrategy.DrawShape(inputText, Constants.Scalene);
					default:
						break;
				}
			}

			throw new FormatException("Invalid input");
		}

		private bool IsValidFreeText(string text, string regex)
		{
			Regex rgx = new Regex(regex);
			return rgx.IsMatch(text);
		}

	}
}
