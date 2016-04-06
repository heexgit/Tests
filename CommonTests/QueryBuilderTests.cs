using System.Collections.Generic;
using ExpertSender.Common.Dao;
using ExpertSender.Common.Entities;
using ExpertSender.Common.QueryBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonTests
{
    [TestClass]
    public class QueryBuilderTests
    {
        #region INSERT
        [TestMethod]
        public void Insert_Basic_Test()
        {
            var actual = NewQueryBuilder
                .Insert(new InsertBuilder<MockEntity>())
                .Values(new ValuesBuilder(new { Name = "Ala", Age = 12 }))
                .GetQuery();

            var expected = $@"INSERT INTO {MockEntityName} (Name,Age) VALUES(@Name,@Age)";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Insert_MultipleValue_Test()
        {
            var actual = NewQueryBuilder
                .Insert(new InsertBuilder<MockEntity>())
                .Values(new ValuesBuilder(new { Name = "Ala", Age = 12 }))
                .Values(new { Email = "ala@nowak.pl" })
                .Values(new { Family = "Nowak" })
                .GetQuery();

            var expected = $@"INSERT INTO {MockEntityName} (Name,Age,Email,Family) VALUES(@Name,@Age,@Email,@Family)";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Insert_Select_Test()
        {
            var actual = NewQueryBuilder
                .Insert(new InsertBuilder<MockEntity>())
                .Select(new InsertSelectBuilder("Name","Age","Email"))
                .From(MockEntityName + "Data")
                .Where(new { Email = "ala@nowak.pl" })
                .Where(new { Family = "Nowak" })
                .GetQuery();

            var expected = $@"INSERT INTO {MockEntityName} (Name,Age,Email) SELECT Name,Age,Email FROM {MockEntityName}Data WHERE Email=@Email AND Family=@Family";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Insert_Select_DuplicatedParamNames_Test()
        {
            var actual = NewQueryBuilder
                .Insert(new InsertBuilder<MockEntity>())
                .Select(new InsertSelectBuilder("Name","Age","Email"))
                .Select(new { Family = "Nowak" })
                .From(MockEntityName + "Data")
                .Where(new { Email = "ala@nowak.pl" })
                .Where(new { Family = "Kowal" })
                .GetQuery();

            var expected = $@"INSERT INTO {MockEntityName} (Name,Age,Email,Family) SELECT Name,Age,Email,@Family FROM {MockEntityName}Data WHERE Email=@Email AND Family=@Family1";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Insert_Fast_Test()
        {
            var qb = NewQueryBuilder
                .Insert(new InsertBuilder<MockEntity>(new MockEntity
                {
                    Name = "Ala",
                    Age = 99,
                    Email = "ala@nowak.pl"
                }));
            var actual = qb.GetQuery();

            var expected = $@"INSERT INTO {MockEntityName} (Age,Name,Email) VALUES(@Age,@Name,@Email)";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Insert_FastComplex_Test()
        {
            var qb = NewQueryBuilder
                .Insert(new InsertBuilder<MockComplexEntity>(new MockComplexEntity
                {
                    Name = "Ala",
                    Age = 99,
                    Email = "ala@nowak.pl"
                }));
            var actual = qb.GetQuery();

            var expected = $@"INSERT INTO {MockComplexEntityName} (Age,Name,Email) VALUES(@Age,@Name,@Email)";

            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region UPDATE
        [TestMethod]
        public void Update_Basic_Test()
        {
            var actual = NewQueryBuilder
                .Insert(new InsertBuilder<MockEntity>())
                .Values(new ValuesBuilder(new { Name = "Ala", Age = 12 }))
                .GetQuery();

            var expected = $@"INSERT INTO {MockEntityName} (Name,Age) VALUES(@Name,@Age)";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Update_MultipleSet_Test()
        {
            var actual = NewQueryBuilder
                .Update(new UpdateBuilder<MockEntity>())
                .Set(new SetBuilder(new { Name = "Ala", Age = 12 }))
                .Set("Family='Nowak'")
                .Set(new { Email = "ala@nowak.pl" })
                .Where(new WhereBuilder(new { Id = 123 }))
                .GetQuery();

            var expected = $@"UPDATE {MockEntityName} SET Family='Nowak',Name=@Name,Age=@Age,Email=@Email WHERE Id=@Id";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Update_DuplicatedParamNames_Test()
        {
            var actual = NewQueryBuilder
                .Update(new UpdateBuilder<MockEntity>())
                .Set(new SetBuilder(new { Name = "Ala", Age = 12 }))
                .Where(new WhereBuilder(new { Name = "Ewa", Age = 18 }))
                .GetQuery();

            var expected = $@"UPDATE {MockEntityName} SET Name=@Name,Age=@Age WHERE Name=@Name1 AND Age=@Age1";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Update_ComplexEntity_Test()
        {
            var actual = NewQueryBuilder
                .Update(new UpdateBuilder<MockComplexEntity>())
                .Set(new SetBuilder(new { Name = "Ala", Age = 12 }))
                .Where(new WhereBuilder(new { Name = "Ewa", Age = 18 }))
                .GetQuery();

            var expected = $@"UPDATE {MockComplexEntityName} SET Name=@Name,Age=@Age WHERE Name=@Name1 AND Age=@Age1";

            Assert.AreEqual(expected, actual);
        }
        #endregion

        private static FrontQueryBuilder NewQueryBuilder => new FrontQueryBuilder(new DapperMediator());

        private static string MockEntityName => typeof(MockEntity).Name;
        private static string MockComplexEntityName => typeof(MockComplexEntity).Name;

        private class MockEntity : AbstractEntity<int>
        {
            public int? Age { get; set; }
            public string Name { get; set; }
            public string Family { get; set; }
            public string Email { get; set; }
        }

        private class MockComplexEntity : MockEntity
        {
            public NestedClass Nested { get; set; }
            public List<NestedClass> Nesteds { get; set; }

            public MockComplexEntity()
            {
                Nesteds = new List<NestedClass>();
            }

            public class NestedClass
            {
                public string Name { get; set; }
            }
        }
    }
}
