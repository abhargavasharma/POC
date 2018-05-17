using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Lifetime;
using Unity.RegistrationByConvention;

namespace OAuthTokenBasedRestService.DependencyInjection
{
	public sealed class ObjectFactory
	{
		private static readonly UnityContainer UnityContainer = new UnityContainer();

		private static bool _isConfigured;

		public static UnityContainer Container => UnityContainer;

		public static bool IsRegistered(Type fromType)
		{
			return UnityContainer.IsRegistered(fromType);
		}

		public static T Resolve<T>()
		{
			return UnityContainer.Resolve<T>();
		}

		public static T Resolve<T>(string name)
		{
			return UnityContainer.Resolve<T>(name);
		}

		public static IEnumerable<T> ResolveAll<T>()
		{
			return UnityContainer.ResolveAll<T>();
		}

		public static void RegisterInstance(Type fromType, object instance, bool reRegister = false)
		{
			if (reRegister || !UnityContainer.IsRegistered(fromType))
			{
				UnityContainer.RegisterInstance(fromType, instance);
			}
		}

		public static void RegisterInstance<T>(T instance, bool reRegister = false)
		{
			if (reRegister || !UnityContainer.IsRegistered<T>())
			{
				UnityContainer.RegisterInstance(typeof(T), instance);
			}
		}

		public static void RegisterInstance(Type type, string name, object instance, bool reRegister = false)
		{
			if (reRegister || !UnityContainer.IsRegistered(type))
			{
				UnityContainer.RegisterInstance(type, name, instance);
			}
		}

		public static void RegisterInstance<T>(T instance, string name, bool reRegister = false)
		{
			if (reRegister || !UnityContainer.IsRegistered<T>())
			{
				UnityContainer.RegisterInstance(typeof(T), name, instance);
			}
		}

		public static void RegisterType<TFrom, TTo>(bool reRegister = false)
		{
			if (reRegister || !UnityContainer.IsRegistered<TFrom>())
			{
				UnityContainer.RegisterType(typeof(TFrom), typeof(TTo));
			}
		}

		public static void RegisterType(Type tFrom, Type tTo, bool reRegister = false)
		{
			if (reRegister || !UnityContainer.IsRegistered(tFrom))
			{
				UnityContainer.RegisterType(tFrom, tTo);
			}
		}

		public static void RegisterTypeAsSingleton<TFrom, TTo>(string name = null, bool reRegister = false)
		{
			if (reRegister || !IsRegistered(typeof(TFrom)))
			{
				UnityContainer.RegisterType(typeof(TFrom), typeof(TTo), new ContainerControlledLifetimeManager());
			}
		}

		public static void RegisterTypeAsSingleton(Type fromType, Type toType, string name = null,
			bool reRegister = false)
		{
			if (reRegister || !IsRegistered(fromType))
			{
				UnityContainer.RegisterType(fromType, toType, new ContainerControlledLifetimeManager());
			}
		}

		public static void RegisterTypes<T>()
		{
			UnityContainer.RegisterTypes(
				AllClasses.FromLoadedAssemblies().Where(type => typeof(T).IsAssignableFrom(type)),
				WithMappings.FromAllInterfaces,
				WithName.TypeName,
				WithLifetime.Transient);
		}

		public static void Configure(Assembly executingAssembly)
		{
			if (_isConfigured)
			{
				return;
			}

			var referencedAssemblyNames = executingAssembly.GetReferencedAssemblies();
			var allAssemblies = new[] { executingAssembly }.Concat(referencedAssemblyNames.Select(Assembly.Load)).ToArray();
			var allTypes = AllClasses.FromAssemblies(allAssemblies).ToArray();

			// Need to overwriteExistingMappings to avoid a conflict for type System.Collections.Generic.ICollection`1[T]
			// between System.Collections.ObjectModel.Collection`1[T] and DevExpress.Web.ASPxClasses.Collection`1[T]
			UnityContainer.RegisterTypes(allTypes,
				WithMappings.FromMatchingInterface,
				WithName.Default,
				overwriteExistingMappings: true);

			// Can use the following if configuring from config file
			const string unitySectionName = "unity";
			var section = (UnityConfigurationSection)ConfigurationManager.GetSection(unitySectionName);
			if (section != null)
			{
				try
				{
					section.Configure(UnityContainer);
				}
				catch (Exception exception)
				{
					Debug.WriteLine($"ObjectFactory.Configure threw exception: {exception}");
					throw;
				}
			}

			_isConfigured = true;
		}

		public static void BuildUp<T>(T existing)
		{
			UnityContainer.BuildUp(existing);
		}
	}
}