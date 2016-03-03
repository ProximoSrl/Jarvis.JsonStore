using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Jarvis.JsonStore.Core.Projections;
using Jarvis.JsonStore.Core.Storage;
using Jarvis.JsonStore.Core.Support;
using MongoDB.Driver;

namespace Jarvis.JsonStore.Host.Support
{
    public class DefaultInstaller : IWindsorInstaller
    {
        JsonObjectServiceConfiguration _config;

        public DefaultInstaller(JsonObjectServiceConfiguration config)
        {
            _config = config;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var databaseUrl = new MongoUrl(_config.MongoDbConnection);
            var database = new MongoClient(databaseUrl).GetDatabase(databaseUrl.DatabaseName);
            container.Register(
                Component
                    .For<IObjectStore>()
                    .ImplementedBy<MongoObjectStore>(),
                Component
                    .For<IMongoDatabase>()
                    .Instance(database),
                Component
                    .For<PayloadProjection>()
                    .ImplementedBy<PayloadProjection>());
        }
    }
}
