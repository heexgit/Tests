using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ExpertSender.Common;
using NHibernate;
using WebDaoTests.Core;

namespace WebDaoTests.Tests
{
    internal class AnyQueryTests : Tester
    {
        public override void Start()
        {
            var app = Container.GetInstance<IEsAppContext>();
            app.CurrentServiceId = 1;

            GetCacheStatusTest();
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
    }
}
