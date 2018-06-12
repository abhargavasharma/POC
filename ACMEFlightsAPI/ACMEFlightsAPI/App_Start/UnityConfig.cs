using System.Reflection;
using System.Web.Mvc;
using ACMEFlightsAPI.DependencyInjection;
using Unity.AspNet.Mvc;

namespace ACMEFlightsAPI
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