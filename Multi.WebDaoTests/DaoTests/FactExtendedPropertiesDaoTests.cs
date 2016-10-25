using ExpertSender.DataModel.CommonDao;
using Multi.WebDaoTests.Core;
using EsAppContext = Multi.WebDaoTests.Mocks.EsAppContext;

namespace Multi.WebDaoTests.DaoTests
{
    internal class FactExtendedPropertiesDaoTests : Tester
    {
        private const int CurrentUnitId = 1;

        public FactExtendedPropertiesDaoTests()
            : base(new EsAppContext { CurrentServiceId = CurrentUnitId })
        { }

        public override void Start()
        {
            SaveBrowserVersionTest();
            //SaveBrowserVersion2Test();
            //SaveClientLanguageTest();
            //SaveClientLanguage2Test();
            //SaveClientLanguage3Test();
            //SaveDeviceVersionTest();
            //SaveDeviceVersion2Test();
        }

        public void SaveBrowserVersionTest()
        {
            var dao = Container.GetInstance<IFactExtendedPropertiesDao>();
            dao.Use(CurrentUnitId);

            dao.Init();

            var result = dao.SaveBrowserVersion("Internet Explorer", "9.0");
            // should be = 30
        }

        public void SaveBrowserVersion2Test()
        {
            var dao = Container.GetInstance<IFactExtendedPropertiesDao>();
            dao.Use(CurrentUnitId);

            dao.Init();

            var result = dao.SaveBrowserVersion("Kupa", "1.0");
        }

        public void SaveClientLanguageTest()
        {
            var dao = Container.GetInstance<IFactExtendedPropertiesDao>();
            dao.Use(CurrentUnitId);

            dao.Init();

            var result = dao.SaveClientLanguage("en-GB");
            // should be = 14567
        }

        public void SaveClientLanguage2Test()
        {
            var dao = Container.GetInstance<IFactExtendedPropertiesDao>();
            dao.Use(CurrentUnitId);

            dao.Init();

            var result = dao.SaveClientLanguage("en-GBa");
            // should be = 14567
        }

        public void SaveClientLanguage3Test()
        {
            var dao = Container.GetInstance<IFactExtendedPropertiesDao>();
            dao.Use(CurrentUnitId);

            dao.Init();

            var result = dao.SaveClientLanguage("pl22");
        }

        public void SaveDeviceVersionTest()
        {
            var dao = Container.GetInstance<IFactExtendedPropertiesDao>();
            dao.Use(CurrentUnitId);

            dao.Init();

            var result = dao.SaveDeviceVersion("Samsung", "Samsung");
            // should be 2
        }

        public void SaveDeviceVersion2Test()
        {
            var dao = Container.GetInstance<IFactExtendedPropertiesDao>();
            dao.Use(CurrentUnitId);

            dao.Init();

            var result = dao.SaveDeviceVersion("Samsung", "Kindle-Silk 1");
        }
    }
}
