using ExpertSender.Common.QueryBuilder;
using ExpertSender.DataModel.Dao;
using Multi.WebDaoTests.Core;
using EsAppContext = Multi.WebDaoTests.Mocks.EsAppContext;

namespace Multi.WebDaoTests.TestsDao
{
    internal class MessageContentDaoTests : Tester
    {
        private const int CurrentUnitId = 1;

        public MessageContentDaoTests()
            : base(new EsAppContext { CurrentServiceId = CurrentUnitId })
        { }

        public override void Start()
        {
            QueryTest();
            QueryTest1();
        }

        public void QueryTest()
        {
            var dao = Container.GetInstance<IMessageContentDao>();
                
            var selectBuilder = new SelectBuilder("Subject");
            selectBuilder.From("MessageContent").Where(new { MessageId = 111, IsDeleted = false });
            var result = dao.Use(CurrentUnitId).SelectOne(selectBuilder);
        }

        public void QueryTest1()
        {
            var dao = Container.GetInstance<IMessageContentDao>();
                
            var selectBuilder = new SelectBuilder("Id, StringValue, IntValue, DateTimeValue");
            selectBuilder.From("SubscriberProperty").Where(new { PropertyId = 5, SubscriberId = 25 });
            var result = dao.Use(CurrentUnitId).SelectOne(selectBuilder);
        }
    }
}
