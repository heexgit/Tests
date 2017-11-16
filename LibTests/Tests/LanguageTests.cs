using System;
// ReSharper disable LocalizableElement

namespace CommonRuns.Tests
{
    internal class LanguageTests
    {
        public void Start()
        {
            //AndExecutionOrder();
            GetTypeTests();
        }

        private static void AndExecutionOrder()
        {
            var a = false;
            a &= ReturnTrue("a &= ReturnTrue"); // executes ReturnTrue

            a = a && ReturnTrue("a = a && ReturnTrue"); // DOESN'T execute ReturnTrue

            a = ReturnTrue("a = ReturnTrue && a") && a; // executes ReturnTrue


            bool ReturnTrue(string executionMethod)
            {
                Console.WriteLine($@"ReturnTrue executed by: '{executionMethod}'");
                return true;
            }
        }

        private static void GetTypeTests()
        {
            var objB = new B();

            ByGeneric(objB);
            ByObjectType(objB);
            ByObjectDeclaringType(objB);

            void ByGeneric<T>(T obj)
            {
                Console.Write(nameof(ByGeneric) + ": ");
                Console.WriteLine(typeof(T).Name);
            }

            void ByObjectType(object obj)
            {
                Console.Write(nameof(ByObjectType) + ": ");
                Console.WriteLine(obj.GetType().Name);
            }

            void ByObjectDeclaringType(object obj)
            {
                try
                {
                    Console.Write(nameof(ByObjectDeclaringType) + ": ");
                    Console.WriteLine(obj.GetType().DeclaringType.Name);
                }
                catch (Exception) { }
            }
        }
        
        class A { }
        class B : A { }
    }
}
