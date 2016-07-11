using BackendDaoTests.Core;
using ExpertSender.Common.QueryBuilder;
using ExpertSender.DataModel.BackendDao;
using BackendDaoTests.Mocks;

namespace BackendDaoTests.Tests
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
            SelectTest();
            InsertTest();
        }

        public void SelectTest()
        {
            var dao = Container.GetInstance<IMessageContentDao>().Use(CurrentServiceId);
                
            var selectBuilder = new SelectBuilder("Subject");
            selectBuilder.From("MessageContent").Where(new { MessageId = 110, IsDeleted = false });
            var result = dao.SelectOne(selectBuilder);

            var subject = result.Subject;
        }

        public void InsertTest()
        {
            var dao = Container.GetInstance<IMessageContentDao>().Use(CurrentServiceId);
                
            var insertBuilder = new InsertBuilder("MessageContent").Values(new ValuesBuilder(new
            {
                MessageId = 110,
                Subject = "testowy Subject",
                IsDeleted = false,
                IsSubjectDynamic = false,
                IsFromNameDynamic = false,
                IsFromEmailDynamic = false,
                IsReplyNameDynamic = false,
                IsReplyEmailDynamic = false,
                IsHtmlDynamic = false,
                IsPlainDynamic = false,
                IsSplitTestWinner = false,
                UsesRemoteContent = false,
                Opens = 0,
                Clicks = 0,
                Goals = 0,
                GoalsValue = 0,
            }));
            int idx;
            var result = dao.Insert(insertBuilder, out idx);
        }
    }
}
