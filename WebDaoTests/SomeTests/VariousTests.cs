using System;
using System.Linq;
using System.Linq.Expressions;
using ExpertSender.Common.Dao;
using ExpertSender.DataModel.Dao;
using ExpertSender.DataModel;
using ExpertSender.DataModel.Enums;
using NHibernate;
using NHibernate.Linq;
using WebDaoTests.Core;
using EsAppContext = WebDaoTests.Mocks.EsAppContext;

namespace WebDaoTests.SomeTests
{
    internal class VariousTests : Tester
    {
        private const int CurrentUnitId = 1;

        public VariousTests()
            : base(new EsAppContext { CurrentServiceId = CurrentUnitId })
        { }

        public override void Start()
        {
            //AnyVsCount();
            //DataTables();
            EnurableVsQueryable();
        }

        private void DataTables()
        {
            var datatebleDao = Container.GetInstance<IDataTableDao>();
            var column = datatebleDao.GetTableColumn(8);
            var table = column.SourceTable;
            var relationships = table.Relationships;
        }

        private void AnyVsCount()
        {
            var data = new { ListId = 76 };
            var subscriberId = 6654;

            Expression<Func<SubscriberInList, bool>> filter =
                sil => sil.List.Id == data.ListId && sil.Subscriber.Id == subscriberId && sil.Status == SubscriberStatus.Active && !sil.List.IsDeleted;

            var sess = Container.GetInstance<ISession>();

            // select cast(count(*) as INT) as col_0_0_ from SubscriberInList...
            var result1 = sess.Query<SubscriberInList>()
                .Count(filter) > 0;

            // select TOP (1) Id, SubscriberId, (...) from SubscriberInList...
            var result2 = sess.Query<SubscriberInList>()
                .Any(filter);

            var dao = Container.GetInstance<IWebDao<SubscriberInList>>();
            dao.Use(CurrentUnitId);

            var result3 = dao.Find(a => new { Id = a.Id }, filter)
                .Any();

            // select TOP (1) Id from SubscriberInList...
            var result4 = dao.Find(a => (int?)a.Id, filter)
                .Any();

            var result5 = dao.Query()
                .Where(filter)
                .Select(e => e.Id)
                .Any();

            var result6 = sess.Query<SubscriberInList>()
                .Where(filter)
                .Select(e => e.Id)
                .Any();
        }

        private void EnurableVsQueryable()
        {
            var sess = Container.GetInstance<ISession>();

            var qResult = sess.Query<List>().Where(l => l.Id == 11).Select(l => l.Name);// sql not executed
            var qFirst = qResult.FirstOrDefault();

            var eResult1 = sess.Query<List>().AsEnumerable().Where(l => l.Id == 12).Select(l => l.Name);// sql not executed
            var eFirst1 = eResult1.FirstOrDefault();
            var eResult2 = sess.Query<List>().Where(l => l.Id == 13).Select(l => l.Name).AsEnumerable();// sql not executed
            var eFirst2 = eResult2.FirstOrDefault();

            var lResult = sess.Query<List>().Where(l => l.Id == 14).Select(l => l.Name).ToList();// sql DO executed
            var lFirst = lResult.FirstOrDefault();
        }
    }
}