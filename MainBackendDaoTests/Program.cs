using MainBackendDaoTests.Tests;

namespace MainBackendDaoTests
{
    internal class Program
    {
        private static void Main()
        {
            new TriggerDaoTests().Start();
            new ShipmentDaoTests().Start();
        }
    }
}
