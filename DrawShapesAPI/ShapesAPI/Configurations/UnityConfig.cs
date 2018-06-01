using System.Reflection;
using ShapesApi.DependencyInjection;
using Unity.AspNet.Mvc;
using System.Web.Mvc;

namespace ShapesApi.Configurations
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
