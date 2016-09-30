using ExpertSender.DataModel.Dao;
using WebDaoTests.Core;
using EsAppContext = WebDaoTests.Mocks.EsAppContext;

namespace WebDaoTests.TestsDao
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
            GetTest();
        }

        public void GetTest()
        {
            var dao = Container.GetInstance<ISubscriberDao>();
            var one = dao.Get(20);
        }
    }
}
