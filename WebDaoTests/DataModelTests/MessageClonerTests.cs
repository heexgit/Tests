using System.Linq;
using ExpertSender.DataModel;
using ExpertSender.DataModel.Abstract;
using ExpertSender.DataModel.Dao;
using ExpertSender.DataModel.Enums;
using ExpertSender.DataModel.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDaoTests.Core;
using EsAppContext = WebDaoTests.Mocks.EsAppContext;

namespace WebDaoTests.DataModelTests
{
    [TestClass]
    public class MessageClonerTests : Tester
    {
        private const int CurrentUnitId = 1;

        public MessageClonerTests()
            : base(new EsAppContext {CurrentServiceId = CurrentUnitId})
        { }

        public override void Start()
        { }

        [TestMethod]
        public void NewsletterTest()
        {
            var newsletterDao = Container.GetInstance<INewsletterDao>();
            newsletterDao.Use(CurrentUnitId);

            var sourceMessage = newsletterDao.Get(125209);

            var newMessage = GetMessageCloner<Newsletter>()
                .GetClone(sourceMessage);
            
            Assert.AreEqual(MessageType.Newsletter, newMessage.Type);
            Assert.AreEqual(sourceMessage.TimeZone, newMessage.TimeZone);
            Assert.AreEqual(sourceMessage.IsSplitTest, newMessage.IsSplitTest);
            Assert.AreEqual(sourceMessage.IsActive, newMessage.IsActive);
            Assert.AreEqual(sourceMessage.IsGoogleAnalyticsOn, newMessage.IsGoogleAnalyticsOn);

            // ShipmentStatus.Queued jest ustawiany w kontruktorze Newslettera
            Assert.AreEqual(ShipmentStatus.Queued, newMessage.Status);
            Assert.AreEqual(1, ((INewsletterMessage)newMessage).Status);

            Assert.IsNull(newMessage.GenerateListDate);
            Assert.IsNull(newMessage.FinishDate);

            foreach (var newContent in newMessage.Contents)
            {
                //var sourceContent = sourceMessage.Contents.FirstOrDefault(c => c.Subject == newContent.Subject);
                Assert.IsNull(newContent.SentDate);
                Assert.IsFalse(newContent.GoogleAnalyticsTags.Any());
                Assert.IsFalse(newContent.IsDeleted);
            }
        }

        private MessageCloner<TMessage> GetMessageCloner<TMessage>()
            where TMessage : Message, new()
        {
            var cloner = new MessageCloner<TMessage>(MessageClonerFlags.TypeWholeExisting)
                .SetInitializeAction(tm =>
                {
                    tm.IsDraft = true;
                    var sourceType = typeof (TMessage);
                    if (sourceType == typeof (Autoresponder) || sourceType == typeof (Transactional) || sourceType == typeof (WorkflowMessage))
                    {
                        tm.IsActive = false;
                    }
                    tm.IsDeleted = false;
                    tm.Container = Container;
                })
                .SetMessageContentCloner(new MessageContentCloner(MessageContentClonerFlags.TypeWholeExisting));
            return cloner;
        }
    }
}