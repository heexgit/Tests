using System;
using ExpertSender.DataModel;
using ExpertSender.DataModel.BackendDao;
using ExpertSender.DataModel.Enums;
using MainBackendDaoTests.Core;
using MainBackendDaoTests.Mocks;

namespace MainBackendDaoTests.Tests
{
    internal class TriggerDaoTests : Tester
    {
        private const int CurrentUnitId = 1;

        public TriggerDaoTests()
            : base(new EsAppContext
            {
                CurrentServiceId = CurrentUnitId
            })
        { }

        public override void Start()
        {
            IsTriggerSubscriberTest();
            //CreateTriggerSubscribersTest();
        }

        public void IsTriggerSubscriberTest()
        {
            var dao = Container.GetInstance<ITriggerDao>();
            dao.Use(CurrentUnitId);
            var yes = dao.IsTriggerSubscriber(123552, 4238630);
            var both = dao.IsTriggerSubscriber(123540, 4238630);
            var trigger = dao.IsTriggerSubscriber(123540, 1);
            var subscriber = dao.IsTriggerSubscriber(1, 4238654);
            var none = dao.IsTriggerSubscriber(1, 1);
        }

        public void CreateTriggerSubscribersTest()
        {
            var dao = Container.GetInstance<ITriggerDao>();

            var trigger = new Trigger
            {
                Id = 123697,
                TriggerType = TriggerType.NotSet,
            };

            var subscriber = new Subscriber
            {
                Id = 46
            };

            dao.CreateTriggerSubscribers(CurrentUnitId, new TriggerSubscriber
            {
                Trigger = trigger,
                Subscriber = subscriber,
                SendDate = DateTime.UtcNow
            });
        }
    }
}
