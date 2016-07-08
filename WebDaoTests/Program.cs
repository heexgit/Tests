
using WebDaoTests.Tests;

namespace WebDaoTests
{
    internal class Program
    {
        private static void Main()
        {
            new UserDaoTests().Start();
            new MessageContentDaoTests().Start();
            new AnyQueryTests().Start();
        }
    }
}
