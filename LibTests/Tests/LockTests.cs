using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommonRuns.Tests
{
    internal class LockTests
    {
        public void Start()
        {
            //AccountLocking();
            DisplayTasksExecution();
        }

        private static void AccountLocking()
        {
            var threads = new Thread[10];
            var acc = new Account(1000);
            for (var i = 0; i < 10; i++)
            {
                var t = new Thread(acc.DoTransactions);
                threads[i] = t;
            }
            for (var i = 0; i < 10; i++)
            {
                threads[i].Start();
            }
        }

        /// <summary>
        /// Pokazuje, że wątki są wstrzymywane przez locka i czekają na zwolnienie
        /// </summary>
        private static void DisplayTasksExecution()
        {
            const int max = 10;
            var alfas = new TaskNumberDisplayer[max];

            var threads = new Task[max];

            for (var i = 0; i < max; i++)
            {
                var alfa = new TaskNumberDisplayer(i);
                alfas[i] = alfa;

                threads[i] = new Task(alfa.Display);
            }

            for (var i = 0; i < max; i++)
            {
                threads[i].Start();
            }

            Task.WaitAll(threads);
        }
    }

    internal class TaskNumberDisplayer
    {
        private readonly int _counter;
        private static readonly object MyLock = new object();

        public TaskNumberDisplayer(int counter)
        {
            _counter = counter;
        }

        public void Display()
        {
            var message1 = $"[   ] Enter Display by:        {_counter}";
            Console.WriteLine(message1);

            lock (MyLock)
            {
                var message2 = $"[ > ] Start Display code by:   {_counter}";
                Console.WriteLine(message2);

                Thread.Sleep(100);
                //Console.ReadKey();

                var message3 = $"[ * ] Finish Display code by:  {_counter}";
                Console.WriteLine(message3);
            }
        }
    }
    

    internal class Account
    {
        private readonly object _thisLock = new object();
        private int _balance;

        readonly Random _randomizer;

        public Account(int initial)
        {
            _balance = initial;
            _randomizer = new Random();
        }

        private int Withdraw(int amount)
        {
            // This condition never is true unless the lock statement is commented out.
            if (_balance < 0)
            {
                throw new Exception("Negative Balance");
            }

            // Comment out the next line to see the effect of leaving out the lock keyword.
            lock (_thisLock)
            {
                if (_balance < amount)
                    return 0; // transaction rejected

                Console.WriteLine("Balance before Withdrawal :  " + _balance);
                Console.WriteLine("Amount to Withdraw        : -" + amount);
                _balance = _balance - amount;
                Console.WriteLine("Balance after Withdrawal  :  " + _balance);
                return amount;
            }
        }

        public void DoTransactions()
        {
            for (var i = 0; i < 100; i++)
            {
                Withdraw(_randomizer.Next(1, 100));
            }
        }
    }
}