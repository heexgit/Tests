using System.Collections.Generic;
using ExpertSender.Common;
using ExpertSender.Common.Cache;
using ExpertSender.Common.Helpers;
using ExpertSender.Lib.Cache;
using StructureMap;

namespace WebDaoTests.Core
{
    /// <summary>
    /// Konfiguracja IoC dla typów infrastrukturalnych
    /// </summary>
    public class InfrastructureRegistry : Registry
    {
        public InfrastructureRegistry()
        {
            //For<ISessionFactory>().Singleton().Use(ctx =>
            //    new Configuration()
            //        .Configure()
            //        .SetInterceptor(ctx.GetInstance<EntityStructureMapInterceptor>())
            //        .BuildSessionFactory()
            //);

            //For<ISession>().HttpContextScoped().Use(context => IoC.GetContextSession(context));

            //For<IControllerActivator>().Use<StructureMapControllerActivator>();

            //For<DefaultModelBinder>()
            //    .Use<SmartBinder>()
            //    .EnumerableOf<IFilteredModelBinder>()
            //    .Contains(y =>
            //    {
            //        y.Type<AuthorisedUserViewModelBinder>();
            //    }
            //);

            //For<IFormsAuthentication>().Use<FormsAuthenticationService>();

            //For<ModelMetadataProvider>().Use<EsDataAnnotationsModelMetadataProvider>();

            For<IAppContext>().Use<Mocks.AppContext>();
            //For<IAppContext>().HttpContextScoped().Use<AppContext>();

            //For<ITempDataProvider>().Use<CacheTempDataProvider>();
            For<ICacheProvider>().Use<AppFabricCacheProvider>().SetProperty(c => c.SetConfig(new AppFabricCacheConfig()));
            For<IMemoryCacheProvider>().Use<MemoryCacheProvider>();
            For<IMemoryStaticCacheProvider>().Use<MemoryCacheProvider>();

            // obiekt czytający dane z bazy MaxMind jest kosztowny, więc trzymamy go w pamięci
            //For<DatabaseReader>().Singleton().Use(ctx => new DatabaseReader(MachineConfig.GetSetting(ColocationSetting.MaxMindDB, true), 0));

            For<DateRange>().Use(ctx => new DateRange());

            // fix bindowania patrz ED-5787
            For<IList<int>>().AlwaysUnique().Use(ctx => new List<int>());
            For<IList<string>>().AlwaysUnique().Use(ctx => new List<string>());
            For<IEnumerable<int>>().AlwaysUnique().Use(ctx => new List<int>());
            For<IEnumerable<string>>().AlwaysUnique().Use(ctx => new List<string>());

            //ModelBinders.Binders.Add(typeof(double), new DoubleViewModelBinder());
        }
    }
}
