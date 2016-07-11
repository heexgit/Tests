using System.Data;
using System.Data.SqlClient;
using ExpertSender.Backend.DAL;
using ExpertSender.Backend.Host;
using ExpertSender.Common;
using ExpertSender.Common.Cache;
using ExpertSender.Common.Dao;
using ExpertSender.Common.Helpers;
using ExpertSender.DataModel.CommonDao;
using ExpertSender.Lib.Cache;
using StructureMap;

namespace BackendDaoTests.Core
{
    /// <summary>
    /// Konfiguracja IoC dla typów infrastrukturalnych
    /// </summary>
    public class InfrastructureRegistry : Registry
    {
        public InfrastructureRegistry()
        {
            For<IEsAppContext>().Use<Mocks.EsAppContext>();
            For<IDbConnection>()
                //.HybridHttpOrThreadLocalScoped()
                .Use<SqlConnection>()
                .SelectConstructor(() => new SqlConnection(string.Empty))
                .Ctor<string>().Is(AppConfig.ConnectionString);
            For<IDbTransactionProvider>()
                //.HybridHttpOrThreadLocalScoped()
                .Use<DbTransactionProvider>();
            //For<IServiceHost>().Use<ServiceHost>();
            For<ICacheProvider>().Use<AppFabricCacheProvider>().SetProperty(c => c.SetConfig(new AppFabricCacheConfig()));
            For<IMemoryCacheProvider>().Use<MemoryCacheSingleThreadProvider>();
            For<IMemoryStaticCacheProvider>().Use<MemoryCacheProvider>();
            For<IAssemblyAuditDao>().Use<AssemblyAuditDao>();
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
