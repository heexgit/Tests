using ExpertSender.Common;
using ExpertSender.DataModel.Dao;
using WebDaoTests.Core;

namespace WebDaoTests.Tests
{
    internal class SubscriberDaoTests : Tester
    {
        public override void Start()
        {
            var app = Container.GetInstance<IEsAppContext>();
            app.CurrentServiceId = 1;

            GetTest();
        }

        public void GetTest()
        {
            var dao = Container.GetInstance<ISubscriberDao>();
            var one = dao.Get(20);
        }
    }
}
