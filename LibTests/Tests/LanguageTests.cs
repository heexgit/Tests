using System;

namespace CommonRuns.Tests
{
    internal class LanguageTests
    {
        public void Start()
        {
            AndExecutionOrder();
        }

        private static void AndExecutionOrder()
        {
            var a = false;
            a &= ReturnTrue("a &= ReturnTrue"); // executes ReturnTrue

            a = a && ReturnTrue("a = a && ReturnTrue"); // DOESN'T execute ReturnTrue

            a = ReturnTrue("a = ReturnTrue && a") && a; // executes ReturnTrue

        }

        private static bool ReturnTrue(string executionMethod)
        {
            Console.WriteLine($@"ReturnTrue executed by: '{executionMethod}'");
            return true;
        }
    }
}
