using ExpertSender.Common.QueryBuilder;
using ExpertSender.DataModel.Dao;
using WebDaoTests.Core;
using EsAppContext = WebDaoTests.Mocks.EsAppContext;

namespace WebDaoTests.Tests
{
    internal class MessageContentDaoTests : Tester
    {
        private const int CurrentServiceId = 1;

        public MessageContentDaoTests()
            : base(new EsAppContext
            {
                CurrentServiceId = CurrentServiceId
            })
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
            var result = dao.Use(CurrentServiceId).SelectOne(selectBuilder);
        }

        public void QueryTest1()
        {
            var dao = Container.GetInstance<IMessageContentDao>();
                
            var selectBuilder = new SelectBuilder("Id, StringValue, IntValue, DateTimeValue");
            selectBuilder.From("SubscriberProperty").Where(new { PropertyId = 5, SubscriberId = 25 });
            var result = dao.Use(CurrentServiceId).SelectOne(selectBuilder);
        }
    }
}
