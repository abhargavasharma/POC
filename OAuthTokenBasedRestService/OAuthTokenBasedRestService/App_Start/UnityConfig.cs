using System.Reflection;
using System.Web.Mvc;
using OAuthTokenBasedRestService.DependencyInjection;
using Unity.AspNet.Mvc;

namespace OAuthTokenBasedRestService
{
	public static class UnityConfig
	{
		public static void RegisterComponents()
		{
			ObjectFactory.Configure(Assembly.GetExecutingAssembly());

			DependencyResolver.SetResolver(new UnityDependencyResolver(ObjectFactory.Container));
		}
	}
}