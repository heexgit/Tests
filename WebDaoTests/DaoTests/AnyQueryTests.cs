using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ExpertSender.Common.Dao;
using ExpertSender.DataModel;
using ExpertSender.DataModel.Dao;
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
            LoadSubscribedOn();
            //ChangeSubscribedOn();
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
        private int GetCacheStatus(byte[] hashKey, SqlConnection connection, SqlTransaction transaction, int timeout)
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
    }
}