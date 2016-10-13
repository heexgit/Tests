﻿using System;
using ExpertSender.Common.Helpers;
using ExpertSender.Common.QueryBuilder;
using ExpertSender.DataModel.Dao;
using ExpertSender.DataModel.Dao.Statistics;
using ExpertSender.DataModel.Enums;
using NHibernate;
using Multi.WebDaoTests.Core;
using EsAppContext = Multi.WebDaoTests.Mocks.EsAppContext;

namespace Multi.WebDaoTests.TestsDao
{
    internal class SubscriberDaoTests : Tester
    {
        private readonly ISubscriberDao _dao;
        private const int CurrentUnitId = 1;

        public SubscriberDaoTests()
            : base(new EsAppContext { CurrentServiceId = CurrentUnitId })
        {
            _dao = Container.GetInstance<ISubscriberDao>();
        }

        public override void Start()
        {
            GetTest();
            NotExistsWithQueryBuilder_Test();
            DoubleFieldsUsage_Test();
            DoubleWhereUsage_Test();
            InsertNullTest();
            UpdateNullTest();
            DoubleValuesInsert_Test();
        }

        private void GetTest()
        {
            var one = _dao.Get(20);
        }

        private void InsertNullTest()
        {
           using (var tran = Container.GetInstance<ISession>().BeginTransaction())
           {
                try
                {
                    var md5 = CryptographyHelper.Md5HashBytes("akudai347DS3@hsyei.pl");

                    var vb = new ValuesBuilder(new
                    {
                        Firstname = "Ala",
                        Lastname = "mala",
                        TrackingCode = (string) null,
                        CreatedOn = DateTime.Now,
                        Email = "akudai347DS3@hsyei.pl",
                        EmailMd5 = md5
                    });
                     _dao.Insert(vb);
                }
                finally
                {
                    // przywracamy zmienione obiekty do poprzedniego stanu
                    tran.Rollback();
                }
            }
        }

        private void UpdateNullTest()
        {
           using (var tran = Container.GetInstance<ISession>().BeginTransaction())
           {
                try
                {
                     _dao.Update(new SetBuilder(new
                        {
                            Firstname = "Ala",
                            Lastname = "mala",
                            TrackingCode = (string)null
                        }), new WhereBuilder(new
                        {
                            Id = 6
                        }));
                }
                finally
                {
                    // przywracamy zmienione obiekty do poprzedniego stanu
                    tran.Rollback();
                }
            }
        }

        private void NotExistsWithQueryBuilder_Test()
        {
           var existingId = _dao.SelectOne(
               new SelectBuilder("Id"),
               new WhereBuilder(new
               {
                    Email = "akudai347DS3@hsyei.pl"
               })
            );
        }

        private void DoubleFieldsUsage_Test()
        {
            
            using (var tran = Container.GetInstance<ISession>().BeginTransaction())
            {
                try
                {
                    var whereFields = new
                    {
                        Email = "grzegorz.tylak@expertsender.com"
                    };

                    var existingId = _dao.SelectOne(new SelectBuilder("Id"), new WhereBuilder(whereFields));

                    if (existingId != null)
                    {
                        _dao.Update(new SetBuilder(new {Firstname = "Jan1"}), new WhereBuilder(whereFields));
                    }
                }
                finally
                {
                    // przywracamy zmienione obiekty do poprzedniego stanu
                    tran.Rollback();
                }
            }
        }

        private void DoubleWhereUsage_Test()
        {
            using (var tran = Container.GetInstance<ISession>().BeginTransaction())
            {
                try
                {
                    var whereBuilder = new WhereBuilder(new
                    {
                        Email = "grzegorz.tylak@expertsender.com"
                    });

                    var existingId = _dao.SelectOne(new SelectBuilder("Id"), whereBuilder);

                    if (existingId != null)
                    {
                        _dao.Update(new SetBuilder(new {Firstname = "Jan2"}), whereBuilder);
                    }
                }
                finally
                {
                    // przywracamy zmienione obiekty do poprzedniego stanu
                    tran.Rollback();
                }
            }
        }

        private void DoubleValuesInsert_Test()
        {
            var dimensionDao = Container.GetInstance<IDimensionDao>();

            var factSubscriptionsTable = $"[ES_DW_Unit_{CurrentUnitId}].[dbo].FactSubscriptions";

            using (var tran = Container.GetInstance<ISession>().BeginTransaction())
            {
                try
                {
                    var valuesBuilder = new ValuesBuilder(new
                    {
                        DateTimeId = DateTime.Now,
                        SubscriberId = 4238630,
                        DomainId = 1000,
                        ClientIP = "65.97.88.99",
                        SubscriptionSourceId = SubscriptionSource.Api
                    });

                    var v1 = valuesBuilder.Combine(new ValuesBuilder(new {ListId = 8}));
                    var v2 = valuesBuilder.Combine(new ValuesBuilder(new {ListId = 11}));

                    dimensionDao.Insert(new InsertBuilder(factSubscriptionsTable), new ValuesBuilder(new
                    {
                        DateTimeId = DateTime.Now,
                        SubscriberId = 4238630,
                        DomainId = 1000,
                        ClientIP = "65.97.88.99",
                        SubscriptionSourceId = SubscriptionSource.Api,
                        ListId = 9
                    }));

                    dimensionDao.Insert(new InsertBuilder(factSubscriptionsTable), v1);

                    dimensionDao.Insert(new InsertBuilder(factSubscriptionsTable), v2);
                }
                finally
                {
                    // przywracamy zmienione obiekty do poprzedniego stanu
                    tran.Rollback();
                }
            }
        }
    }
}