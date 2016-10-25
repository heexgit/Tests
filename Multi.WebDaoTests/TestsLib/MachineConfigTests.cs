using System.Configuration;
using ExpertSender.DataModel.Distributed.Enums;
using ExpertSender.DataModel.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Multi.WebDaoTests.Core;
using EsAppContext = Multi.WebDaoTests.Mocks.EsAppContext;

namespace Multi.WebDaoTests.TestsLib
{
    [TestClass]
    public class MachineConfigTests : Tester
    {
        private const int CurrentUnitId = 1;

        public MachineConfigTests()
            : base(new EsAppContext {CurrentServiceId = CurrentUnitId})
        { }

        public override void Start()
        {
            NotExistOrEmptyTest();
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void NotExistOrEmptyTest()
        {
            var setting = MachineConfig.GetSetting(ColocationSetting.InvaluementDnsServer);
        }
    }
}