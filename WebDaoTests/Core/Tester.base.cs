using System;
using ExpertSender.Common;
using ExpertSender.Common.Helpers;
using ExpertSender.DAL.Core;
using ExpertSender.DAL.Registry;
using NHibernate;
using StructureMap;

namespace WebDaoTests.Core
{
    public abstract class Tester : IDisposable
    {
        protected IContainer Container { get; private set; }
        private ISessionFactory _sessionFactory;
        private ITransaction _transaction;

        protected Tester()
        {
            PrepareContainer();

            PrepareNHibernate();
        }

        protected Tester(IEsAppContext appContext)
        {
            PrepareContainer();

            Container.Inject(appContext);

            PrepareNHibernate();
        }

        public void Dispose()
        {
            _transaction.Rollback();
        }

        private void PrepareNHibernate()
        {
            var config = new NHibernate.Cfg.Configuration().Configure();

            // zmieniamy ConnectionString na bazę wybranego unita
            var connstring = config.Properties[NHibernate.Cfg.Environment.ConnectionString];
            ReflectionHelper.SetValueOfPrivateStaticField(typeof(DynamicConnectionProvider), "_connectionString", connstring);

            _sessionFactory = config.BuildSessionFactory();

            _transaction = Container.GetInstance<ISession>().BeginTransaction();
        }

        private void PrepareContainer()
        {
            Container = (IContainer)ReflectionHelper.GetValueOfPrivateStaticField(typeof(StaticsProvider), "_container");
            if (Container == null)
            {
                Container = new Container(c =>
                {
                    c.AddRegistry<InfrastructureRegistry>();
                    c.AddRegistry<DaoRegistry>();

                    c.Policies.FillAllPropertiesOfType<IContainer>();
                    c.Policies.FillAllPropertiesOfType<ISession>()
                        .Singleton() // zapewniamy jedną instancję ISession dla wszystkich wywołań
                        .Use(context => GetContextSession(context));
                });
                StaticsProvider.Container = Container;
            }
        }

        /// <summary>
        /// nHibernate w ES2 nie uzywa transakcji (nie ma automatycznego rozpoczynania transakcji przy otwieraniu sesji). 
        /// Jest to sprzeczne z dobrymi praktykami ale niesamowicie upraszcza przelaczanie sie pomiedzy unitami.
        /// Nie musimy troszczyc sie o komitowanie / rollbackowanie transakcji. Po prostu zmieniamy connection string w locie. 
        /// </summary>
        /// <param name="structureMapContext"></param>
        /// <returns></returns>
        public ISession GetContextSession(IContext structureMapContext)
        {
            return _sessionFactory.OpenSession();
        }

        public abstract void Start();
    }
}
