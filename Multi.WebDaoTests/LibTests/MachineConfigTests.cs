using System.Configuration;
using ExpertSender.DataModel.CommonDao;
using ExpertSender.DataModel.Distributed.Enums;
using ExpertSender.DataModel.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Multi.WebDaoTests.Core;
using EsAppContext = Multi.WebDaoTests.Mocks.EsAppContext;

namespace Multi.WebDaoTests.LibTests
{
    [TestClass]
    public class MachineConfigTests : Tester
    {
        private const int CurrentUnitId = 1;

        public MachineConfigTests()
            : base(new EsAppContext {CurrentServiceId = CurrentUnitId})
        { }

        public override void Start()
        { }

        [TestMethod]
        public void ExistEmptyRequiredTest()
        {
            var setting = MachineConfig.GetSetting(ColocationSetting.InvaluementDnsServer);

            var colocationDao = Container.GetInstance<IColocationDao>();
            var colocation = colocationDao.GetWithSettings();

            Assert.AreEqual(colocation.GetSetting(ColocationSetting.InvaluementDnsServer), setting);
        }

        [TestMethod]
        public void ExistEmptyOptionalTest()
        {
            var setting = MachineConfig.GetSetting(ColocationSetting.InvaluementDnsServer, false);

            Assert.AreEqual("", setting);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void NotExistRequiredTest()
        {
            var setting = MachineConfig.GetSetting((ColocationSetting)99909);
        }

        [TestMethod]
        public void NotExistOptionalTest()
        {
            var setting = MachineConfig.GetSetting((ColocationSetting)99909, false);

            Assert.IsNull(setting);
        }

        [TestMethod]
        public void ExistTest()
        {
            var setting = MachineConfig.GetSetting(ColocationSetting.ImageBrowser_HostedImagesDir);

            Assert.AreEqual("xxdd123", setting);
        }

        [TestMethod]
        public void ExistWithDotsTest()
        {
            var setting = MachineConfig.GetSetting(ColocationSetting.Microsoft_ServiceBus_ConnectionString);

            Assert.AreEqual("abc123", setting);
        }
    }
}