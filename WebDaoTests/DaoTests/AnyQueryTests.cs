using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ExpertSender.Common.Dao;
using ExpertSender.Common.Extensions;
using ExpertSender.Common.QueryBuilder;
using ExpertSender.DataModel;
using ExpertSender.DataModel.Dao;
using ExpertSender.DataModel.Enums;
using ExpertSender.Lib;
using MaxMind.GeoIP2;
using NHibernate;
using WebDaoTests.Core;
using EsAppContext = WebDaoTests.Mocks.EsAppContext;

namespace WebDaoTests.DaoTests
{
    internal class AnyQueryTests : Tester
    {
        private const int CurrentUnitId = 1;

        public AnyQueryTests()
            : base(new EsAppContext { CurrentServiceId = CurrentUnitId })
        { }

        public override void Start()
        {
            //GetCacheStatusTest();
            //LoadListSettingDictionary();
            //LoadSubscribedOn();
            //ChangeSubscribedOn();
            //CompareSingleFirst();
            //SaveGeoinfo();
            //ExistsTest();
            LoadComplexObject();
        }

        private void GetCacheStatusTest()
        {
            var queryMd5 = new byte[16];
            var a = BitConverter.GetBytes(0xAB3423DF55E20F96);
            var b = BitConverter.GetBytes(0xC167BEABF5C4B9B2);
            Buffer.BlockCopy(a, 0, queryMd5, 0, a.Length);
            Buffer.BlockCopy(b, 0, queryMd5, 8, b.Length);
            queryMd5 = queryMd5.Reverse().ToArray();

            var ses = Container.GetInstance<ISession>();
            using (var connection = new SqlConnection(ses.Connection.ConnectionString))
            {
                connection.Open();

                var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    var status = GetCacheStatus(queryMd5, connection, transaction, 30);
                }
                catch (Exception)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception)
                    {
                        // ignorujemy błędy rollback
                        // jeśli się nie uda tzn, że np połączenie zostało już zamkniete
                    }
                }
            }
        }

        /// <summary>
        /// Metoda skopiowana z SegmentCacheDao.GetCacheStatus()
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private static int GetCacheStatus(byte[] hashKey, SqlConnection connection, SqlTransaction transaction, int timeout)
        {
            const string selectQueryStatus =
                @"SELECT
                    MIN(Status),
                    Id
                FROM (
                    SELECT Id, 1 as Status FROM SegmentCache WITH(READPAST) WHERE HashKey = @HashKey AND LastUpdate > DATEADD(MINUTE, ExpireTime, GETUTCDATE())
                    UNION ALL
                    SELECT Id, 2 as Status FROM SegmentCache WITH(NOLOCK) WHERE HashKey = @HashKey AND LastUpdate > DATEADD(MINUTE, ExpireTime, GETUTCDATE())
                ) AS SUB GROUP BY Id";

            var cmd = new SqlCommand
            {
                Connection = connection,
                Transaction = transaction,
                CommandTimeout = timeout,
                CommandText = selectQueryStatus
            };
            cmd.Parameters.AddWithValue("@HashKey", hashKey);
            var result = cmd.ExecuteScalar();

            return (int?) result ?? 3;
        }

        //to nie działa
        //private void LoadListSettingDictionary1()
        //{
        //    var listDao = Container.GetInstance<IListDao>();
        //    var lists = listDao.Find(l => l.Settings, l => l.Id == 8);
        //    var list = lists.Single();
        //}

        //to nie działa
        //private void LoadListSettingDictionary2()
        //{
        //    var listDao = Container.GetInstance<IListDao>();
        //    var lists = listDao.Find(l => l.Settings.Select(s => new { s.Key, s.Value }), l => l.Id == 8);
        //    var list = lists.Single();
        //}

        private void LoadSubscribedOn()
        {
            var subscribedOn = Container.GetInstance<IWebDao<SubscriberInList>>()
                    .Find(e => e.SubscribedOn, sil => sil.List.Id == 8 && sil.Subscriber.Id == 4238630)
                    .Single();
        }

        private void ChangeSubscribedOn()
        {
            var dao = Container.GetInstance<IWebDao<SubscriberInList>>();
            var subscriberInList = dao
                    .Find(e => e, sil => sil.List.Id == 8 && sil.Subscriber.Id == 4238630)
                    .Single();
            subscriberInList.ConfirmedOn = subscriberInList.ConfirmedOn.Value.AddSeconds(1);

            var dao2 = Container.GetInstance<ISubscriberDao>();
            var subscriber = dao2.Get(4238630);
            subscriber.LastMessage = subscriber.LastMessage.Value.AddSeconds(1);


            var ses = Container.GetInstance<ISession>();
            ses.Flush();
        }

        private void CompareSingleFirst()
        {
            var dao = Container.GetInstance<IWebDao<Subscriber>>();

            var subs = dao.Find(s => new
            {
                s.Id,
                s.Email,
                s.Firstname,
                s.Lastname
            }, s => s.Email == "andrzej@duda.pl");

            try
            {
                var single = subs.Single();
            }
            catch (Exception)
            { }

            try
            {
                var singleOrDefault = subs.SingleOrDefault();
            }
            catch (Exception)
            { }

            try
            {
                var first = subs.First();
            }
            catch (Exception)
            { }

            try
            {
                var firstOrDefault = subs.FirstOrDefault();
            }
            catch (Exception)
            { }
        }

        private void SaveGeoinfo()
        {
            var goinfoDao = Container.GetInstance<ISubscriberGeoinfoDao>();
            var maxmind = Container.GetInstance<DatabaseReader>();

            const string ipAddress = "87.206.135.48";
            const int subscriberId = 84803;

            try
            {
               var geoinfo = maxmind.City(ipAddress);

                goinfoDao.Store(
                    subscriberId,
                    country: geoinfo.Country.IsoCode.Truncate(2),
                    city: geoinfo.City.Name.Truncate(200),
                    zipCode: geoinfo.Postal.Code.Truncate(10),
                    state: geoinfo.MostSpecificSubdivision.IsoCode.Truncate(3),
                    continent: GeoHelper.GetContinentByCode(geoinfo.Continent.Code),
                    timezoneId: TimeZoneHelper.GetTimezone(geoinfo.Location.TimeZone, Container),
                    metroCode: geoinfo.Location.MetroCode,
                    latitude: geoinfo.Location.Latitude,
                    longitude: geoinfo.Location.Longitude
                );
            }
            catch (HibernateException)
            {
                // inne błędy zapisu należy rzucić dalej
                throw;
            }
            catch (Exception)
            {
                // błedy maxminda są ignorowane - to nie jest krytyczny kod
            }
        }

        private void ExistsTest()
        {
            var dao = Container.GetInstance<IMessageDao>().Use(CurrentUnitId);

            var resultFalse = dao.Exists(1);
            var resultTrue = dao.Exists(4);
        }

        private void LoadComplexObject()
        {
            var dao = Container.GetInstance<IAutoresponderDao>().Use(CurrentUnitId);

            var messages1 = dao.Find(m => new {
                Id = m.Id,
                Subject = m.Contents.Where(mc => mc.IsDeleted == false && mc.Subject != string.Empty).Select(mc => mc.Subject).FirstOrDefault(),
                IsAttachments = m.Attachments.Any(a => a.IsInline == false),
                Interval = m.Interval,
                Counters = m.Counters,
                IsEdm = m.Contents.Any(c => c.Source == ContentSource.EdmDesigner),
                HasEmailTests = m.EmailTests.Any()
            }, m => m.IsActive);

            var filtered1 = messages1.Where(m => m.Counters != null);

            var one1 = filtered1.FirstOrDefault();


            var messages2 = dao.Find(m => new
            {
                Id = m.Id,
                Subject = m.Contents.Where(mc => mc.IsDeleted == false && mc.Subject != string.Empty).Select(mc => mc.Subject).FirstOrDefault(),
                IsAttachments = m.Attachments.Any(a => a.IsInline == false),
                Interval = m.Interval,
                Sent = m.Counters.Sent,
                Bounces = m.Counters.Bounces,
                Opens = m.Counters.Opens,
                Clicks = m.Counters.Clicks,
                IsEdm = m.Contents.Any(c => c.Source == ContentSource.EdmDesigner),
                HasEmailTests = m.EmailTests.Any()
            }, m => m.IsActive);

            var filtered2 = messages2.Where(m => m.Sent > 0);

            var one2 = filtered2.FirstOrDefault();
        }
    }
}