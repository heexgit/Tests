using ExpertSender.Common.Helpers;
using ExpertSender.DAL.Core;
using ExpertSender.DAL.Registry;
using NHibernate;
using StructureMap;

namespace WebDaoTests.Core
{
    abstract class Tester
    {
        protected IContainer Container { get; private set; }
        private ISessionFactory _sessionFactory;

        protected Tester()
        {
            PrepareContainer();
            StaticsProvider.Container = Container;

            PrepareNHibernate();
        }

        private void PrepareNHibernate()
        {
            var config = new NHibernate.Cfg.Configuration().Configure();

            var connstring = config.Properties[NHibernate.Cfg.Environment.ConnectionString];
            ReflectionHelper.SetValueOfPrivateStaticField(typeof(DynamicConnectionProvider), "_connectionString", connstring);

            _sessionFactory = config.BuildSessionFactory();
        }

        private void PrepareContainer()
        {
            Container = new Container(c =>
            {
                c.AddRegistry<InfrastructureRegistry>();
                c.AddRegistry<DaoRegistry>();

                c.Policies.FillAllPropertiesOfType<IContainer>();
                c.Policies.FillAllPropertiesOfType<ISession>().Use(context => GetContextSession(context));
            }
            );
        }

        /// <summary>
        /// Nhibernate w ES2 nie uzywa transakcji (nie ma automatycznego rozpoczynania trnsakcji przy otwieraniu sesji). 
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
