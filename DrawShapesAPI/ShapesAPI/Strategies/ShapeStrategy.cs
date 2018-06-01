using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ShapesApi.Model;

namespace ShapesApi.Strategies
{
	public class RectangleStrategy : IShapeStrategy
	{
		public async Task<Shape> DrawShape(string text, string shapeName)
		{
			int width = Utilities.GetDimention(text, Constants.Width);
			int height = Utilities.GetDimention(text, Constants.Height);

			if (width > 0 && height > 0)
			{
				var shape = new Rectangle()
				{
					Name = shapeName,
					Width = width,
					Height = height
				};

				var resp = Utilities.GetHttpResponseMessage(shape);

				return await resp.Content.ReadAsAsync<Rectangle>();
			}
			else
			{
				throw new FormatException("Invalid input");
			}
		}
	}

	public class CircleStrategy : IShapeStrategy
	{
		public async Task<Shape> DrawShape(string text, string shapeName)
		{
			int radius = Utilities.GetDimention(text, Constants.Radius);

			if (radius > 0)
			{
				var shape = new Circle()
				{
					Name = shapeName,
					Radius = radius
				};

				var resp = Utilities.GetHttpResponseMessage(shape);

				return await resp.Content.ReadAsAsync<Circle>();
			}
			else
			{
				throw new FormatException("Invalid input");
			}
		}
	}

	public class SquareStrategy : IShapeStrategy
	{
		public async Task<Shape> DrawShape(string text, string shapeName)
		{
			int side = Utilities.GetDimention(text, Constants.Side);

			if (side > 0)
			{
				var shape = new Square()
				{
					Name = shapeName,
					Side = side
				};

				var resp = Utilities.GetHttpResponseMessage(shape);

				return await resp.Content.ReadAsAsync<Square>();
			}
			else
			{
				throw new FormatException("Invalid input");
			}
		}
	}

	public class OvalStrategy : IShapeStrategy
	{
		public async Task<Shape> DrawShape(string text, string shapeName)
		{
			int length = Utilities.GetDimention(text, Constants.Length);
			int width = Utilities.GetDimention(text, Constants.Width);

			if (width > 0 && length > 0)
			{
				var shape = new Oval()
				{
					Name = shapeName,
					Width = width,
					Length = length
				};

				var resp = Utilities.GetHttpResponseMessage(shape);

				return await resp.Content.ReadAsAsync<Oval>();
			}
			else
			{
				throw new FormatException("Invalid input");
			}
		}
	}

	public class TriangleStrategy : IShapeStrategy
	{
		public async Task<Shape> DrawShape(string text, string shapeName)
		{
			int leftSide = Utilities.GetDimention(text, Constants.LeftSide);
			int rightSide = Utilities.GetDimention(text, Constants.RightSide);
			int bottomSide = Utilities.GetDimention(text, Constants.BottomSide);


			if (leftSide > 0 && rightSide > 0 && bottomSide > 0)
			{
				var shape = new Triangle()
				{
					Name = shapeName,
					LeftSide = leftSide,
					RightSide = rightSide,
					BottomSide = bottomSide
				};

				var resp = Utilities.GetHttpResponseMessage(shape);

				return await resp.Content.ReadAsAsync<Triangle>();
			}
			else
			{
				throw new FormatException("Invalid input");
			}
		}
	}

}
