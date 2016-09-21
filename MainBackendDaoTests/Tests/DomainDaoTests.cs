using ExpertSender.DataModel.BackendDao;
using MainBackendDaoTests.Core;
using MainBackendDaoTests.Mocks;

namespace MainBackendDaoTests.Tests
{
    internal class DomainDaoTests : Tester
    {
        private const int CurrentUnitId = 1;

        public DomainDaoTests()
            : base(new EsAppContext
            {
                CurrentServiceId = CurrentUnitId
            })
        { }

        public override void Start()
        {
            ClearDomainFamiliesInDwTest1();
            ClearDomainFamiliesInDwTest2();
            ClearDomainFamiliesInDwTest3();
            ClearDomainFamiliesInDwTest4();
        }

        public void ClearDomainFamiliesInDwTest1()
        {
            var dao = Container.GetInstance<IDomainDao>();
            dao.Use(CurrentUnitId);

            dao.ClearDomainFamiliesInDw(CurrentUnitId, 0, 1000);
        }

        public void ClearDomainFamiliesInDwTest2()
        {
            var dao = Container.GetInstance<IDomainDao>();
            dao.Use(CurrentUnitId);

            dao.ClearDomainFamiliesInDw(CurrentUnitId, 0, 0);
        }

        public void ClearDomainFamiliesInDwTest3()
        {
            var dao = Container.GetInstance<IDomainDao>();
            dao.Use(CurrentUnitId);

            dao.ClearDomainFamiliesInDw(CurrentUnitId, 4, 21);
        }

        public void ClearDomainFamiliesInDwTest4()
        {
            var dao = Container.GetInstance<IDomainDao>();
            dao.Use(CurrentUnitId);

            dao.ClearDomainFamiliesInDw(CurrentUnitId, 4, 1000);
        }
    }
}
