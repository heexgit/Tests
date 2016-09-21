using System.Collections.Generic;
using System.Linq;
using ExpertSender.DataModel.Dao;
using WebDaoTests.Core;
using EsAppContext = WebDaoTests.Mocks.EsAppContext;

namespace WebDaoTests.Tests
{
    internal class TagDaoTests : Tester
    {
        private const int CurrentUnitId = 1;

        public TagDaoTests()
            : base(new EsAppContext
            {
                CurrentServiceId = CurrentUnitId
            })
        { }

        public override void Start()
        {
            FindTest();
        }

        public void FindTest()
        {
            var dao = Container.GetInstance<ITagDao>();

            var tags = new [] {"ali", "BALI", "Mali", "niemali"};

            var all0 = dao.Find(t => new {t.Id, t.Name},
                t => t.Name.Contains("ali"));

            var all01 = dao.Find(t => new {t.Id, t.Name},
                t => t.Name.Contains(tags.First()));

            var all02 = dao.Find(t => new {t.Id, t.Name},
                t => t.Name.ToUpper().Contains(tags.First().ToUpper()));

            var all03 = dao.Find(t => new {t.Id, t.Name},
                t => t.Name.ToUpper().StartsWith(tags.First().ToUpper()));

            var all04 = dao.Find(t => new {t.Id, t.Name},
                t => tags.FirstOrDefault(e => t.Name.ToUpper().StartsWith(e.ToUpper())) != null);

            var found = tags
                .Select(tag => dao.Find(t => new {t.Id, t.Name}, t => t.Name.ToUpper().StartsWith(tag.ToUpper())))
                .Where(all05 => all05.Any())
                .SelectMany(all05 => all05)
                .Select(a => a.Name)
                .ToList();

            var found1 = tags
                .SelectMany(tag => dao.Find(t => new {t.Id, t.Name}, t => t.Name.ToUpper().StartsWith(tag.ToUpper())))
                .Select(a => a.Name)
                .ToList();

           /* 
            * these doesn't work
            * var all1 = dao.Find(t => new {t.Id, t.Name},
                t => tags.Any(seek => t.Name.ToUpper().StartsWith(seek.ToUpper())));

            var all2 = dao.Find(t => new {t.Id, t.Name},
                t => tags.Any(seek => t.Name.StartsWith(seek)));
            
            var all3 = dao.Find(t => new {t.Id, t.Name},
                t => tags.Any(seek => t.Name.ToUpper().Contains(seek.ToUpper())));
            
            var all4 = dao.Find(t => new {t.Id, t.Name},
                t => tags.Any(seek => t.Name.ToUpper() == seek.ToUpper()));
            
            var all5 = dao.Find(t => new {t.Id, t.Name},
                t => tags.Any(seek => t.Name == seek.ToUpper()));
            
            var all6 = dao.Find(t => new {t.Id, t.Name},
                t => tags.Any(seek => t.Name.ToUpper() == seek));
            
            var all7 = dao.Find(t => new {t.Id, t.Name},
                t => tags.Any(seek => t.Name == seek));
            
            var all8 = dao.Find(t => new {t.Id, t.Name},
                t => tags.Any(seek => t.Name.Contains(seek)));
            
            var all9 = dao.Find(t => new {t.Id, t.Name},
                t => tags.Any(seek => t.Name.Contains(seek)));*/
        }
    }
}
