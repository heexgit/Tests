using ExpertSender.DataModel.Dao;
using WebDaoTests.Core;

namespace WebDaoTests.Tests
{
    class UserDaoTests : Tester
    {
        public override void Start()
        {
            GetAllTest();
        }

        public void GetAllTest()
        {
            var dao = Container.GetInstance<IUserDao>();
            var all = dao.GetAll();
        }
    }
}
