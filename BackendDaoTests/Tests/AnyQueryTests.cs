using System;
using System.Linq;
using BackendDaoTests.Core;
using ExpertSender.DataModel.BackendDao;
using BackendDaoTests.Mocks;
using ExpertSender.Common.Dao;
using ExpertSender.Common.QueryBuilder;
using ExpertSender.DataModel.BackendDao.Workflows;
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
            LoadWorkflowPathsOrderTest();
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
    }
}