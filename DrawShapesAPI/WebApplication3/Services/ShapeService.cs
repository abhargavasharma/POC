using System;
using System.Threading.Tasks;
using ShapesApi.Model;
using ShapesApi.Strategies;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

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
			var regex = Startup.Configuration["InputPattern"];
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
					case ShapeType.Parallelogram:
					case ShapeType.Equilateral:
					case ShapeType.Isosceles:
					case ShapeType.Scalene:
						shapeStrategy = new RectangleStrategy();
						return shapeStrategy.DrawShape(inputText, desiredShape);
					case ShapeType.Circle:
						shapeStrategy = new CircleStrategy();
						return shapeStrategy.DrawShape(inputText, desiredShape);
					case ShapeType.Oval:
						shapeStrategy = new OvalStrategy();
						return shapeStrategy.DrawShape(inputText, desiredShape);
					case ShapeType.Square:
					case ShapeType.Pentagon:
					case ShapeType.Hexagon:
					case ShapeType.Heptagon:
					case ShapeType.Octagon:
						shapeStrategy = new SquareStrategy();
						return shapeStrategy.DrawShape(inputText, desiredShape);
					default:
						break;
				}
			}

			throw new FormatException("Invalid input");
		}

		public bool IsValidFreeText(string text, string regex)
		{
			Regex rgx = new Regex(regex);
			return rgx.IsMatch(text);
		}

	}
}
