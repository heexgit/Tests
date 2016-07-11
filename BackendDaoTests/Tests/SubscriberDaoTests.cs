using BackendDaoTests.Core;
using ExpertSender.DataModel.BackendDao;
using BackendDaoTests.Mocks;

namespace BackendDaoTests.Tests
{
    internal class SubscriberDaoTests : Tester
    {
        private const int CurrentUnitId = 1;

        public SubscriberDaoTests()
            : base(new EsAppContext
            {
                CurrentServiceId = CurrentUnitId
            })
        { }

        public override void Start()
        {
            GetSubscriptionTest();
        }

        public void GetSubscriptionTest()
        {
            var dao = Container.GetInstance<ISubscriberDao>().Use(CurrentUnitId);
            var yes = dao.GetSubscription(15, 1);
            var both = dao.GetSubscription(15, 2);
            var subscriber = dao.GetSubscription(15, 0);
            var list = dao.GetSubscription(0, 1);
            var none = dao.GetSubscription(0, 0);
        }
    }
}
