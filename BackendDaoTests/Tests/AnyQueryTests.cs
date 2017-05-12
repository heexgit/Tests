using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BackendDaoTests.Core;
using ExpertSender.DataModel.BackendDao;
using BackendDaoTests.Mocks;
using Dapper;
using ExpertSender.Common.Dao;
using ExpertSender.Common.Helpers;
using ExpertSender.Common.QueryBuilder;
using ExpertSender.DataModel;
using ExpertSender.DataModel.BackendDao.Workflows;
using ExpertSender.DataModel.Distributed;
using ExpertSender.DataModel.Enums;
using ExpertSender.DataModel.Enums.Workflows;
using ExpertSender.DataModel.Simplified.Workflows;
using ExpertSender.DataModel.Workflows;

namespace BackendDaoTests.Tests
{
    internal class AnyQueryTests : Tester
    {
        private const int CurrentUnitId = 1;

        public AnyQueryTests()
            : base(new EsAppContext { CurrentServiceId = CurrentUnitId })
        { }

        public override void Start()
        {
            //LoadByIdsTest();
            //LoadJoinTest();
            //LoadJoinByIdsTest();
            //LoadWorkflowPathsOrderTest();
            //DaoNatureTests();
            //TempTableCreateAndSelect_Move();
            //TempTableCreateAndSelect_Add();
            //CreateShipmentSubscribersListTest();
            TwoQueriesTest();
        }

        private void LoadByIdsTest()
        {
            var ids = new []{ 523425, 523429, 779703, 779740, 779762, 779766, 523430};

            var dao = Container.GetInstance<ISubscriberDao>().Use(CurrentUnitId);
            var susbs = dao.SelectMany(new SelectBuilder("Id, Firstname, Lastname"), new WhereBuilder(new {Id = ids}))
                .Select(s => new { s.Id, s.Firstname, s.Lastname });
        }

        private void LoadJoinTest()
        {
            var dao = Container.GetInstance<ISubscriberDao>().Use(CurrentUnitId);

            var sel = new SelectBuilder("s.Id, s.Firstname, s.Lastname, sil.SubscribedOn")
                .From(new FromBuilder("Subscriber", "s"))
                .Join(new JoinBuilder("SubscriberInList", "sil")).On(new OnBuilder("sil.SubscriberId = s.Id"))
                .Where(new WhereBuilder().Where("s.Id={0}", new {SubscriberId = 523425}));

            var susbs = dao.SelectMany((SelectBuilder)sel.MainClause);
        }

        private void LoadJoinByIdsTest()
        {
            var ids = new []{ 523425, 523429, 779703, 779740, 779762, 779766, 523430};
            var dao = Container.GetInstance<ISubscriberDao>().Use(CurrentUnitId);

            var sel = new SelectBuilder("s.Id, s.Firstname, s.Lastname, sil.SubscribedOn")
                .From(new FromBuilder("Subscriber", "s WITH(NOLOCK)"))
                .Join(new JoinBuilder("SubscriberInList", "sil WITH(NOLOCK)")).On(new OnBuilder("sil.SubscriberId = s.Id"))
                .Where(new WhereBuilder().Where("s.Id IN{0}", new {SubscriberId = ids}));

            var susbs = dao.SelectMany((SelectBuilder)sel.MainClause);
        }

        private void LoadWorkflowPathsOrderTest()
        {
            var dao = Container.GetInstance<IBackendDao<WorkflowDataSwitchPath>>();
            dao.Use(CurrentUnitId);
            
            var all = dao.FindAll();

            var dataSwitchPaths = dao.SelectMany(new SelectBuilder("Id, WorkflowDataSwitchId, Name, WorkFlowElementId, SubscriberSegmentId"), new WhereBuilder(new {WorkflowDataSwitchId = 2027}));
            var dataSwitchPaths2 = dataSwitchPaths.Select(p => new
            {
                p.Id, p.WorkflowDataSwitchId, p.Name, p.WorkFlowElementId,
                Segment = p.SubscriberSegmentId == null ? null : new { Id = p.SubscriberSegmentId}
            }).ToList();
            
            var dataSwitchPaths4 = dataSwitchPaths2.Where(p => p.Id == 25).Union(dataSwitchPaths2.Where(p => p.Id == 23)).Union(dataSwitchPaths2.Where(p => p.Id == 24));

            var dataSwitchPaths5 = dataSwitchPaths4.OrderByDescending(p => p.Segment?.Id);
            var dataSwitchPaths6 = dataSwitchPaths4.OrderBy(p => p.Segment?.Id);

            var workflowDataSwitchDao = Container.GetInstance<IWorkflowDataSwitchDao>();
            //workflowDataSwitchDao.Use(CurrentUnitId);
            //var switches = workflowDataSwitchDao.GetSwitchesWithSubscribers(DateTime.Now);
            //// dla każdego switcha
            //foreach (var switchElement in switches)
            //{
            //    var paths = switchElement.Paths.Where(p => p.Segment != null).Concat(switchElement.Paths.Where(p => p.Segment == null)).ToList();
            //    var paths2 = switchElement.Paths.OrderByDescending(p => p.Segment).ToList();
            //}
        }

