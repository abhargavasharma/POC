using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using TAL.QuoteAndApply.Infrastructure.Crypto;
using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.ServiceLayer.Ioc;
using TAL.QuoteAndApply.Web.Shared.Configuration;
using TAL.QuoteAndApply.Web.Shared.Converters;
using TAL.QuoteAndApply.Web.Shared.Cookie;
using TAL.QuoteAndApply.Web.Shared.Loggers;
using TAL.QuoteAndApply.Web.Shared.Services;
using TAL.QuoteAndApply.Web.Shared.Workflow;

namespace TAL.QuoteAndApply.Web.Shared.Ioc
{
    public class BaseIocConfig
    {
        public IContainer RegisterForWeb(HttpConfiguration config, Action<ContainerBuilder> registerInteralAction)
        {
            var builder = SetupContainerBuilder(registerInteralAction);

            var container = CreateContainer(builder);
            RegisterContainerWithWebApi(config, container, builder);
            RegisterContainerWithMvc(container);
            RegisterObservers(config);

            return container;
        }

        public IContainer CreateContainer(ContainerBuilder containerBuilder)
        {
            var container = containerBuilder.Build();
            return container;
        }

        public void RegisterContainerWithWebApi(HttpConfiguration config, IContainer container, ContainerBuilder builder)
        {
            var webApiResolver = new AutofacWebApiDependencyResolver(container);            
            config.DependencyResolver = webApiResolver;
        }

        public void RegisterObservers(HttpConfiguration config)
        {
            var eventRegistrations = (IEnumerable<IEventsRegistration>)config.DependencyResolver.GetServices(typeof(IEventsRegistration));
            foreach (var eventReg in eventRegistrations)
            {
                eventReg.RegisterAllObserversWithSubjects();
            }
        }

        public void RegisterContainerWithMvc(IContainer container)
        {
            var mvcResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(mvcResolver);
        }

        public ContainerBuilder SetupContainerBuilder(Action<ContainerBuilder> registerInteralAction)
        {
            var builder = new ContainerBuilder();

            //register modules from service layer and dependant modules
            RegisterModuleMappings(builder, typeof(ServiceLayer.Ioc.RegistrationModule), "TAL.QuoteAndApply.");

            RegisterCommomInternalModules(builder);
            registerInteralAction(builder);
            
            //Registers HttpContextBase, HttpRequestBase, HttpReponseBase, HttpServerUtilityBase, HttpSessionStateBase
            builder.RegisterModule(new AutofacWebTypesModule());

            return builder;
        }

        private void RegisterCommomInternalModules(ContainerBuilder builder)
        {
            builder.RegisterModule<Log4NetInjectionModule>();
            builder.RegisterFilterProvider();
            builder.RegisterType<WebApiExceptionLogger>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<MvcExceptionLoggingAttribute>()
                .AsExceptionFilterFor<Controller>()
                .InstancePerLifetimeScope()
                .PropertiesAutowired();

            builder.RegisterType<SecurityService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<CookieService>().As<ICookieService>().InstancePerLifetimeScope();
            builder.RegisterType<CookiePolicyInitialisationMetadataSessionStorageService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<ErrorsAndWarningsConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<WebConfiguration>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<RedirectionActionProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<ApplicationStepContextService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<ApplicationStepWorkFlowProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
        }
         

        public void RegisterModuleMappings(ContainerBuilder builder, Type type, string assemblyStartsWith)
        {
            var resolver = new IocRegistrationResolver();
            resolver.LoadAssembly(Assembly.GetAssembly(type), assemblyStartsWith);
            var mappings = resolver.GetMappings();

            foreach (var typeMapItem in mappings.Where(m => m.WithMatchInterface && !m.IsSingletonScope))
            {
                builder.RegisterType(typeMapItem.ResolveToType).InstancePerLifetimeScope().AsImplementedInterfaces();
            }

            foreach (var typeMapItem in mappings.Where(m => !m.WithMatchInterface && !m.IsSingletonScope))
            {
                builder.RegisterType(typeMapItem.ResolveToType).InstancePerLifetimeScope().As(typeMapItem.RegisteredType);
            }

            foreach (var typeMapItem in mappings.Where(m => m.WithMatchInterface && m.IsSingletonScope))
            {
                builder.RegisterType(typeMapItem.ResolveToType).SingleInstance().AsImplementedInterfaces();
            }

            foreach (var typeMapItem in mappings.Where(m => !m.WithMatchInterface && m.IsSingletonScope))
            {
                builder.RegisterType(typeMapItem.ResolveToType).SingleInstance().As(typeMapItem.RegisteredType);
            }
        }

    }

}
