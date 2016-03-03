using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Castle.Windsor;

namespace Jarvis.JsonStore.Host.Support
{
    public class WindsorResolver : IDependencyResolver
    {
        private readonly IWindsorContainer _container;


        public WindsorResolver(IWindsorContainer container)
        {
            _container = container;
        }

        IWindsorContainer SelectContainer()
        {
             return _container;
        }

        public IDependencyScope BeginScope()
        {
            return new WindsorDependencyScope(SelectContainer());
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public object GetService(Type serviceType)
        {
            var container = SelectContainer();

            if (!container.Kernel.HasComponent(serviceType))
                return null;

            return container.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var container = SelectContainer();

            if (!container.Kernel.HasComponent(serviceType))
                return new object[0];

            return container.ResolveAll(serviceType).Cast<object>();
        }
    }
}