        private void DaoNatureTests()
        {
            var dao1 = Container.GetInstance<IBounceAggregationDao>().Use(CurrentUnitId);

            var dao2 = Container.GetInstance<IBackendDao<BounceAggregation>>();
            dao2.Use(CurrentUnitId);

            var dao3 = Container.GetInstance<IBlackListCheckerDao>().Use(CurrentUnitId);

            var dao4 = Container.GetInstance<IBackendDao<BlacklistChecker>>();
            dao2.Use(CurrentUnitId);
        }

        private void TempTableCreateAndSelect_Move()
        {
            var dao = Container.GetInstance<IWorkflowStartPointDao>().Use(CurrentUnitId);

            dao.StartSubscriber(new WorkflowStartPointSimplified
            {
                Id = 2045,
                WorkflowId = 12,
                WorkflowElementId = 2047,
                DuplicateDataFields = new WorkflowStartPointDuplicateDataFieldSimplified[0]
            }, new []
            {
                new WorkflowStartPointEventSimplified
                {
                    SubscriberId = 1,
                    DataFields = new WorkflowStartPointEventDataFieldSimplified[0]
                }
            }, true);
        }

        private void TempTableCreateAndSelect_Add()
        {
            var dao = Container.GetInstance<IWorkflowStartPointDao>().Use(CurrentUnitId);

            dao.StartSubscriber(new WorkflowStartPointSimplified
            {
                Id = 2045,
                WorkflowId = 12,
                WorkflowElementId = 2047,
                DuplicateDataFields = new WorkflowStartPointDuplicateDataFieldSimplified[0]
            }, new []
            {
                new WorkflowStartPointEventSimplified
                {
                    SubscriberId = 7,
                    DataFields = new WorkflowStartPointEventDataFieldSimplified[0]
                }
            }, true);
        }

        private void CreateShipmentSubscribersListTest()
        {
            var dao = Container.GetInstance<IShipmentDao>().Use(CurrentUnitId);

            var tempTableNameGuid = Guid.NewGuid().ToString("N");

            try { 
                dao.CreateShipmentSubscribersListTempTable(tempTableNameGuid);
                dao.CreateShipmentSubscribersList(new Shipment
                {
                    Id = 15100,
                    Message = new Message
                    {
                        Type = MessageType.Test
                    },
                    Targets = new List<ShipmentTarget>
                    {
                        new ShipmentTarget
                        {
                            Id = 10041,
                            List = new List
                            {
                                Id = 1162
                            }
                        }
                    }
                }, TimeZoneInfo.Utc, TimeZoneInfo.Utc, false, null, null, false, null, null, tempTableNameGuid);
            }
            finally
            {
                dao.RemoveShipmentSubscribersListTempTable(tempTableNameGuid);
            }
        }

        private void TwoQueriesTest()
        {
            var dao = Container.GetInstance<ISubscriberInWorkflowDao>().Use(CurrentUnitId);

            var sqlUpd = @"CREATE TABLE #changedSIW (SubscriberId INT);
                        UPDATE [ES_TR_Unit_1].[dbo].[SubscriberInWorkflow]
                        SET WorkflowElementId = @WorkflowElementId
                        OUTPUT inserted.SubscriberId
                        INTO #changedSIW
                        WHERE
                            Id IN (SELECT Id FROM [ES_TR_Unit_1].[dbo].[SubscriberInWorkflow])
                            AND WorkflowElementId != @WorkflowElementId
                            and Status != @Status;
                        SELECT SubscriberId, COUNT(1) AS Amount FROM #changedSIW GROUP BY SubscriberId;";

            var con = Container.GetInstance<IDbConnection>();
            var transactionProvider = Container.GetInstance<IDbTransactionProvider>();
            var currentTransaction = transactionProvider.GetCurrentTransaction();

            try
            {
                var manys = con.Query(sqlUpd, new { WorkflowElementId = 2002, Status = 3 }, currentTransaction, true, 30);
            }
            catch (Exception e)
            {
                using (var manys = con.QueryMultiple(sqlUpd, new { WorkflowElementId = 2002 }, currentTransaction, 30))
                {
                    var a = manys.Read<int>().SingleOrDefault();
                    var b = manys.Read<int>().SingleOrDefault();
                    var c = manys.Read().SingleOrDefault();
                }
            }
        }
    }
}