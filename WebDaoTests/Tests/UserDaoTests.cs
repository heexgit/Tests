using ExpertSender.Common;
using ExpertSender.DataModel.Dao;
using WebDaoTests.Core;

namespace WebDaoTests.Tests
{
    class UserDaoTests : Tester
    {
        public override void Start()
        {
            var app = Container.GetInstance<IEsAppContext>();
            app.CurrentServiceId = 1;

            GetAllTest();
        }

        public void GetAllTest()
        {
            var dao = Container.GetInstance<IUserDao>();
            var all = dao.GetAll();
        }
    }
}
