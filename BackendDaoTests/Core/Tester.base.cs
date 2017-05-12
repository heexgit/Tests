using System;
using System.Data;
using ExpertSender.Backend.DAL.Registry;
using ExpertSender.Common;
using ExpertSender.Common.Dao;
using ExpertSender.Common.Helpers;
using StructureMap;

namespace BackendDaoTests.Core
{
    public abstract class Tester : IDisposable
    {
        protected IContainer Container { get; private set; }

        private readonly IDbConnection _connection;
        private readonly IDbTransactionProvider _transactionProvider;

        protected Tester()
        {
            PrepareContainer();
        }

        protected Tester(IEsAppContext appContext)
        {
            PrepareContainer();

            Container.Inject(appContext);

            _connection = Container.GetInstance<IDbConnection>();
            _connection.Open();
            _transactionProvider = Container.GetInstance<IDbTransactionProvider>();
            _transactionProvider.BeginTransaction();
        }

        public void Dispose()
        {
            _transactionProvider.RollbackTransaction();
            _connection.Close();
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
                });
                StaticsProvider.Container = Container;
            }
        }

        public abstract void Start();
    }
}
