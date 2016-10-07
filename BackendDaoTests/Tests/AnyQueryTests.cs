using System.Linq;
using BackendDaoTests.Core;
using ExpertSender.DataModel.BackendDao;
using BackendDaoTests.Mocks;
using ExpertSender.Common.QueryBuilder;

namespace BackendDaoTests.Tests
{
    internal class AnyQueryTests : Tester
    {
        private const int CurrentUnitId = 7;

        public AnyQueryTests()
            : base(new EsAppContext { CurrentServiceId = CurrentUnitId })
        { }

        public override void Start()
        {
            LoadByIdsTest();
            LoadJoinTest();
            LoadJoinByIdsTest();
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
    }
}