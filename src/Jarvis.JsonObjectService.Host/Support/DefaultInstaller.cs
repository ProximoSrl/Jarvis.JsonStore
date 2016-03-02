using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Jarvis.JsonObjectService.Core.Storage;
using Jarvis.JsonObjectService.Core.Support;
using MongoDB.Driver;

namespace Json.ObjectService.Host.Support
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
                    .Instance(database));
        }
    }
}
