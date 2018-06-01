using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShapesApi
{
    public static class Utilities
    {
		public static HttpResponseMessage GetHttpResponseMessage(Object obj)
		{
			return new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(
						 Newtonsoft.Json.JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8,
						"application/json"),
			};
		}

		public static int GetDimention(string inputText, string DimensionName)
		{
			var textArray = inputText.Split(" ", StringSplitOptions.RemoveEmptyEntries);
			int dimention = 0;
			int indexCounter = 2;

			if (DimensionName == Constants.Side || DimensionName == Constants.LeftSide || DimensionName == Constants.RightSide || DimensionName == Constants.BottomSide)
				indexCounter = 3;

			for (int i = 0; i < textArray.Length; i++)
			{
				if (textArray[i] == DimensionName)
				{
					if (i + indexCounter < textArray.Length)
					{
						if (Regex.IsMatch(textArray[i + indexCounter], @"\d"))
						{
							dimention = Convert.ToInt32(textArray[i + indexCounter]);
						}
					}
				}
			}

			return dimention;
		}
	}
}
