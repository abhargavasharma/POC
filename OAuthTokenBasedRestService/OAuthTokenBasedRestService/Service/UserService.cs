using OAuthTokenBasedRestService.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace OAuthTokenBasedRestService.Service
{
	public interface IUserService
	{
		Task<User> GetUserByCredentialsAsync(string email, string password);
	}

	public class UserService : IUserService
	{
		public async Task<User> GetUserByCredentialsAsync(string email, string password)
		{
			if (email != null && password != null)
			{
				if (email == "bhargav@bhargav.com" && password == "123456")
				{
					var user = new User { Id = "1", Email = "bhargav@bhargav.com", Name = "Bhargav", Password = "123456", Role = "user" };
					var resp = CreateManualResponseFromObject(user, HttpStatusCode.OK);
					return await resp.Content.ReadAsAsync<User>();
				}
				if (email == "bhargav@test.com" && password == "123456")
				{
					var user = new User() { Id = "1", Email = "bhargav@test.com", Name = "Bhargav", Password = "123456", Role = "admin" };
					var resp = CreateManualResponseFromObject(user, HttpStatusCode.OK);
					return await resp.Content.ReadAsAsync<User>();
				}
			}
			return null;
		}

		private HttpResponseMessage CreateManualResponseFromObject(object value, HttpStatusCode code)
		{
			return new HttpResponseMessage
			{
				StatusCode = code,
				Content = new StringContent(
						 Newtonsoft.Json.JsonConvert.SerializeObject(value), System.Text.Encoding.UTF8,
						"application/json"),
			};
		}
	}
}