using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.Facilities.Startable;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Services.Logging.Log4netIntegration;
using Castle.Windsor;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json.ObjectService.Host.Support
{
    class BootStrapper
    {
        IWindsorContainer _container;
        ILogger _logger;
        private IDisposable _webApplication;

        internal bool Start()
        {
            _container = new WindsorContainer();

            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));
            _container.Kernel.Resolver.AddSubResolver(new ArrayResolver(_container.Kernel, true));


            _container.AddFacility<LoggingFacility>(
                f => f.LogUsing(new ExtendedLog4netFactory("log4net.config")));

            _container.AddFacility<StartableFacility>();
            _container.AddFacility<TypedFactoryFacility>();

            ContainerWrapper.Init(_container);

            _logger = _container.Resolve<ILoggerFactory>().Create(GetType());

            var installers = new List<IWindsorInstaller>()
            {
                new ApiInstaller(),
            };


            var options = new StartOptions();

            _logger.InfoFormat("Binding to @ {0}", "http://+:40000");
            options.Urls.Add("http://+:40000");

            _webApplication = WebApp.Start<WebApplication>(options);

            return true;
        }

        internal bool Stop()
        {
            return true;
        }
    }
}
