using Castle.Windsor;

namespace Jarvis.JsonStore.Host.Support
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
