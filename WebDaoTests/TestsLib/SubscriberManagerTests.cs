using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSender.DataModel.Enums;
using ExpertSender.Lib.SubscriberManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using WebDaoTests.Core;
using EsAppContext = WebDaoTests.Mocks.EsAppContext;
using Resources = ExpertSender.Lib.SubscriberManager.Resources;

// ReSharper disable UnusedVariable, PossibleIntendedRethrow

namespace WebDaoTests.TestsLib
{
    /*

        KOD może się nie buildować ze wzgledu na wycoafanie zmian z DEV
        rozważyć przełączenie do MULTI

    */
    [TestClass]
    public class SubscriberManagerTests : Tester
    {
        private const int CurrentUnitId = 1;

        private const int ExistSubscriberId = 4238630;
        private const int NotExistSubscriberId = -1;
        private const string ExistSubscriber = "grzegorz.tylak@expertsender.com";
        private const string NotExistSubscriber = "xyz19872842@expertsender.com";

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
                var result = SM_AddAndIgnore()
                    .AddSubscriber(
                        DeletedListId,
                        id: null, //SubscriberId
                        email: ExistSubscriber,
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
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_FieldIsRequired_Test()
        {
            try
            {
                var result = SM_AddAndIgnore()
                    .AddSubscriber(
                        ActiveSubscribedListId,
                        id: null, //SubscriberId
                        email: ExistSubscriber,
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
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_InvalidValue_Test()
        {
            try
            {
                var result = SM_AddAndIgnore()
                    .AddSubscriber(
                        ActiveSubscribedListId,
                        id: null, //SubscriberId
                        email: ExistSubscriber,
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
            }
        }

        [TestMethod]
        public void AddSubscriber_WasIgnored_Test()
        {
            var result = SM_AddAndIgnore()
                .AddSubscriber(
                    ActiveSubscribedListId,
                    id: null, //SubscriberId
                    email: ExistSubscriber,
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
            var result = SM_AddAndUpdate()
                .AddSubscriber(
                    ActiveSubscribedListId,
                    id: null, //SubscriberId
                    email: ExistSubscriber,
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
        public void AddSubscriber_WasAddedToList_Test()
        {
            AddSubscriberResult result;

            using (var tran = Container.GetInstance<ISession>().BeginTransaction())
            {
                try
                {
                    result = SM_AddAndUpdate()
                        .AddSubscriber(
                            ActiveNotSubscribedListId,
                            id: null, //SubscriberId
                            email: ExistSubscriber,
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
                }
                finally
                {
                    // przywracamy zmienione obiekty do poprzedniego stanu
                    tran.Rollback();
                }
            }

            Assert.AreEqual(true, result.WasAddedToList);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_EmailRejected_Test()
        {
            try
            {
                var result = SM_AddAndUpdate()
                    .AddSubscriber(
                        ActiveComplaintListId,
                        id: null, //SubscriberId
                        email: ExistSubscriber,
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
            }
            catch (SubscriberManagerException ex)
            {
                if (
                    ex.FieldsErrors.SingleOrDefault(e => e.Key == "email").Value.Contains(Resources.SubscriberManager.EmailRejected)
                )
                    throw ex;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_EmailRejected_Option_Test()
        {
            try
            {
                var result = SM_AddAndUpdate()
                    .AddSubscriber(
                        ActiveComplaintListId,
                        id: null, //SubscriberId
                        email: ExistSubscriber,
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
            }
            catch (SubscriberManagerException ex)
            {
                if (
                    ex.FieldsErrors.SingleOrDefault(e => e.Key == "email").Value.Contains(Resources.SubscriberManager.EmailRejected)
                )
                    throw ex;
            }
        }

        [TestMethod]
        public void AddSubscriber_WasReAddedToList_Test()
        {
            AddSubscriberResult result;

            using (var tran = Container.GetInstance<ISession>().BeginTransaction())
            {
                try
                {
                    result = SM_AddAndUpdate_AllowUnsubscribedByUser()
                        .AddSubscriber(
                            ActiveUnsubscribedApiListId,
                            id: null, //SubscriberId
                            email: ExistSubscriber,
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
                }
                finally
                {
                    // przywracamy zmienione obiekty do poprzedniego stanu
                    tran.Rollback();
                }
            }

            Assert.AreEqual(true, result.WasReAddedToList && result.SubscriberId > 0 && result.ListId == ActiveUnsubscribedApiListId);
        }

        [TestMethod]
        public void AddSubscriber_WasAdded_Test()
        {
            AddSubscriberResult result;

            using (var tran = Container.GetInstance<ISession>().BeginTransaction())
            {
                try
                {
                    result = SM_AddAndUpdate()
                        .AddSubscriber(
                            ActiveNotSubscribedListId,
                            id: null, //SubscriberId
                            email: NotExistSubscriber,
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
                }
                finally
                {
                    // przywracamy zmienione obiekty do poprzedniego stanu
                    tran.Rollback();
                }
            }

            Assert.AreEqual(true, result.WasAdded && result.SubscriberId > 0 && result.ListId == ActiveNotSubscribedListId);
        }

        [TestMethod]
        public void AddSubscriber_ReplaceSubscriber_Test()
        {
            AddSubscriberResult result;

            using (var tran = Container.GetInstance<ISession>().BeginTransaction())
            {
                try
                {
                    result = SM_AddAndReplace()
                        .AddSubscriber(
                            ActiveSubscribedListId,
                            id: null, //SubscriberId
                            email: ExistSubscriber,
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
                }
                finally
                {
                    // przywracamy zmienione obiekty do poprzedniego stanu
                    tran.Rollback();
                }
            }

            Assert.AreEqual(true, result.WasAlreadyOnList);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteSubscriber_NotExists_Test()
        {
            var subscriberManager = Container.GetInstance<SubscriberManager>();

            using (var tran = Container.GetInstance<ISession>().BeginTransaction())
            {
                try
                {
                    subscriberManager
                        .DeleteSubscriber(
                            subscriberId: NotExistSubscriberId,
                            messageGuid: null,
                            listId: ActiveSubscribedListId,
                            unitId: CurrentUnitId,
                            reason: UnsubscribeReason.Api,
                            browser: null,
                            browserVersion: null,
                            device: null,
                            deviceVersion: null,
                            environment: null,
                            environmentVersion: null,
                            renderingEngine: null,
                            renderingEngineVersion: null,
                            isMobile: false,
                            clientLanguage: null,
                            clientEmailDomain: null
                        );
                }
                finally
                {
                    // przywracamy zmienione obiekty do poprzedniego stanu
                    tran.Rollback();
                }
            }
        }

        [TestMethod]
        public void DeleteSubscriber_Exist_Test()
        {
            List<RemovalModel> result;

            var subscriberManager = Container.GetInstance<SubscriberManager>();

            using (var tran = Container.GetInstance<ISession>().BeginTransaction())
            {
                try
                {
                    result = subscriberManager
                        .DeleteSubscriber(
                            subscriberId: ExistSubscriberId,
                            messageGuid: null,
                            listId: ActiveSubscribedListId,
                            unitId: CurrentUnitId,
                            reason: UnsubscribeReason.Api,
                            browser: null,
                            browserVersion: null,
                            device: null,
                            deviceVersion: null,
                            environment: null,
                            environmentVersion: null,
                            renderingEngine: null,
                            renderingEngineVersion: null,
                            isMobile: false,
                            clientLanguage: null,
                            clientEmailDomain: null
                        );
                }
                finally
                {
                    // przywracamy zmienione obiekty do poprzedniego stanu
                    tran.Rollback();
                }
            }

            Assert.AreEqual(1, result.Count(r => r.ListId == ActiveSubscribedListId));
        }

        #region GetSubscriberManager

        private SubscriberManager SM_AddAndIgnore()
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

        private SubscriberManager SM_AddAndUpdate()
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

        private SubscriberManager SM_AddAndUpdate_AllowUnsubscribedByUser()
        {
            var subscriberManager = Container.GetInstance<SubscriberManager>();
            return subscriberManager
                .PreAddSubscriber(
                    CurrentUnitId,
                    SubscriberManagerMode.AddAndUpdate,
                    SubscriptionSource.Api,
                    forceConfirmation: false,
                    addUnsubscribed: false,
                    addUnsubscribedByUser: true,
                    subscriptionLanguage: "en-US"
                );
        }

        private SubscriberManager SM_AddAndReplace()
        {
            var subscriberManager = Container.GetInstance<SubscriberManager>();
            return subscriberManager
                .PreAddSubscriber(
                    CurrentUnitId,
                    SubscriberManagerMode.AddAndReplace,
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