using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ExpertSender.Backend.Host;
using ExpertSender.Common;
using ExpertSender.Common.Cache;
using ExpertSender.Common.Dao;
using MainBackendDaoTests.Mocks;
using StructureMap;

namespace MainBackendDaoTests.Core
{
    /// <summary>
    /// Konfiguracja IoC dla typów infrastrukturalnych
    /// </summary>
    public class InfrastructureRegistry : Registry
    {
        public InfrastructureRegistry()
        {
            For<IAppContext>().Use<EsAppContext>();
            For<IDbConnection>()
                //.HybridHttpOrThreadLocalScoped()
                .Use<SqlConnection>()
                .SelectConstructor(() => new SqlConnection(string.Empty))
                .Ctor<string>().Is(ConfigurationManager.AppSettings["DbConnectionString"]);
            For<IDbTransactionProvider>()
                //.HybridHttpOrThreadLocalScoped()
                .Use<DbTransactionProvider>();
            //For<IServiceHost>().Use<ServiceHost>();
            For<ICacheProvider>().Use<AppFabricCacheProvider>().SetProperty(c => c.SetConfig(new AppFabricCacheConfig()));
            For<IMemoryCacheProvider>().Use<MemoryCacheSingleThreadProvider>();
            //For<QueueFactory>()
            //    .Singleton()
            //    .Use<QueueFactory>();
            //Scan(scan =>
            //{
            //    scan.AssemblyContainingType<IAzureSubService>();
            //    scan.AddAllTypesOf<IAzureSubService>();
            //});
            //AddRegistry<DaoRegistry>();
        }
    }
}
