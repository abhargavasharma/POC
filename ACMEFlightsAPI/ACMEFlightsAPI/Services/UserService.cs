using ACMEFlightsAPI.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace ACMEFlightsAPI.Service
{
	public interface IUserService
	{
		Task<User> GetUserByCredentialsAsync(string email, string password);
	}

	public class UserService : IUserService
	{
		public async Task<User> GetUserByCredentialsAsync(string username, string password)
		{
			if (username != null && password != null)
			{
				if (username == "admin" && password == "admin")
				{
					var user = new User { Id = "1", Name = "GOD", Role = "admin" };
					var resp = Utilities.Utility.CreateManualResponseFromObject(user, HttpStatusCode.OK);
					return await resp.Content.ReadAsAsync<User>();
				}
			}
			throw new HttpRequestException("Invalid username and password");
		}
	}
}