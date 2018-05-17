using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OAuthTokenBasedRestService.Models
{
	public class User
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }

	}
}