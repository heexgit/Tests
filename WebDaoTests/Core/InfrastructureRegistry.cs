﻿using System.Collections.Generic;
using ExpertSender.Common;
using ExpertSender.Common.Cache;
using ExpertSender.Common.Helpers;
using ExpertSender.DataModel.Distributed.Enums;
using ExpertSender.DataModel.Helpers;
using ExpertSender.Lib.Cache;
using MaxMind.GeoIP2;
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
            For<IEsAppContext>().Use<Mocks.EsAppContext>();

            //For<ITempDataProvider>().Use<CacheTempDataProvider>();
            For<ICacheProvider>().Use<AppFabricCacheProvider>().SetProperty(c => c.SetConfig(new AppFabricCacheConfig()));
            For<IMemoryCacheProvider>().Use<MemoryCacheProvider>();
            For<IMemoryStaticCacheProvider>().Use<MemoryCacheProvider>();
            For(typeof(ICacheProxy<>)).Use(typeof(SpecializedCacheProxy<>));

            // obiekt czytający dane z bazy MaxMind jest kosztowny, więc trzymamy go w pamięci
            For<DatabaseReader>().Singleton().Use(ctx => new DatabaseReader(MachineConfig.GetSetting(ColocationSetting.MaxMindDB, true), 0));

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
