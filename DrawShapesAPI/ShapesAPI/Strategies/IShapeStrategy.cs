using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShapesApi.Model;

namespace ShapesApi.Strategies
{
    interface IShapeStrategy
    {
		Task<Shape> DrawShape(string text, string shape);
	}
}
