using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json.ObjectService.Host.Support
{
    public static class ContainerWrapper
    {
        public static IWindsorContainer Container { get; private set; }

        public static void Init(IWindsorContainer container)
        {
            Container = container;
        }
    }
}
