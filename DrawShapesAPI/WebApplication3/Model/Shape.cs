using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShapesApi.Model
{
    public class Shape
    {
		public string Name { get; set; }
	}

	public class Rectangle : Shape
	{
		public int Width { get; set; }
		public int Height { get; set; }
	}

	public class Circle : Shape
	{
		public int Radius { get; set; }
	}

	public class Square : Shape
	{
		public int Side { get; set; }
	}

	public class Oval : Shape
	{
		public int Length { get; set; }
		public int Width { get; set; }
	}

	public class Triangle : Shape
	{
		public int LeftSide { get; set; }
		public int RightSide { get; set; }
		public int BottomSide { get; set; }

	}
}
