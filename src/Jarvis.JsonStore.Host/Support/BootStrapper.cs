using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.Facilities.Startable;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Services.Logging.Log4netIntegration;
using Castle.Windsor;
using Jarvis.JsonStore.Core.Support;
using Microsoft.Owin.Hosting;

namespace Jarvis.JsonStore.Host.Support
{
    class BootStrapper
    {
        IWindsorContainer _container;
        ILogger _logger;
        private IDisposable _webApplication;
        LocalJsonObjectServiceConfiguration _configuration;

        internal bool Start()
        {
            try
            {


                _configuration = new LocalJsonObjectServiceConfiguration();
                _container = new WindsorContainer();
                _container.Register(Component.For<JsonObjectServiceConfiguration>().Instance(_configuration));

                _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));
                _container.Kernel.Resolver.AddSubResolver(new ArrayResolver(_container.Kernel, true));


                _container.AddFacility<LoggingFacility>(
                    f => f.LogUsing(new ExtendedLog4netFactory("log4net.config")));
                _logger = _container.Resolve<ILoggerFactory>().Create(GetType());

                _container.AddFacility<StartableFacility>();
                _container.AddFacility<TypedFactoryFacility>();

                ContainerWrapper.Init(_container);

                var installers = new List<IWindsorInstaller>()
                {
                    new ApiInstaller(),
                    new DefaultInstaller(_configuration),
                };

                _container.Install(installers.ToArray());

                var options = new StartOptions();

                _logger.InfoFormat("Binding to @ {0}", "http://+:40000");
                options.Urls.Add("http://+:40000");

                _webApplication = WebApp.Start<WebApplication>(options);

                return true;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.Error("Error during bootstrap: " + ex.Message, ex);
                }
                throw;
            }
        }

        internal bool Stop()
        {
            _container.Dispose();
            return true;
        }
    }
}
