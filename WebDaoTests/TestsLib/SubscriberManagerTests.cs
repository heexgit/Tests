using System.Collections.Generic;
using System.Linq;
using ExpertSender.DataModel.Enums;
using ExpertSender.Lib.SubscriberManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDaoTests.Core;
using EsAppContext = WebDaoTests.Mocks.EsAppContext;
using Resources = ExpertSender.Lib.SubscriberManager.Resources;

namespace WebDaoTests.TestsLib
{
    [TestClass]
    public class SubscriberManagerTests : Tester
    {
        private const int CurrentUnitId = 1;

        private const int ActiveListId = 8;
        private const int DeletedListId = 3;

        private const int RequiredPropertyString = 4;
        private const int RequiredPropertyInt = 5;
        private const int DeletedProperty = 2166;

        public SubscriberManagerTests()
            : base(new EsAppContext { CurrentServiceId = CurrentUnitId })
        { }

        public override void Start()
        {
            AddSubscriber_ListDoesNotBelongToService_Test();
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_ListDoesNotBelongToService_Test()
        {
            try
            {
                var result = GetSubscriberManager_AddAndIgnore()
                    .AddSubscriber(
                        DeletedListId,
                        id: null, //SubscriberId
                        email: "grzegorz.tylak@expertsender.com",
                        emailMd5: null,
                        firstName: "Jan",
                        lastName: "Kowalski",
                        trackingCode: null,
                        vendor: null,
                        ip: "123.9.88.99",
                        propertiesDictionary: new Dictionary<int, object> {{DeletedProperty, "jakiś blok"}}
                    );
            }
            catch (SubscriberManagerException ex)
            {
                if (
                    ex.FieldsErrors.SingleOrDefault(e => e.Key == "listId").Value.Contains(Resources.SubscriberManager.ListDoesNotBelongToService)
                    && ex.FieldsErrors.SingleOrDefault(e => e.Key == "serviceId").Value.Contains(Resources.SubscriberManager.ListDoesNotBelongToService)
                )
                    throw ex;
                else
                    Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_FieldIsRequired_Test()
        {
            try
            {
                var result = GetSubscriberManager_AddAndIgnore()
                    .AddSubscriber(
                        ActiveListId,
                        id: null, //SubscriberId
                        email: "grzegorz.tylak@expertsender.com",
                        emailMd5: null,
                        firstName: "Jan",
                        lastName: "Kowalski",
                        trackingCode: null,
                        vendor: null,
                        ip: "123.9.88.99",
                        propertiesDictionary: new Dictionary<int, object> {{DeletedProperty, "jakiś blok"}}
                    );
            }
            catch (SubscriberManagerException ex)
            {
                if (
                    ex.PropertiesErrors.SingleOrDefault(e => e.Key == RequiredPropertyString).Value.Contains(string.Format(Resources.SubscriberManager.FieldIsRequired, "wymagana cecha"))
                    && ex.PropertiesErrors.SingleOrDefault(e => e.Key == RequiredPropertyInt).Value.Contains(string.Format(Resources.SubscriberManager.FieldIsRequired, "cecha liczbowa xx3"))
                )
                    throw ex;
                else
                    Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_InvalidValue_Test()
        {
            try
            {
                var result = GetSubscriberManager_AddAndIgnore()
                    .AddSubscriber(
                        ActiveListId,
                        id: null, //SubscriberId
                        email: "grzegorz.tylak@expertsender.com",
                        emailMd5: null,
                        firstName: "Jan",
                        lastName: "Kowalski",
                        trackingCode: null,
                        vendor: null,
                        ip: "123.9.88.99",
                        propertiesDictionary: new Dictionary<int, object>
                        {
                            {RequiredPropertyString, "jakiś blok"},
                            {RequiredPropertyInt, "jakiś blok"},
                            {DeletedProperty, "jakiś blok"}
                        }
                    );
            }
            catch (SubscriberManagerException ex)
            {
                if (
                    ex.PropertiesErrors.SingleOrDefault(e => e.Key == RequiredPropertyInt).Value.Contains(string.Format(Resources.SubscriberManager.InvalidValue, "cecha liczbowa xx3"))
                )
                    throw ex;
                else
                    Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void AddSubscriber_WasIgnored_Test()
        {
            var result = GetSubscriberManager_AddAndIgnore()
                .AddSubscriber(
                    ActiveListId,
                    id: null, //SubscriberId
                    email: "grzegorz.tylak@expertsender.com",
                    emailMd5: null,
                    firstName: "Jan",
                    lastName: "Kowalski",
                    trackingCode: null,
                    vendor: null,
                    ip: "123.9.88.99",
                    propertiesDictionary: new Dictionary<int, object>
                    {
                        {RequiredPropertyString, "jakiś blok"},
                        {RequiredPropertyInt, 7897987},
                        {DeletedProperty, "jakiś blok"}
                    }
                );

            Assert.AreEqual(true, result.WasIgnored);
        }

        [TestMethod]
        public void AddSubscriber_WasAlreadyOnList_Test()
        {
            var result = GetSubscriberManager_AddAndUpdate()
                .AddSubscriber(
                    ActiveListId,
                    id: null, //SubscriberId
                    email: "grzegorz.tylak@expertsender.com",
                    emailMd5: null,
                    firstName: "Jan",
                    lastName: "Kowalski",
                    trackingCode: null,
                    vendor: null,
                    ip: "123.9.88.99",
                    propertiesDictionary: new Dictionary<int, object>
                    {
                        {RequiredPropertyString, "jakiś blok"},
                        {RequiredPropertyInt, 7897987},
                        {DeletedProperty, "jakiś blok"}
                    }
                );

            Assert.AreEqual(true, result.WasAlreadyOnList);
        }

        [TestMethod]
        public void AddSubscriber_1_Test()
        {
            var result = GetSubscriberManager_AddAndUpdate()
                .AddSubscriber(
                    11,//ActiveListId,
                    id: null, //SubscriberId
                    email: "grzegorz.tylak@expertsender.com",
                    emailMd5: null,
                    firstName: "Jan",
                    lastName: "Kowalski",
                    trackingCode: null,
                    vendor: null,
                    ip: "123.9.88.99",
                    propertiesDictionary: new Dictionary<int, object>
                    {
                        {RequiredPropertyString, "jakiś blok"},
                        {RequiredPropertyInt, 7897987},
                        {DeletedProperty, "jakiś blok"}
                    }
                );

            Assert.AreEqual(true, result.WasAddedToList);
        }

        #region GetSubscriberManager

        private SubscriberManager GetSubscriberManager_AddAndIgnore()
        {
            var subscriberManager = Container.GetInstance<SubscriberManager>();
            return subscriberManager
                .PreAddSubscriber(
                    CurrentUnitId,
                    SubscriberManagerMode.AddAndIgnore,
                    SubscriptionSource.Api,
                    forceConfirmation: false,
                    addUnsubscribed: false,
                    addUnsubscribedByUser: false,
                    subscriptionLanguage: "en-US"
                );
        }

        private SubscriberManager GetSubscriberManager_AddAndUpdate()
        {
            var subscriberManager = Container.GetInstance<SubscriberManager>();
            return subscriberManager
                .PreAddSubscriber(
                    CurrentUnitId,
                    SubscriberManagerMode.AddAndUpdate,
                    SubscriptionSource.Api,
                    forceConfirmation: false,
                    addUnsubscribed: false,
                    addUnsubscribedByUser: false,
                    subscriptionLanguage: "en-US"
                );
        }
        #endregion
    }
}