using Castle.Core.Logging;
using Jarvis.DocumentStore.Host.Support;
using Metrics;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using Owin.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Json.ObjectService.Host.Support
{
    class WebApplication
    {


        public void Configuration(IAppBuilder application)
        {
            ConfigureApi(application);
            ConfigureAdmin(application);

            Metric
                .Config
                .WithOwin(middleware => application.Use(middleware), config => config
                .WithRequestMetricsConfig(c => c.WithAllOwinMetrics())
                .WithMetricsEndpoint()
            );
        }


        void ConfigureAdmin(IAppBuilder application)
        {
            var appFolder = FindAppRoot();

            var fileSystem = new PhysicalFileSystem(appFolder);

            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                FileSystem = fileSystem,
                EnableDefaultFiles = true
            };

            application.UseFileServer(options);
        }

        static string FindAppRoot()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory
                .ToLowerInvariant()
                .Split(System.IO.Path.DirectorySeparatorChar)
                .ToList();

            while (true)
            {
                var last = root.Last();
                if (last == String.Empty || last == "debug" || last == "release" || last == "bin")
                {
                    root.RemoveAt(root.Count - 1);
                    continue;
                }

                break;
            }

            root.Add("app");

            var appFolder = String.Join("" + System.IO.Path.DirectorySeparatorChar, root);
            return appFolder;
        }

        static void ConfigureApi(IAppBuilder application)
        {
            var config = new HttpConfiguration
            {
                DependencyResolver = new WindsorResolver(
                    ContainerWrapper.Container
                )
            };

            config.MapHttpAttributeRoutes();
            var loggerFactory = ContainerWrapper.Container.Resolve<IExtendedLoggerFactory>();


            config.Services.Add(
                typeof(IExceptionLogger),
                new Log4NetExceptionLogger(ContainerWrapper.Container.Resolve<ILoggerFactory>())
            );

            var factory = ContainerWrapper.Container.Resolve<IExtendedLoggerFactory>();

            application.UseWebApi(config);
        }
    }
}
