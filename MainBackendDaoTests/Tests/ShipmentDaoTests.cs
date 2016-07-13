using System.Collections.Generic;
using ExpertSender.DataModel.BackendDao;
using MainBackendDaoTests.Core;
using MainBackendDaoTests.Mocks;

namespace MainBackendDaoTests.Tests
{
    internal class ShipmentDaoTests : Tester
    {
        private const int CurrentUnitId = 1;

        public ShipmentDaoTests()
            : base(new EsAppContext
            {
                CurrentServiceId = CurrentUnitId
            })
        { }

        public override void Start()
        {
            SaveAllowedSubscribersTest();
        }

        public void SaveAllowedSubscribersTest()
        {
            var dao = Container.GetInstance<IShipmentDao>();
            dao.Use(CurrentUnitId);

            var list = new List<int[]>
            {
                new[] {1,2,3}
            };

            dao.SaveAllowedSubscribers(list);
        }
    }
}
