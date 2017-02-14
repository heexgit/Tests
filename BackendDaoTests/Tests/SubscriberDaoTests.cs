using BackendDaoTests.Core;
using ExpertSender.DataModel.BackendDao;
using BackendDaoTests.Mocks;
using ExpertSender.Common.Helpers;
using ExpertSender.Common.QueryBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EsAppContext = BackendDaoTests.Mocks.EsAppContext;

namespace BackendDaoTests.Tests
{
    internal class SubscriberDaoTests : Tester
    {
        private const int CurrentUnitId = 1;
        private readonly ISubscriberDao _dao;

        private const int ExistSubscriberId = 4238630;
        private const string ExistEmail = "grzegorz.tylak@expertsender.com";
        private const string ExistMd5 = "DC6A810C497071BEAD1DFC2449282A63";
        private const string ExistPhone = "48900800700";
        private const string ExistCustomSubscriberId = "grzegorz.tylak";

        private const int NotExistSubscriberId = -1;
        private const string NotExistEmail = "xyz19872842@expertsender.com";
        private const string NotExistMd5 = "158DC5A944BA94E12E95F3A70DC5FB49";
        private const string NotExistPhone = "1441872842";
        private const string NotExistCustomSubscriberId = "xxppeoo3994hdfhs";

        private const string SubscriberFirstname = "Jan";
        private const string SubscriberLastname = "Kowalski";
        private const string SubscriberIp = "123.9.88.99";

        /// <summary>
        /// subskrybent jest zapisany do listy
        /// </summary>
        private const int ActiveSubscribedListId = 8;

        /// <summary>
        /// subskrybent NIE jest zapisany do listy
        /// </summary>
        private const int ActiveNotSubscribedListId = 14;

        /// <summary>
        /// subskrybent jest wypisany z listy z powodem Complaint
        /// </summary>
        private const int ActiveComplaintListId = 146;

        /// <summary>
        /// subskrybent jest wypisany z listy z powodem Api
        /// </summary>
        private const int ActiveUnsubscribedApiListId = 100;

        /// <summary>
        /// lista jest nieaktywna
        /// </summary>
        private const int DeletedListId = 3;

        private const int RequiredPropertyString = 4;
        private const int RequiredPropertyInt = 5;
        private const int DeletedProperty = 2166;

        public SubscriberDaoTests()
            : base(new EsAppContext
            {
                CurrentServiceId = CurrentUnitId
            })
        {
            _dao = Container.GetInstance<ISubscriberDao>().Use(CurrentUnitId);
        }

        public override void Start()
        {
            GetSubscriptionTest();
        }

        private void GetSubscriptionTest()
        {
            var yes = _dao.GetSubscription(15, 1);
            var both = _dao.GetSubscription(15, 2);
            var subscriber = _dao.GetSubscription(15, 0);
            var list = _dao.GetSubscription(0, 1);
            var none = _dao.GetSubscription(0, 0);
        }

        #region Use

        [TestMethod]
        public void Use_Ok_Test()
        {
            _dao.Use(CurrentUnitId);

            var acctual = ReflectionHelper.GetValueOfPrivateProperty(_dao, "DatabaseName");

            Assert.AreEqual("[ES_TR_Unit_1].[dbo].", acctual);
        }

        // @todo Id unitu nie jest weryfikowane - dorobić
        //[TestMethod]
        //[ExpectedException(typeof(jakisException))]
        //public void Use_Wrong_Test()
        //{
        //    _dao.Use(-1);
        //}

        #endregion

        #region Update

        [TestMethod]
        public void Update_Ok_Test()
        {
            var result = _dao.Update(new SetBuilder(new {Email = NotExistEmail}), new WhereBuilder(new {Id = ExistSubscriberId }));
            var subscriber = _dao.SelectOne(new SelectBuilder(new {Email = string.Empty}), new WhereBuilder(new {Id = ExistSubscriberId}));

            Assert.AreEqual(1, result);
            Assert.AreEqual(NotExistEmail, subscriber.Email);
        }

        #endregion
    }
}