using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSender.Common.Helpers;
using ExpertSender.Common.QueryBuilder;
using ExpertSender.DataModel.Dao;
using ExpertSender.DataModel.Dao.Statistics;
using ExpertSender.DataModel.Enums;
using ExpertSender.Lib;
using ExpertSender.Lib.SubscriberManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDaoTests.Core;
using EsAppContext = WebDaoTests.Mocks.EsAppContext;
using Resources = ExpertSender.Lib.SubscriberManager.Resources;
using SubscriberManager = ExpertSender.Lib.SubscriberManager.SubscriberManager;

// ReSharper disable UnusedVariable, PossibleIntendedRethrow

namespace WebDaoTests.LibTests
{
    [TestClass]
    public class SubscriberManagerTests : Tester
    {
        private const int CurrentUnitId = 1;

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

        public SubscriberManagerTests()
            : base(new EsAppContext {CurrentServiceId = CurrentUnitId})
        { }

        public override void Start()
        { }

        #region ListDoesNotBelongToService

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Email_E_ListDoesNotBelongToService_Test()
        {
            ListDoesNotBelongToService(ChannelType.SmsMms, MatchBy.Email, email: ExistEmail);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Phone_P_ListDoesNotBelongToService_Test()
        {
            ListDoesNotBelongToService(ChannelType.SmsMms, MatchBy.Phone, phone: ExistPhone);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Email_EP_ListDoesNotBelongToService_Test()
        {
            ListDoesNotBelongToService(ChannelType.SmsMms, MatchBy.Email, email: ExistEmail, phone: ExistPhone);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Phone_EP_ListDoesNotBelongToService_Test()
        {
            ListDoesNotBelongToService(ChannelType.SmsMms, MatchBy.Phone, email: ExistEmail, phone: ExistPhone);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Email_EPD_ListDoesNotBelongToService_Test()
        {
            ListDoesNotBelongToService(ChannelType.SmsMms, MatchBy.Email, email: ExistEmail, phone: ExistPhone, emailMd5: ExistMd5);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Phone_EPD_ListDoesNotBelongToService_Test()
        {
            ListDoesNotBelongToService(ChannelType.SmsMms, MatchBy.Phone, email: ExistEmail, phone: ExistPhone, emailMd5: ExistMd5);
        }

        private AddSubscriberResult ListDoesNotBelongToService(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null)
        {
            AddSubscriberResult result = null;

            try
            {
                result = SM_AddAndIgnore(channelType, matchBy)
                    .AddSubscriber(
                        DeletedListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
                        propertiesDictionary: new Dictionary<int, object> {{DeletedProperty, "jakiś blok"}}
                    );
            }
            catch (SubscriberManagerException ex)
            {
                Assert.AreEqual(2, ex.FieldsErrors.Count);

                if (
                    ex.FieldsErrors.SingleOrDefault(e => e.Key == "listId").Value.Contains(Resources.SubscriberManager.ListDoesNotBelongToService)
                    && ex.FieldsErrors.SingleOrDefault(e => e.Key == "serviceId").Value.Contains(Resources.SubscriberManager.ListDoesNotBelongToService)
                )
                    throw ex;
            }

            return result;
        }
        #endregion

        #region FieldIsRequired

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Email_E_FieldIsRequired_Test()
        {
            FieldIsRequired(ChannelType.SmsMms, MatchBy.Email, email: NotExistEmail);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Phone_P_FieldIsRequired_Test()
        {
            FieldIsRequired(ChannelType.SmsMms, MatchBy.Phone, phone: NotExistPhone);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Email_EP_FieldIsRequired_Test()
        {
            FieldIsRequired(ChannelType.SmsMms, MatchBy.Email, email: NotExistEmail, phone: NotExistPhone);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Phone_EP_FieldIsRequired_Test()
        {
            FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.Phone,
                email: NotExistEmail,
                phone: NotExistPhone
            );
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void AddSubscriber_Email_PD_FieldIsRequired_Test()
        {
            FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.Email,
                phone: NotExistPhone,
                emailMd5: NotExistMd5,
                ifErrorConditition: ex => ex.FieldsErrors.SingleOrDefault(e => e.Key == "email").Value.Contains(Resources.SubscriberManager.EmailIsInvalid)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Phone_ED_FieldIsRequired_Test()
        {
            FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.Phone,
                email: NotExistEmail,
                emailMd5: NotExistMd5,
                ifErrorConditition: ex => ex.FieldsErrors.SingleOrDefault(e => e.Key == "phone").Value.Contains(Resources.SubscriberManager.PhoneIsInvalid)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Custom_EP_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.CustomSubscriberId,
                email: NotExistEmail,
                phone: NotExistPhone,
                ifErrorConditition: ex => ex.FieldsErrors.SingleOrDefault(e => e.Key == "customSubscriberId").Value.Contains(Resources.SubscriberManager.CustomSubscriberIdIsInvalid)
            );
        }
        
        /// <summary>
        /// Przy MatchBy.CustomSubscriberId wymagane są customSubscriberId oraz Email lub Telefon
        /// Dla kanału SmsMms podajemy komunikat o wymaganym mailu lub telefonie
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Custom_NoData_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.CustomSubscriberId,
                ifErrorConditition: ex =>
                    ex.FieldsErrors.Count == 3
                    && !ex.PropertiesErrors.Any()
                    && ex.FieldsErrors.Any(e => e.Key == "customSubscriberId" && e.Value.Contains(Resources.SubscriberManager.CustomSubscriberIdIsInvalid))
                    && ex.FieldsErrors.Any(e => e.Key == "email" && e.Value.Contains(Resources.SubscriberManager.EmailOrPhoneRequired))
                    && ex.FieldsErrors.Any(e => e.Key == "phone" && e.Value.Contains(Resources.SubscriberManager.EmailOrPhoneRequired))
            );
        }
        
        /// <summary>
        /// Przy MatchBy.CustomSubscriberId wymagane są customSubscriberId oraz Email lub Telefon
        /// Dla kanału Email nie podajemy komunikatu o wymaganym telefonie
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Custom_NoData_ChEmail_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.Email,
                MatchBy.CustomSubscriberId,
                ifErrorConditition: ex =>
                    ex.FieldsErrors.Count == 2
                    && !ex.PropertiesErrors.Any()
                    && ex.FieldsErrors.Any(e => e.Key == "customSubscriberId" && e.Value.Contains(Resources.SubscriberManager.CustomSubscriberIdIsInvalid))
                    && ex.FieldsErrors.Any(e => e.Key == "email" && e.Value.Contains(Resources.SubscriberManager.EmailRequired))
            );
        }

        /// <summary>
        /// Przy MatchBy.CustomSubscriberId wymagany jest Email lub Telefon
        /// Dla kanału SmsMms podajemy komunikat o wymaganym mailu lub telefonie
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Custom_NoContacts_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.CustomSubscriberId,
                customSubscriberId: ExistCustomSubscriberId,
                ifErrorConditition: ex =>
                    ex.FieldsErrors.Count == 2
                    && !ex.PropertiesErrors.Any()
                    && ex.FieldsErrors.Any(e => e.Key == "email" && e.Value.Contains(Resources.SubscriberManager.EmailOrPhoneRequired))
                    && ex.FieldsErrors.Any(e => e.Key == "phone" && e.Value.Contains(Resources.SubscriberManager.EmailOrPhoneRequired))
            );
        }

        /// <summary>
        /// Przy MatchBy.CustomSubscriberId wymagany jest Email
        /// Dla kanału Email nie podajemy komunikatu o wymaganym telefonie
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Custom_NoContacts_ChEmail_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.Email,
                MatchBy.CustomSubscriberId,
                customSubscriberId: ExistCustomSubscriberId,
                ifErrorConditition: ex =>
                    ex.FieldsErrors.Count == 1
                    && !ex.PropertiesErrors.Any()
                    && ex.FieldsErrors.Any(e => e.Key == "email" && e.Value.Contains(Resources.SubscriberManager.EmailRequired))
            );
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Id_E_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.Id,
                email: NotExistEmail,
                ifErrorConditition: ex => ex.FieldsErrors.SingleOrDefault(e => e.Key == "subscriberId").Value.Contains(Resources.SubscriberManager.IdIsInvalid)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Id_EP_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.Id,
                email: NotExistEmail,
                phone: NotExistPhone,
                ifErrorConditition: ex => ex.FieldsErrors.SingleOrDefault(e => e.Key == "subscriberId").Value.Contains(Resources.SubscriberManager.IdIsInvalid)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Id_NoData_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.Id,
                ifErrorConditition: ex =>
                    ex.FieldsErrors.Any(e => e.Key == "subscriberId" && e.Value.Contains(Resources.SubscriberManager.IdIsInvalid))
                    && ex.FieldsErrors.Any(e => e.Key == "email" && e.Value.Contains(Resources.SubscriberManager.EmailOrPhoneRequired))
                    && ex.FieldsErrors.Any(e => e.Key == "phone" && e.Value.Contains(Resources.SubscriberManager.EmailOrPhoneRequired))
            );
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Id_NoContacts_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.Id,
                subscriberId: ExistSubscriberId,
                ifErrorConditition: ex =>
                    ex.FieldsErrors.Any(e => e.Key == "email" && e.Value.Contains(Resources.SubscriberManager.EmailOrPhoneRequired))
                    && ex.FieldsErrors.Any(e => e.Key == "phone" && e.Value.Contains(Resources.SubscriberManager.EmailOrPhoneRequired))
            );
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Email_EPx_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.Email, 
                email: NotExistEmail,
                phone: "xxx",
                ifErrorConditition: ex =>
                    ex.FieldsErrors.Any(e => e.Key == "phone" && e.Value.Contains(Resources.SubscriberManager.PhoneIsInvalid))
            );
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Email_EP000_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.Email, 
                email: NotExistEmail,
                phone: "123456789013467901346790",
                ifErrorConditition: ex =>
                    ex.FieldsErrors.SingleOrDefault(e => e.Key == "phone").Value.Any(v => v.StartsWith(Resources.SubscriberManager.PhoneTooLong.Substring(0, 20)))
            );
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Phone_ExP_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.Phone,
                email: "xxx",
                phone: NotExistPhone,
                ifErrorConditition: ex =>
                    ex.FieldsErrors.SingleOrDefault(e => e.Key == "email").Value.Contains(Resources.SubscriberManager.EmailIsInvalid)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Phone_E000P_FieldIsRequired_Test()
        {
            var result = FieldIsRequired(
                ChannelType.SmsMms,
                MatchBy.Phone,
                email: "#dss@sdsdsxxx.sooedm,W222",
                phone: NotExistPhone,
                ifErrorConditition: ex =>
                    ex.FieldsErrors.SingleOrDefault(e => e.Key == "email").Value.Contains(Resources.SubscriberManager.EmailIsInvalid)
            );
        }

        private AddSubscriberResult FieldIsRequired(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;

            try
            {
                result = SM_AddAndIgnore(channelType, matchBy)
                    .AddSubscriber(
                        ActiveSubscribedListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
                        propertiesDictionary: new Dictionary<int, object> {{DeletedProperty, "jakiś blok"}}
                    );
            }
            catch (SubscriberManagerException ex)
            {
                if (ifErrorConditition == null)
                {
                    ifErrorConditition = exx =>
                        exx.PropertiesErrors.SingleOrDefault(e => e.Key == RequiredPropertyString).Value.Contains(string.Format(Resources.SubscriberManager.FieldIsRequired, "wymagana cecha"))
                        && exx.PropertiesErrors.SingleOrDefault(e => e.Key == RequiredPropertyInt).Value.Contains(string.Format(Resources.SubscriberManager.FieldIsRequired, "cecha liczbowa xx3"));
                }

                if (ifErrorConditition(ex))
                    throw ex;
            }

            return result;
        }
        #endregion

        #region InvalidPropValue
        
        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Email_E_InvalidPropValue_Test()
        {
            InvalidPropValue(ChannelType.SmsMms, MatchBy.Email, email: ExistEmail);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Phone_P_InvalidPropValue_Test()
        {
            InvalidPropValue(ChannelType.SmsMms, MatchBy.Phone, phone: ExistPhone);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Email_D_InvalidPropValue_Test()
        {
            InvalidPropValue(ChannelType.SmsMms, MatchBy.Email, emailMd5: ExistMd5);
        }

        private AddSubscriberResult InvalidPropValue(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;

            try
            {
                result = SM_AddAndIgnore(channelType, matchBy)
                    .AddSubscriber(
                        ActiveSubscribedListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
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
                if (ifErrorConditition == null)
                {
                    ifErrorConditition = exx =>
                        exx.PropertiesErrors.SingleOrDefault(e => e.Key == RequiredPropertyInt).Value.Contains(string.Format(Resources.SubscriberManager.InvalidValue, "cecha liczbowa xx3"));
                }

                if (ifErrorConditition(ex))
                    throw ex;
            }

            return result;
        }
        #endregion

        #region WasIgnored

        [TestMethod]
        public void AddSubscriber_Email_WasIgnored_Test()
        {
            WasIgnored(ChannelType.SmsMms, MatchBy.Email, email: ExistEmail);
        }

        [TestMethod]
        public void AddSubscriber_Phone_WasIgnored_Test()
        {
            WasIgnored(ChannelType.SmsMms, MatchBy.Phone, phone: ExistPhone);
        }

        public AddSubscriberResult WasIgnored(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;

            try
            {
                result = SM_AddAndIgnore(channelType, matchBy)
                    .AddSubscriber(
                        ActiveSubscribedListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
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
                if (ifErrorConditition == null)
                {
                    ifErrorConditition = exx =>
                        exx.PropertiesErrors.Any(e => e.Key == RequiredPropertyInt && e.Value.Contains(string.Format(Resources.SubscriberManager.InvalidValue, "cecha liczbowa xx3")))
                        && !exx.FieldsErrors.Any();
                }

                if (ifErrorConditition(ex))
                    throw ex;
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.WasIgnored);

            return result;
        }
        #endregion

        #region WasAlreadyOnList

        [TestMethod]
        public void AddSubscriber_Email_WasAlreadyOnList_Test()
        {
            var result = WasAlreadyOnList(ChannelType.SmsMms, MatchBy.Email, email: ExistEmail);
        }

        [TestMethod]
        public void AddSubscriber_Phone_WasAlreadyOnList_Test()
        {
            var result = WasAlreadyOnList(ChannelType.SmsMms, MatchBy.Phone, phone: ExistPhone);
        }

        private AddSubscriberResult WasAlreadyOnList(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;
            IEnumerable<FactSubscription> facts;

            try
            {
                result = SM_AddAndUpdate(channelType, matchBy)
                    .AddSubscriber(
                        listId: ActiveSubscribedListId,
                        email: email,
                        id: subscriberId,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
                        propertiesDictionary: new Dictionary<int, object>
                        {
                            {RequiredPropertyString, "jakiś blok"},
                            {RequiredPropertyInt, 7897987},
                            {DeletedProperty, "jakiś blok"}
                        }
                    );

                facts = LoadSubscriptions(result.SubscriberId, result.ListId);
            }
            catch (SubscriberManagerException ex)
            {
                //if (ifErrorConditition == null)
                //{
                //    ifErrorConditition = exx =>
                //        exx.PropertiesErrors.SingleOrDefault(e => e.Key == RequiredPropertyInt).Value.Contains(string.Format(Resources.SubscriberManager.InvalidValue, "cecha liczbowa xx3"));
                //}

                //if (ifErrorConditition(ex))
                //    throw ex;
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.WasAlreadyOnList);

            return result;
        }
        #endregion

        #region WasAddedToList

        [TestMethod]
        public void AddSubscriber_Email_E_WasAddedToList_Test()
        {
            var result = WasAddedToList(ChannelType.SmsMms, MatchBy.Email, email: ExistEmail);

            var fact = result.Item2;
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.Email, fact.ChannelTypeId);
            }
            
            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(ExistEmail, subscriber.Email);
            Assert.AreEqual(ExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(fact.DomainId, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);
            
            Assert.AreEqual(ExistPhone, subscriber.Phone);
            Assert.IsTrue(ExistPhone.EndsWith(subscriber.PhoneLocal));
            Assert.IsTrue(ExistPhone.StartsWith(subscriber.DialingPrefix));
        }

        [TestMethod]
        public void AddSubscriber_Email_D_WasAddedToList_Test()
        {
            var result = WasAddedToList(ChannelType.SmsMms, MatchBy.Email, emailMd5: ExistMd5);

            var fact = result.Item2;
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.Email, fact.ChannelTypeId);
            }
            
            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(ExistEmail, subscriber.Email);
            Assert.AreEqual(ExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(fact.DomainId, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);

            Assert.AreEqual(ExistPhone, subscriber.Phone);
            Assert.IsTrue(ExistPhone.EndsWith(subscriber.PhoneLocal));
            Assert.IsTrue(ExistPhone.StartsWith(subscriber.DialingPrefix));
        }

        [TestMethod]
        public void AddSubscriber_Phone_P_WasAddedToList_Test()
        {
            var result = WasAddedToList(ChannelType.SmsMms, MatchBy.Phone, phone: ExistPhone);

            var fact = result.Item2;
            if (fact != null)
            {
                Assert.AreEqual(fact.DomainId, 0);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.SmsMms, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(ExistPhone, subscriber.Phone);
            Assert.IsTrue(ExistPhone.EndsWith(subscriber.PhoneLocal));
            Assert.IsTrue(ExistPhone.StartsWith(subscriber.DialingPrefix));

            Assert.AreEqual(ExistEmail, subscriber.Email);
            Assert.AreEqual(ExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(3, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);
        }

        private Tuple<AddSubscriberResult, FactSubscription> WasAddedToList(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;
            FactSubscription fact = null;

            try
            {
                result = SM_AddAndUpdate(channelType, matchBy)
                    .AddSubscriber(
                        ActiveNotSubscribedListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
                        propertiesDictionary: new Dictionary<int, object>
                        {
                            {RequiredPropertyString, "jakiś blok"},
                            {RequiredPropertyInt, 7897987},
                            {DeletedProperty, "jakiś blok"}
                        }
                    );

                fact = LoadSubscriptions(result.SubscriberId, result.ListId).FirstOrDefault();
            }
            catch (SubscriberManagerException ex)
            { }

            Assert.IsNotNull(result);
            if (result != null)
            {
                Assert.AreEqual(true, result.WasAddedToList);
            }

            Assert.IsNotNull(fact);

            return new Tuple<AddSubscriberResult, FactSubscription>(result, fact);
        }
        #endregion

        #region EmailRejected

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Email_EmailRejected_Test()
        {
            var result = EmailRejected(ChannelType.SmsMms, MatchBy.Email, email: ExistEmail);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Phone_EmailRejected_Test()
        {
            var result = EmailRejected(ChannelType.SmsMms, MatchBy.Phone, phone: ExistPhone);
        }

        public AddSubscriberResult EmailRejected(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;

            try
            {
                result = SM_AddAndUpdate(channelType, matchBy)
                    .AddSubscriber(
                        ActiveComplaintListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
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
                if (ifErrorConditition == null)
                {
                    if (matchBy == MatchBy.Email)
                        ifErrorConditition = exx =>
                            exx.FieldsErrors.SingleOrDefault(e => e.Key == "email").Value.Contains(Resources.SubscriberManager.EmailRejected);
                    else
                        ifErrorConditition = exx =>
                            exx.FieldsErrors.SingleOrDefault(e => e.Key == "phone").Value.Contains(Resources.SubscriberManager.PhoneRejected);
                }

                if (ifErrorConditition(ex))
                    throw ex;
            }

            return result;
        }
        #endregion

        #region AddUnsubscribed

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Email_AddUnsubscribed_EmailRejected_Test()
        {
            var result = AddUnsubscribed(ChannelType.SmsMms, MatchBy.Email, email: ExistEmail);
        }

        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_Phone_AddUnsubscribed_EmailRejected_Test()
        {
            var result = AddUnsubscribed(ChannelType.SmsMms, MatchBy.Phone, phone: ExistPhone);
        }

        private AddSubscriberResult AddUnsubscribed(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;
            try
            {
                result = SM_AddAndUpdate_AddUnsubscribed(channelType, matchBy)
                    .AddSubscriber(
                        ActiveComplaintListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
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
                if (ifErrorConditition == null)
                {
                    if (matchBy == MatchBy.Email)
                        ifErrorConditition = exx =>
                            exx.FieldsErrors.SingleOrDefault(e => e.Key == "email").Value.Contains(Resources.SubscriberManager.EmailRejected);
                    else
                        ifErrorConditition = exx =>
                            exx.FieldsErrors.SingleOrDefault(e => e.Key == "phone").Value.Contains(Resources.SubscriberManager.PhoneRejected);
                }

                if (ifErrorConditition(ex))
                    throw ex;
            }

            return result;
        }
        #endregion

        #region WasReAddedToList

        /// <summary>
        /// Sprawda czy Email zostanie ponownie wpisany na listę (zostanie usunięte wypisanie)
        /// </summary>
        [TestMethod]
        public void AddSubscriber_Email_WasReAddedToList_Test()
        {
            var result = WasReAddedToList(ChannelType.SmsMms, MatchBy.Email, email: ExistEmail);

            var facts = result.Item2;
            var fact = facts.SingleOrDefault();
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.Email, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(ExistEmail, subscriber.Email);
            Assert.AreEqual(ExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(fact.DomainId, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);
        }

        /// <summary>
        /// Sprawda czy Phone zostanie ponownie wpisany na listę (zostanie usunięte wypisanie)
        /// </summary>
        [TestMethod]
        public void AddSubscriber_Phone_WasReAddedToList_Test()
        {
            var result = WasReAddedToList(ChannelType.SmsMms, MatchBy.Phone, phone: ExistPhone);

            var facts = result.Item2;
            var fact = facts.SingleOrDefault();
            if (fact != null)
            {
                Assert.AreEqual(fact.DomainId, 0);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.SmsMms, fact.ChannelTypeId);
            }
            
            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(ExistPhone, subscriber.Phone);
            Assert.IsTrue(ExistPhone.EndsWith(subscriber.PhoneLocal));
            Assert.IsTrue(ExistPhone.StartsWith(subscriber.DialingPrefix));
        }

        private Tuple<AddSubscriberResult, IEnumerable<FactSubscription>> WasReAddedToList(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result;
            IEnumerable<FactSubscription> facts;

            try
            {
                result = SM_AddAndUpdate_AllowUnsubscribedByUser(channelType, matchBy)
                    .AddSubscriber(
                        ActiveUnsubscribedApiListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
                        propertiesDictionary: new Dictionary<int, object>
                        {
                            {RequiredPropertyString, "jakiś blok"},
                            {RequiredPropertyInt, 7897987},
                            {DeletedProperty, "jakiś blok"}
                        }
                    );

                facts = LoadSubscriptions(result.SubscriberId, result.ListId);
            }
            catch (SubscriberManagerException ex)
            {
                //if (ifErrorConditition == null)
                //{
                //    ifErrorConditition = exx =>
                //        exx.FieldsErrors.SingleOrDefault(e => e.Key == "email").Value.Contains(Resources.SubscriberManager.EmailRejected);
                //}

                //if (ifErrorConditition(ex))
                    throw ex;
            }
            
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.WasReAddedToList && result.SubscriberId > 0 && result.ListId == ActiveUnsubscribedApiListId);
            
            Assert.IsNotNull(facts);
            Assert.IsTrue(facts.Any());

            return new Tuple<AddSubscriberResult, IEnumerable<FactSubscription>>(result, facts);
        }
        #endregion

        #region WasAdded

        [TestMethod]
        public void AddSubscriber_Email_E_WasAdded_Test()
        {
            var result = WasAdded(ChannelType.SmsMms, MatchBy.Email, email: NotExistEmail);

            var fact = result.Item2.SingleOrDefault();
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.Email, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(NotExistEmail, subscriber.Email);
            Assert.AreEqual(NotExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(fact.DomainId, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);

            Assert.AreEqual(SubscriberIp, subscriber.Ip);
            Assert.AreEqual(SubscriberLastname, subscriber.Lastname);
            Assert.AreEqual(SubscriberFirstname, subscriber.Firstname);
        }

        [TestMethod]
        public void AddSubscriber_Email_EP0_WasAdded_Test()
        {
            var result = WasAdded(ChannelType.SmsMms, MatchBy.Email, email: NotExistEmail, phone: string.Empty);

            var fact = result.Item2.SingleOrDefault();
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.Email, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(NotExistEmail, subscriber.Email);
            Assert.AreEqual(NotExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(fact.DomainId, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);

            Assert.IsNull(subscriber.Phone);
            Assert.IsNull(subscriber.PhoneLocal);
            Assert.IsNull(subscriber.DialingPrefix);

            Assert.AreEqual(SubscriberIp, subscriber.Ip);
            Assert.AreEqual(SubscriberLastname, subscriber.Lastname);
            Assert.AreEqual(SubscriberFirstname, subscriber.Firstname);
        }

        [TestMethod]
        public void AddSubscriber_Email_EP1_WasAdded_Test()
        {
            var result = WasAdded(ChannelType.SmsMms, MatchBy.Email, email: NotExistEmail, phone: " ");

            var fact = result.Item2.SingleOrDefault();
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.Email, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(NotExistEmail, subscriber.Email);
            Assert.AreEqual(NotExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(fact.DomainId, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);

            Assert.IsNull(subscriber.Phone);
            Assert.IsNull(subscriber.PhoneLocal);
            Assert.IsNull(subscriber.DialingPrefix);

            Assert.AreEqual(SubscriberIp, subscriber.Ip);
            Assert.AreEqual(SubscriberLastname, subscriber.Lastname);
            Assert.AreEqual(SubscriberFirstname, subscriber.Firstname);
        }

        [TestMethod]
        public void AddSubscriber_Phone_P_WasAdded_Test()
        {
            var result = WasAdded(ChannelType.SmsMms, MatchBy.Phone, phone: NotExistPhone);

            var fact = result.Item2.SingleOrDefault();
            if (fact != null)
            {
                Assert.AreEqual(fact.DomainId, 0);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.SmsMms, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(NotExistPhone, subscriber.Phone);
            Assert.IsTrue(NotExistPhone.EndsWith(subscriber.PhoneLocal));
            Assert.IsTrue(NotExistPhone.StartsWith(subscriber.DialingPrefix));

            Assert.AreEqual(SubscriberIp, subscriber.Ip);
            Assert.AreEqual(SubscriberLastname, subscriber.Lastname);
            Assert.AreEqual(SubscriberFirstname, subscriber.Firstname);
        }

        [TestMethod]
        public void AddSubscriber_Phone_E0P_WasAdded_Test()
        {
            var result = WasAdded(ChannelType.SmsMms, MatchBy.Phone, phone: NotExistPhone, email: string.Empty);

            var fact = result.Item2.SingleOrDefault();
            if (fact != null)
            {
                Assert.AreEqual(fact.DomainId, 0);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.SmsMms, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.IsNull(subscriber.Email);
            Assert.IsNull(subscriber.DomainId);
            Assert.IsNull(subscriber.FamilyDomainId);

            Assert.AreEqual(NotExistPhone, subscriber.Phone);
            Assert.IsTrue(NotExistPhone.EndsWith(subscriber.PhoneLocal));
            Assert.IsTrue(NotExistPhone.StartsWith(subscriber.DialingPrefix));

            Assert.AreEqual(SubscriberIp, subscriber.Ip);
            Assert.AreEqual(SubscriberLastname, subscriber.Lastname);
            Assert.AreEqual(SubscriberFirstname, subscriber.Firstname);
        }

        [TestMethod]
        public void AddSubscriber_Phone_E1P_WasAdded_Test()
        {
            var result = WasAdded(ChannelType.SmsMms, MatchBy.Phone, phone: NotExistPhone, email: " ");

            var fact = result.Item2.SingleOrDefault();
            if (fact != null)
            {
                Assert.AreEqual(fact.DomainId, 0);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.SmsMms, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.IsNull(subscriber.Email);
            Assert.IsNull(subscriber.DomainId);
            Assert.IsNull(subscriber.FamilyDomainId);

            Assert.AreEqual(NotExistPhone, subscriber.Phone);
            Assert.IsTrue(NotExistPhone.EndsWith(subscriber.PhoneLocal));
            Assert.IsTrue(NotExistPhone.StartsWith(subscriber.DialingPrefix));

            Assert.AreEqual(SubscriberIp, subscriber.Ip);
            Assert.AreEqual(SubscriberLastname, subscriber.Lastname);
            Assert.AreEqual(SubscriberFirstname, subscriber.Firstname);
        }

        [TestMethod]
        public void AddSubscriber_Phone_E3P_WasAdded_Test()
        {
            var result = WasAdded(ChannelType.SmsMms, MatchBy.Phone, phone: NotExistPhone, email: "   ");

            var fact = result.Item2.SingleOrDefault();
            if (fact != null)
            {
                Assert.AreEqual(fact.DomainId, 0);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.SmsMms, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.IsNull(subscriber.Email);
            Assert.IsNull(subscriber.DomainId);
            Assert.IsNull(subscriber.FamilyDomainId);

            Assert.AreEqual(NotExistPhone, subscriber.Phone);
            Assert.IsTrue(NotExistPhone.EndsWith(subscriber.PhoneLocal));
            Assert.IsTrue(NotExistPhone.StartsWith(subscriber.DialingPrefix));

            Assert.AreEqual(SubscriberIp, subscriber.Ip);
            Assert.AreEqual(SubscriberLastname, subscriber.Lastname);
            Assert.AreEqual(SubscriberFirstname, subscriber.Firstname);
        }

        [TestMethod]
        public void AddSubscriber_Email_EP_WasAdded_Test()
        {
            var result = WasAdded(ChannelType.SmsMms, MatchBy.Email, email: NotExistEmail, phone: NotExistPhone);

            var facts = result.Item2;
            Assert.AreEqual(2, facts.Length);

            var fact = facts.FirstOrDefault(f => f.ChannelTypeId == ChannelType.Email);
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);
            }

            fact = facts.FirstOrDefault(f => f.ChannelTypeId == ChannelType.SmsMms);
            if (fact != null)
            {
                Assert.AreEqual(fact.DomainId, 0);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(NotExistPhone, subscriber.Phone);
            Assert.IsTrue(NotExistPhone.EndsWith(subscriber.PhoneLocal));
            Assert.IsTrue(NotExistPhone.StartsWith(subscriber.DialingPrefix));

            Assert.AreEqual(NotExistEmail, subscriber.Email);
            Assert.AreEqual(NotExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(3, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);

            Assert.AreEqual(SubscriberIp, subscriber.Ip);
            Assert.AreEqual(SubscriberLastname, subscriber.Lastname);
            Assert.AreEqual(SubscriberFirstname, subscriber.Firstname);
        }

        private Tuple<AddSubscriberResult, FactSubscription[]> WasAdded(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;
            FactSubscription[] facts = null;

            try
            {
                result = SM_AddAndUpdate(channelType, matchBy)
                    .AddSubscriber(
                        ActiveNotSubscribedListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
                        propertiesDictionary: new Dictionary<int, object>
                        {
                            {RequiredPropertyString, "jakiś blok"},
                            {RequiredPropertyInt, 7897987},
                            {DeletedProperty, "jakiś blok"}
                        }
                    );

                facts = LoadSubscriptions(result.SubscriberId, result.ListId).ToArray();
            }
            catch (SubscriberManagerException ex)
            {
                if (ifErrorConditition == null)
                    ifErrorConditition = exx => !(exx.FieldsErrors.Any() || exx.PropertiesErrors.Any());

                if ( ifErrorConditition(ex))
                    throw ex;
            }

            Assert.IsNotNull(result);
            if (result != null)
            {
                Assert.AreEqual(true, result.WasAdded && result.SubscriberId > 0 && result.ListId == ActiveNotSubscribedListId);
            }

            Assert.IsNotNull(facts);
            Assert.AreNotEqual(0, facts.Length);

            return new Tuple<AddSubscriberResult, FactSubscription[]>(result, facts);
        }
        #endregion

        #region ReplaceSubscriber

        [TestMethod]
        public void AddSubscriber_Email_E_ReplaceSubscriber_Test()
        {
            var result = ReplaceSubscriber(
                ChannelType.Email,
                MatchBy.Email,
                email: ExistEmail
            );

            var fact = result.Item2;
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.Email, fact.ChannelTypeId);
            }
            
            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(ExistEmail, subscriber.Email);
            Assert.AreEqual(ExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(fact.DomainId, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);
            
            Assert.IsNull(subscriber.Phone);
            Assert.IsNull(subscriber.PhoneLocal);
            Assert.IsNull(subscriber.DialingPrefix);
        }

        [TestMethod]
        public void AddSubscriber_Phone_P_ReplaceSubscriber_Test()
        {
            var result = ReplaceSubscriber(
                ChannelType.Email,
                MatchBy.Phone,
                phone: ExistPhone
            );

            var fact = result.Item2;
            if (fact != null)
            {
                Assert.AreEqual(fact.DomainId, 0);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.SmsMms, fact.ChannelTypeId);
            }
            
            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.IsNull(subscriber.Email);
            Assert.IsNull(subscriber.EmailMd5);
            Assert.IsNull(subscriber.DomainId);
            Assert.IsNull(subscriber.FamilyDomainId);

            Assert.AreEqual(ExistPhone, subscriber.Phone);
            Assert.IsTrue(ExistPhone.EndsWith(subscriber.PhoneLocal));
            Assert.IsTrue(ExistPhone.StartsWith(subscriber.DialingPrefix));
        }

        private Tuple<AddSubscriberResult, FactSubscription> ReplaceSubscriber(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;
            FactSubscription fact = null;

            try
            {
                result = SM_AddAndReplace(channelType, matchBy)
                    .AddSubscriber(
                        ActiveNotSubscribedListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
                        propertiesDictionary: new Dictionary<int, object>
                        {
                            {RequiredPropertyString, "jakiś blok"},
                            {RequiredPropertyInt, 7897987},
                            {DeletedProperty, "jakiś blok"}
                        }
                    );

                fact = LoadSubscriptions(result.SubscriberId, result.ListId).FirstOrDefault();
            }
            catch (SubscriberManagerException ex)
            { }

            Assert.IsNotNull(result);
            if (result != null)
            {
                Assert.AreEqual(true, result.WasAddedToList);
            }

            Assert.IsNotNull(fact);

            return new Tuple<AddSubscriberResult, FactSubscription>(result, fact);
        }
        #endregion

        #region DeleteNotExists

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteSubscriber_Email_NotExists_Test()
        {
            var result = DeleteNotExists(ChannelType.Email, subscriberId: NotExistSubscriberId, listId: ActiveSubscribedListId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteSubscriber_Phone_NotExists_Test()
        {
            var result = DeleteNotExists(ChannelType.SmsMms, subscriberId: NotExistSubscriberId, listId: ActiveSubscribedListId);
        }

        private List<RemovalModel> DeleteNotExists(ChannelType channelType, int subscriberId, int listId, Guid? messageGuid = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            List<RemovalModel> result = null;
            var subscriberManager = Container.GetInstance<SubscriberManager>();

            try
            {
                result = subscriberManager
                    .DeleteSubscriber(
                        subscriberId: subscriberId,
                        messageGuid: messageGuid,
                        listId: listId,
                        serviceId: CurrentUnitId,
                        reason: UnsubscribeReason.Api,
                        channelType: channelType,
                        subscriberRequestInformation: new SubscriberRequestInformation
                        {
                            Browser = null,
                            BrowserVersion = null,
                            Device = null,
                            DeviceVersion = null,
                            Environment = null,
                            EnvironmentVersion = null,
                            RenderEngine = null,
                            IsMobile = false,
                            ClientLanguage = null,
                            ClientEmailDomain = null
                        }
                    );
            }
            catch (SubscriberManagerException ex)
            {
                //if (ifErrorConditition == null)
                //{
                //    ifErrorConditition = exx =>
                //        exx.PropertiesErrors.SingleOrDefault(e => e.Key == RequiredPropertyInt).Value.Contains(string.Format(Resources.SubscriberManager.InvalidValue, "cecha liczbowa xx3"));
                //}

                //if (ifErrorConditition(ex))
                //    throw ex;
            }

            return result;
        }
        #endregion
        
        #region DeleteExist

        [TestMethod]
        public void DeleteSubscriber_Email_Exist_Test()
        {
            var result = DeleteExist(channelType: ChannelType.Email, subscriberId: ExistSubscriberId, listId: ActiveSubscribedListId);
        }

        [TestMethod]
        public void DeleteSubscriber_Phone_Exist_Test()
        {
            var result = DeleteExist(channelType: ChannelType.SmsMms, subscriberId: ExistSubscriberId, listId: ActiveSubscribedListId);
        }

        private List<RemovalModel> DeleteExist(ChannelType channelType, int subscriberId, int listId, Guid? messageGuid = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            List<RemovalModel> result = null;

            var subscriberManager = Container.GetInstance<SubscriberManager>();

            try
            {
                result = subscriberManager
                    .DeleteSubscriber(
                       subscriberId: subscriberId,
                        messageGuid: messageGuid,
                        listId: listId,
                        serviceId: CurrentUnitId,
                        reason: UnsubscribeReason.Api,
                        channelType: channelType,
                        subscriberRequestInformation: new SubscriberRequestInformation
                        {
                            Browser = null,
                            BrowserVersion = null,
                            Device = null,
                            DeviceVersion = null,
                            Environment = null,
                            EnvironmentVersion = null,
                            RenderEngine = null,
                            IsMobile = false,
                            ClientLanguage = null,
                            ClientEmailDomain = null
                        }
                    );
            }
            catch (SubscriberManagerException ex)
            { 
                //if (ifErrorConditition == null)
                //{
                //    ifErrorConditition = exx =>
                //        exx.PropertiesErrors.SingleOrDefault(e => e.Key == RequiredPropertyInt).Value.Contains(string.Format(Resources.SubscriberManager.InvalidValue, "cecha liczbowa xx3"));
                //}

                //if (ifErrorConditition(ex))
                //    throw ex;
            }

            Assert.IsNotNull(result);
            if (result != null)
            {
                Assert.AreEqual(1, result.Count(r => r.ListId == ActiveSubscribedListId));
            }

            return result;
        }

        #endregion

        #region ChangeContacts

        [TestMethod]
        public void AddSubscriber_Email_I_ChangeContacts_NotSubscribed_Test()
        {
            var result = ChangeContacts(false, ChannelType.SmsMms, MatchBy.Id, subscriberId: ExistSubscriberId, email: NotExistEmail);

            Assert.AreEqual(NotExistEmail, result.Item1.SubscriberEmail);

            var facts = result.Item2;
            Assert.AreEqual(1, facts.Length);

            var fact = facts.FirstOrDefault(f => f.ChannelTypeId == ChannelType.Email);
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.Email, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(NotExistEmail, subscriber.Email);
            Assert.AreEqual(NotExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(fact.DomainId, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);
        }

        [TestMethod]
        public void AddSubscriber_Email_D_ChangeContacts_NotSubscribed_Test()
        {
            var result = ChangeContacts(false, ChannelType.SmsMms, MatchBy.Email, emailMd5: ExistMd5, email: NotExistEmail);

            Assert.AreEqual(NotExistEmail, result.Item1.SubscriberEmail);

            var facts = result.Item2;
            Assert.AreEqual(1, facts.Length);

            var fact = facts.FirstOrDefault(f => f.ChannelTypeId == ChannelType.Email);
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.Email, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(NotExistEmail, subscriber.Email);
            Assert.AreEqual(NotExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(fact.DomainId, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);
        }

        [TestMethod]
        public void AddSubscriber_Custom_EP_ChangeContacts_NotSubscribed_Test()
        {
            var result = ChangeContacts(false, ChannelType.SmsMms, MatchBy.CustomSubscriberId, customSubscriberId: ExistCustomSubscriberId, email: NotExistEmail, phone: NotExistPhone);

            Assert.AreEqual(NotExistEmail, result.Item1.SubscriberEmail);

            var facts = result.Item2;
            Assert.AreEqual(2, facts.Length);

            var fact = facts.FirstOrDefault(f => f.ChannelTypeId == ChannelType.Email);
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);
            }

            fact = facts.FirstOrDefault(f => f.ChannelTypeId == ChannelType.SmsMms);
            if (fact != null)
            {
                Assert.AreEqual(fact.DomainId, 0);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);

            Assert.AreEqual(NotExistEmail, subscriber.Email);
            Assert.AreEqual(NotExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(3, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);

            Assert.AreEqual(NotExistPhone, subscriber.Phone);
            Assert.IsTrue(NotExistPhone.EndsWith(subscriber.PhoneLocal));
            Assert.IsTrue(NotExistPhone.StartsWith(subscriber.DialingPrefix));
        }
        [TestMethod]
        public void AddSubscriber_Email_I_ChangeContacts_Subscribed_Test()
        {
            var result = ChangeContacts(true, ChannelType.SmsMms, MatchBy.Id, subscriberId: ExistSubscriberId, email: NotExistEmail);

            Assert.AreEqual(NotExistEmail, result.Item1.SubscriberEmail);

            var facts = result.Item2;
            Assert.AreEqual(0, facts.Length);

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(NotExistEmail, subscriber.Email);
            Assert.AreEqual(NotExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(1000, subscriber.FamilyDomainId);
        }

        [TestMethod]
        public void AddSubscriber_Email_D_ChangeContacts_Subscribed_Test()
        {
            var result = ChangeContacts(true, ChannelType.SmsMms, MatchBy.Email, emailMd5: ExistMd5, email: NotExistEmail);

            Assert.AreEqual(NotExistEmail, result.Item1.SubscriberEmail);

            var facts = result.Item2;
            Assert.AreEqual(0, facts.Length);

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(NotExistEmail, subscriber.Email);
            Assert.AreEqual(NotExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(1000, subscriber.FamilyDomainId);
        }

        [TestMethod]
        public void AddSubscriber_Custom_EP_ChangeContacts_Subscribed_Test()
        {
            var result = ChangeContacts(true, ChannelType.SmsMms, MatchBy.CustomSubscriberId, customSubscriberId: ExistCustomSubscriberId, email: NotExistEmail, phone: NotExistPhone);

            Assert.AreEqual(NotExistEmail, result.Item1.SubscriberEmail);

            var facts = result.Item2;
            Assert.AreEqual(0, facts.Length);

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);

            Assert.AreEqual(NotExistEmail, subscriber.Email);
            Assert.AreEqual(NotExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(3, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);

            Assert.AreEqual(NotExistPhone, subscriber.Phone);
            Assert.IsTrue(NotExistPhone.EndsWith(subscriber.PhoneLocal));
            Assert.IsTrue(NotExistPhone.StartsWith(subscriber.DialingPrefix));
        }

        private Tuple<AddSubscriberResult, FactSubscription[]> ChangeContacts(bool addToSubscribed, ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;
            FactSubscription[] facts = null;

            var listId = addToSubscribed ? ActiveSubscribedListId : ActiveNotSubscribedListId;

            try
            {
                result = SM_AddAndUpdate(channelType, matchBy)
                    .AddSubscriber(
                        listId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
                        propertiesDictionary: new Dictionary<int, object>
                        {
                            {RequiredPropertyString, "jakiś blok"},
                            {RequiredPropertyInt, 7897987},
                            {DeletedProperty, "jakiś blok"}
                        }
                    );

                facts = LoadSubscriptions(result.SubscriberId, result.ListId).ToArray();
            }
            catch (SubscriberManagerException ex)
            {
                if (ifErrorConditition(ex))
                    throw ex;
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(true, (addToSubscribed || result.WasAddedToList) && result.SubscriberId == ExistSubscriberId && result.ListId == listId);

            Assert.IsNotNull(facts);

            return new Tuple<AddSubscriberResult, FactSubscription[]>(result, facts);
        }
        #endregion

        #region ChangeSubscriberData

        [TestMethod]
        public void AddSubscriber_Email_Update_ChangeSubscriberData_Test()
        {
            var result = ChangeSubscriberData(true, ChannelType.SmsMms, MatchBy.Email, email: ExistEmail);

            Assert.AreEqual(ExistEmail, result.Item1.SubscriberEmail);

           var facts = result.Item2;
            Assert.AreEqual(1, facts.Length);

            var fact = facts.FirstOrDefault(f => f.ChannelTypeId == ChannelType.Email);
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.Email, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(ExistEmail, subscriber.Email);
            Assert.AreEqual(ExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(fact.DomainId, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);

            Assert.AreEqual(SubscriberFirstname, subscriber.Firstname);
            Assert.AreEqual("Nowacki", subscriber.Lastname);
        }

        [TestMethod]
        public void AddSubscriber_Email_Replace_ChangeSubscriberData_Test()
        {
            var result = ChangeSubscriberData(false, ChannelType.SmsMms, MatchBy.Email, email: ExistEmail);

            Assert.AreEqual(ExistEmail, result.Item1.SubscriberEmail);

            var facts = result.Item2;
            Assert.AreEqual(1, facts.Length);

            var fact = facts.FirstOrDefault(f => f.ChannelTypeId == ChannelType.Email);
            if (fact != null)
            {
                Assert.AreEqual(3, fact.DomainId);
                Assert.AreEqual(SubscriberIp, fact.ClientIp);
                Assert.AreEqual(SubscriptionSource.Api, fact.SubscriptionSourceId);

                Assert.AreEqual(ChannelType.Email, fact.ChannelTypeId);
            }

            var subscriber = LoadSubscriber(result.Item1.SubscriberId);
            Assert.IsNotNull(subscriber);

            Assert.AreEqual(ExistEmail, subscriber.Email);
            Assert.AreEqual(ExistMd5, CryptographyHelper.BytesToHexString(subscriber.EmailMd5).ToUpper());
            Assert.AreEqual(fact.DomainId, subscriber.DomainId);
            Assert.AreEqual(1000, subscriber.FamilyDomainId);

            Assert.IsNull(subscriber.Firstname);
            Assert.AreEqual("Nowacki", subscriber.Lastname);
        }

        private Tuple<AddSubscriberResult, FactSubscription[]> ChangeSubscriberData(bool isUpdate, ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;
            FactSubscription[] facts = null;

            var sm = isUpdate ? SM_AddAndUpdate(channelType, matchBy) : SM_AddAndReplace(channelType, matchBy);

            try
            {
                result = sm
                    .AddSubscriber(
                        ActiveNotSubscribedListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: "",
                        lastName: "Nowacki",
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
                        propertiesDictionary: new Dictionary<int, object>
                        {
                            {RequiredPropertyString, "jakiś blok"},
                            {RequiredPropertyInt, 7897987},
                            {DeletedProperty, "jakiś blok"}
                        }
                    );

                facts = LoadSubscriptions(result.SubscriberId, result.ListId).ToArray();
            }
            catch (SubscriberManagerException ex)
            {
                //if (ifErrorConditition(ex))
                    throw ex;
            }

            Assert.IsNotNull(result);
            if (result != null)
            {
                Assert.AreEqual(true, result.WasAddedToList && result.SubscriberId == ExistSubscriberId && result.ListId == ActiveNotSubscribedListId);
            }

            Assert.IsNotNull(facts);
            Assert.AreNotEqual(0, facts.Length);

            return new Tuple<AddSubscriberResult, FactSubscription[]>(result, facts);
        }
        #endregion

        #region Blacklisted_AddToNewList
        
        /// <summary>
        /// Oba kanały są zblaklistowane globalnie
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_strictE_strictP_Blacklisted_AddToNewList_Test()
        {
            var blackListEmailDao = Container.GetInstance<IBlacklistedGlobalEmailDao>();
            blackListEmailDao.Insert(new InsertBuilder("ES_TR_Common.dbo.BlacklistedGlobalEmail"), new ValuesBuilder(new
            {
                Id = 0,
                CreatedOn = DateTime.Now,
                Email = ExistEmail,
                ExpirationDate = default(DateTime?),
                Reason = "bo tak",
                Source = BlacklistingSource.Ui
            }));

            var blackListPhoneDao = Container.GetInstance<IBlacklistedGlobalPhoneDao>();
            blackListPhoneDao.Insert(new InsertBuilder("ES_TR_Common.dbo.BlacklistedGlobalPhone"), new ValuesBuilder(new
            {
                Id = 0,
                CreatedOn = DateTime.Now,
                Phone = ExistPhone,
                ExpirationDate = default(DateTime?),
                Reason = "bo tak",
                Source = BlacklistingSource.Ui
            }));

            Blacklisted_AddToNewList(ChannelType.SmsMms, MatchBy.Id, subscriberId: ExistSubscriberId, email: ExistEmail, phone: ExistPhone, ifErrorConditition: ex =>
                ex.FieldsErrors.Count == 2
                && !ex.PropertiesErrors.Any()
                && ex.FieldsErrors.Any(e => e.Key == "email" && e.Value.Contains(Resources.SubscriberManager.EmailGlobalBlacklist))
                && ex.FieldsErrors.Any(e => e.Key == "phone" && e.Value.Contains(Resources.SubscriberManager.PhoneGlobalBlacklist))
            );
        }
        
        /// <summary>
        /// Oba kanały są zblaklistowane globalnie; Email jest zblaklistowany przez domenę
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SubscriberManagerException))]
        public void AddSubscriber_domainE_strictP_Blacklisted_AddToNewList_Test()
        {
            var blackListDomainDao = Container.GetInstance<IBlacklistedGlobalDomainDao>();
            blackListDomainDao.Insert(new InsertBuilder("ES_TR_Common.dbo.BlacklistedGlobalDomain"), new ValuesBuilder(new
            {
                Id = 0,
                CreatedOn = DateTime.Now,
                Domain = ExistEmail.Substring(ExistEmail.IndexOf('@') + 1),
                Reason = "bo tak",
                Source = BlacklistingSource.Ui
            }));

            var blackListPhoneDao = Container.GetInstance<IBlacklistedGlobalPhoneDao>();
            blackListPhoneDao.Insert(new InsertBuilder("ES_TR_Common.dbo.BlacklistedGlobalPhone"), new ValuesBuilder(new
            {
                Id = 0,
                CreatedOn = DateTime.Now,
                Phone = ExistPhone,
                ExpirationDate = default(DateTime?),
                Reason = "bo tak",
                Source = BlacklistingSource.Ui
            }));

            Blacklisted_AddToNewList(ChannelType.SmsMms, MatchBy.Id, subscriberId: ExistSubscriberId, email: ExistEmail, phone: ExistPhone, ifErrorConditition: ex =>
                ex.FieldsErrors.Count == 2
                && !ex.PropertiesErrors.Any()
                && ex.FieldsErrors.Any(e => e.Key == "email" && e.Value.Contains(Resources.SubscriberManager.EmailGlobalBlacklist))
                && ex.FieldsErrors.Any(e => e.Key == "phone" && e.Value.Contains(Resources.SubscriberManager.PhoneGlobalBlacklist))
            );
        }
        
        /// <summary>
        /// Tylko Phone jest zblaklistowany globalnie
        /// Sprawdzenie czy można dodać do listy dla kanału Email, ale nie można dodać Phone
        /// </summary>
        [TestMethod]
        public void AddSubscriber_strictP_Blacklisted_AddToNewList_Test()
        {
            var blackListPhoneDao = Container.GetInstance<IBlacklistedGlobalPhoneDao>();
            blackListPhoneDao.Insert(new InsertBuilder("ES_TR_Common.dbo.BlacklistedGlobalPhone"), new ValuesBuilder(new
            {
                Id = 0,
                CreatedOn = DateTime.Now,
                Phone = ExistPhone,
                ExpirationDate = default(DateTime?),
                Reason = "bo tak",
                Source = BlacklistingSource.Ui
            }));

            var result = Blacklisted_AddToNewList(ChannelType.SmsMms, MatchBy.Id, subscriberId: ExistSubscriberId, email: ExistEmail, phone: ExistPhone);

            Assert.AreEqual(true, result.WasAddedToList);
            Assert.AreEqual(ExistEmail, result.SubscriberEmail);
            Assert.AreEqual(result.SubscriberId, ExistSubscriberId);
            Assert.AreEqual(result.ListId, ActiveNotSubscribedListId);

            var subscriptions = LoadSubscriptions(result.SubscriberId, result.ListId);
            Assert.IsNotNull(subscriptions);
            Assert.AreEqual(1, subscriptions.Count());
            Assert.AreEqual(ChannelType.Email, subscriptions.Single().ChannelTypeId);
        }
        
        /// <summary>
        /// Tylko Phone jest zblaklistowany globalnie
        /// Sprawdzenie czy można dodać subskrybenta, czy można dodać do listy dla kanału Email, ale nie można dodać Phone
        /// </summary>
        [TestMethod]
        public void AddSubscriber_matchE_strictP_NewSubscriber_Blacklisted_AddToNewList_Test()
        {
            var blackListPhoneDao = Container.GetInstance<IBlacklistedGlobalPhoneDao>();
            blackListPhoneDao.Insert(new InsertBuilder("ES_TR_Common.dbo.BlacklistedGlobalPhone"), new ValuesBuilder(new
            {
                Id = 0,
                CreatedOn = DateTime.Now,
                Phone = NotExistPhone,
                ExpirationDate = default(DateTime?),
                Reason = "bo tak",
                Source = BlacklistingSource.Ui
            }));

            var result = Blacklisted_AddToNewList(ChannelType.SmsMms, MatchBy.Email, email: NotExistEmail, phone: NotExistPhone);

            Assert.AreEqual(result.WasAdded, true);
            Assert.AreEqual(true, result.WasAddedToList);
            Assert.AreEqual(NotExistEmail, result.SubscriberEmail);
            Assert.AreEqual(result.ListId, ActiveNotSubscribedListId);

            //Assert.AreEqual(result.SubscriberId, ExistSubscriberId);

            var subscriber = LoadSubscriber(result.SubscriberId);

            Assert.AreEqual(NotExistEmail, subscriber.Email);
            Assert.AreEqual(NotExistPhone, subscriber.Phone);

            var subscriptions = LoadSubscriptions(result.SubscriberId, result.ListId);
            Assert.IsNotNull(subscriptions);
            Assert.AreEqual(1, subscriptions.Count());
            Assert.AreEqual(ChannelType.Email, subscriptions.Single().ChannelTypeId);
        }
        
        /// <summary>
        /// Tylko Phone jest zblaklistowany globalnie
        /// Sprawdzenie czy można zmienić Email
        /// </summary>
        [TestMethod]
        public void AddSubscriber_matchP_strictP_ChangeEmail_Blacklisted_AddToNewList_Test()
        {
            var blackListPhoneDao = Container.GetInstance<IBlacklistedGlobalPhoneDao>();
            blackListPhoneDao.Insert(new InsertBuilder("ES_TR_Common.dbo.BlacklistedGlobalPhone"), new ValuesBuilder(new
            {
                Id = 0,
                CreatedOn = DateTime.Now,
                Phone = ExistPhone,
                ExpirationDate = default(DateTime?),
                Reason = "bo tak",
                Source = BlacklistingSource.Ui
            }));

            var result = Blacklisted_AddToNewList(ChannelType.SmsMms, MatchBy.Phone, email: NotExistEmail, phone: ExistPhone);

            Assert.AreEqual(true, result.WasAddedToList);
            Assert.AreEqual(NotExistEmail, result.SubscriberEmail);
            Assert.AreEqual(result.ListId, ActiveNotSubscribedListId);

            var subscriber = LoadSubscriber(result.SubscriberId);

            Assert.AreEqual(ExistPhone, subscriber.Phone);
            Assert.AreEqual(NotExistEmail, subscriber.Email);

            var subscriptions = LoadSubscriptions(result.SubscriberId, result.ListId);
            Assert.IsNotNull(subscriptions);
            Assert.AreEqual(1, subscriptions.Count());
            Assert.AreEqual(ChannelType.Email, subscriptions.Single().ChannelTypeId);
        }

        private AddSubscriberResult Blacklisted_AddToNewList(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;

            try
            {
                result = SM_AddAndIgnore(channelType, matchBy)
                    .AddSubscriber(
                        ActiveNotSubscribedListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
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
                if (ifErrorConditition == null)
                {
                    ifErrorConditition = exx =>
                        exx.PropertiesErrors.Any(e => e.Key == RequiredPropertyInt && e.Value.Contains(string.Format(Resources.SubscriberManager.InvalidValue, "cecha liczbowa xx3")));
                }

                if (ifErrorConditition(ex))
                    throw ex;
            }

            return result;
        }
        #endregion

        #region Blacklisted_AddToExistList
        
        /// <summary>
        /// Tylko Phone jest zblaklistowany globalnie
        /// Sprawdzenie czy nie usunie Phone z subskrypcji
        /// </summary>
        [TestMethod]
        public void AddSubscriber_strictP_Blacklisted_AddToExistList_Test()
        {
            var blackListPhoneDao = Container.GetInstance<IBlacklistedGlobalPhoneDao>();
            blackListPhoneDao.Insert(new InsertBuilder("ES_TR_Common.dbo.BlacklistedGlobalPhone"), new ValuesBuilder(new
            {
                Id = 0,
                CreatedOn = DateTime.Now,
                Phone = ExistPhone,
                ExpirationDate = default(DateTime?),
                Reason = "bo tak",
                Source = BlacklistingSource.Ui
            }));

            var result = Blacklisted_AddToExistList(ChannelType.SmsMms, MatchBy.Id, subscriberId: ExistSubscriberId, email: NotExistEmail, phone: NotExistPhone);

            Assert.AreEqual(result.WasAlreadyOnList, true);
            Assert.AreEqual(result.RemovedChannels, ChannelType.NotSet);
        }
        
        /// <summary>
        /// Tylko Phone jest zblaklistowany globalnie
        /// Sprawdzenie czy można zmienić Email
        /// </summary>
        [TestMethod]
        public void AddSubscriber_matchP_strictP_ChangeEmail_Blacklisted_AddToExistList_Test()
        {
            var blackListPhoneDao = Container.GetInstance<IBlacklistedGlobalPhoneDao>();
            blackListPhoneDao.Insert(new InsertBuilder("ES_TR_Common.dbo.BlacklistedGlobalPhone"), new ValuesBuilder(new
            {
                Id = 0,
                CreatedOn = DateTime.Now,
                Phone = ExistPhone,
                ExpirationDate = default(DateTime?),
                Reason = "bo tak",
                Source = BlacklistingSource.Ui
            }));

            var result = Blacklisted_AddToExistList(ChannelType.SmsMms, MatchBy.Phone, email: NotExistEmail, phone: ExistPhone);

            Assert.AreEqual(result.WasAlreadyOnList, true);
            Assert.AreEqual(NotExistEmail, result.SubscriberEmail);
            Assert.AreEqual(result.ListId, ActiveSubscribedListId);

            var subscriber = LoadSubscriber(result.SubscriberId);
            Assert.AreEqual(ExistPhone, subscriber.Phone);
            Assert.AreEqual(NotExistEmail, subscriber.Email);

            var subscriptions = LoadSubscriptions(result.SubscriberId, result.ListId);
            Assert.IsNotNull(subscriptions);
            Assert.AreEqual(0, subscriptions.Count());
        }
        
        /// <summary>
        /// Tylko Phone jest zblaklistowany globalnie
        /// Sprawdzenie czy usunie subskrypcję Phone i zmieni subskrypcję Email
        /// </summary>
        [TestMethod]
        public void AddSubscriber_matchP_strictP_Unubscribebliacklisted_Blacklisted_AddToExistList_Test()
        {
            var blackListPhoneDao = Container.GetInstance<IBlacklistedGlobalPhoneDao>();
            blackListPhoneDao.Insert(new InsertBuilder("ES_TR_Common.dbo.BlacklistedGlobalPhone"), new ValuesBuilder(new
            {
                Id = 0,
                CreatedOn = DateTime.Now,
                Phone = NotExistPhone,
                ExpirationDate = default(DateTime?),
                Reason = "bo tak",
                Source = BlacklistingSource.Ui
            }));

            var result = Blacklisted_AddToExistList(ChannelType.SmsMms, MatchBy.Id, subscriberId: ExistSubscriberId, email: NotExistEmail, phone: NotExistPhone);

            Assert.AreEqual(result.WasAlreadyOnList, true);
            Assert.AreEqual(NotExistEmail, result.SubscriberEmail);
            Assert.AreEqual(result.ListId, ActiveSubscribedListId);

            var subscriber = LoadSubscriber(result.SubscriberId);
            Assert.AreEqual(subscriber.Phone, NotExistPhone);
            Assert.AreEqual(NotExistEmail, subscriber.Email);

            Assert.AreEqual(result.RemovedChannels & ChannelType.SmsMms, ChannelType.SmsMms);

            var subscriptions = LoadSubscriptions(result.SubscriberId, result.ListId);
            Assert.IsNotNull(subscriptions);
            Assert.AreEqual(0, subscriptions.Count());
        }

        private AddSubscriberResult Blacklisted_AddToExistList(ChannelType channelType, MatchBy matchBy, int? subscriberId = null, string email = null, string emailMd5 = null, string phone = null, string customSubscriberId = null, Func<SubscriberManagerException, bool> ifErrorConditition = null)
        {
            AddSubscriberResult result = null;

            try
            {
                result = SM_AddAndReplace(channelType, matchBy)
                    .AddSubscriber(
                        ActiveSubscribedListId,
                        id: subscriberId,
                        email: email,
                        emailMd5: emailMd5,
                        phone: phone,
                        customSubscriberId: customSubscriberId,
                        firstName: SubscriberFirstname,
                        lastName: SubscriberLastname,
                        trackingCode: null,
                        vendor: null,
                        ip: SubscriberIp,
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
                if (ifErrorConditition == null)
                {
                    ifErrorConditition = exx =>
                        exx.PropertiesErrors.Any(e => e.Key == RequiredPropertyInt && e.Value.Contains(string.Format(Resources.SubscriberManager.InvalidValue, "cecha liczbowa xx3")));
                }

                if (ifErrorConditition(ex))
                    throw ex;
            }

            return result;
        }
        #endregion

        #region GetSubscriberManager

        private SubscriberManager SM_AddAndIgnore(ChannelType channelType, MatchBy matchBy)
        {
            var subscriberManager = Container.GetInstance<SubscriberManager>();
            return subscriberManager
                .PreAddSubscriber(
                    CurrentUnitId,
                    SubscriberManagerMode.AddAndIgnore,
                    SubscriptionSource.Api,
                    matchBy: matchBy,
                    enabledChannels: channelType,
                    forceConfirmation: false,
                    addUnsubscribed: false,
                    addUnsubscribedByUser: false,
                    subscriptionLanguage: "en-US"
                );
        }

        private SubscriberManager SM_AddAndUpdate(ChannelType channelType, MatchBy matchBy)
        {
            var subscriberManager = Container.GetInstance<SubscriberManager>();
            return subscriberManager
                .PreAddSubscriber(
                    CurrentUnitId,
                    SubscriberManagerMode.AddAndUpdate,
                    SubscriptionSource.Api,
                    matchBy: matchBy,
                    enabledChannels: channelType,
                    forceConfirmation: false,
                    addUnsubscribed: false,
                    addUnsubscribedByUser: false,
                    subscriptionLanguage: "en-US"
                );
        }

        private SubscriberManager SM_AddAndUpdate_AddUnsubscribed(ChannelType channelType, MatchBy matchBy)
        {
            var subscriberManager = Container.GetInstance<SubscriberManager>();
            return subscriberManager
                .PreAddSubscriber(
                    CurrentUnitId,
                    SubscriberManagerMode.AddAndUpdate,
                    SubscriptionSource.Api,
                    matchBy: matchBy,
                    enabledChannels: channelType,
                    forceConfirmation: false,
                    addUnsubscribed: true,
                    addUnsubscribedByUser: false,
                    subscriptionLanguage: "en-US"
                );
        }

        private SubscriberManager SM_AddAndUpdate_AllowUnsubscribedByUser(ChannelType channelType, MatchBy matchBy)
        {
            var subscriberManager = Container.GetInstance<SubscriberManager>();
            return subscriberManager
                .PreAddSubscriber(
                    CurrentUnitId,
                    SubscriberManagerMode.AddAndUpdate,
                    SubscriptionSource.Api,
                    matchBy: matchBy,
                    enabledChannels: channelType,
                    forceConfirmation: false,
                    addUnsubscribed: false,
                    addUnsubscribedByUser: true,
                    subscriptionLanguage: "en-US"
                );
        }

        private SubscriberManager SM_AddAndReplace(ChannelType channelType, MatchBy matchBy)
        {
            var subscriberManager = Container.GetInstance<SubscriberManager>();
            return subscriberManager
                .PreAddSubscriber(
                    CurrentUnitId,
                    SubscriberManagerMode.AddAndReplace,
                    SubscriptionSource.Api,
                    matchBy: matchBy,
                    enabledChannels: channelType,
                    forceConfirmation: false,
                    addUnsubscribed: false,
                    addUnsubscribedByUser: false,
                    subscriptionLanguage: "en-US"
                );
        }
        #endregion

        #region Load ChangedData

        private IEnumerable<FactSubscription> LoadSubscriptions(int subscriberId, int listId)
        {
            var factSubscriptionsTable = $"[ES_DW_Unit_{CurrentUnitId}].[dbo].FactSubscriptions";
            var dao = Container.GetInstance<IDimensionDao>();

            var select = new SelectBuilder("Id,DateTimeId,DomainId,ListId,ClientIP,SubscriptionSourceId,ChannelTypeId")
                .From(factSubscriptionsTable)
                .Where(new {SubscriberId = subscriberId, ListId = listId})
                .Where("DateTimeId>={0}", new { DateTimeId = DateTime.Now.AddDays(-1)})// interesują nas tylko nowo dodane subskrypcje
                .MainClause;

            var results = dao.SelectMany((SelectBuilder)select).Select(f => new FactSubscription
            {
                Id = f.Id,
                DateTimeId = f.DateTimeId,
                DomainId = f.DomainId,
                ListId = f.ListId,
                ClientIp = f.ClientIP,
                SubscriptionSourceId = (SubscriptionSource)f.SubscriptionSourceId,
                ChannelTypeId = (ChannelType)f.ChannelTypeId,
            });

            return results.OrderByDescending(f => f.DateTimeId);
        }

        private class FactSubscription
        {
            public int Id { get; set; }
            public DateTime DateTimeId { get; set; }
            public int? DomainId { get; set; }
            public int ListId { get; set; }
            public string ClientIp { get; set; }
            public SubscriptionSource SubscriptionSourceId { get; set; }
            public ChannelType ChannelTypeId { get; set; }
        }

        private SubscribedOne LoadSubscriber(int subscriberId)
        {
            var dao = Container.GetInstance<ISubscriberDao>();

            var subscribedOne = dao.Find(s => new SubscribedOne
            {
                Id = s.Id,
                Email = s.Email,
                EmailMd5 = s.EmailMd5,
                Phone = s.Phone,
                PhoneLocal = s.PhoneLocal,
                DialingPrefix = s.DialingPrefix,
                Firstname = s.Firstname,
                Lastname = s.Lastname,
                Ip = s.Ip,
                DomainId = s.Domain.Id,
                FamilyDomainId = s.Domain.Family.Id
                
            }, s => s.Id == subscriberId).FirstOrDefault();
            return subscribedOne;
        }

        private class SubscribedOne
        {
            public int Id { get; set; }
            public string Ip { get; set; }
            public string Email { get; set; }
            public byte[] EmailMd5 { get; set; }
            public string Phone { get; set; }
            public string PhoneLocal { get; set; }
            public string DialingPrefix { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public int? DomainId { get; set; }
            public int? FamilyDomainId { get; set; }
        }
        #endregion
    }
}