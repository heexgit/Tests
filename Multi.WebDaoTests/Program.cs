﻿using Multi.WebDaoTests.DaoTests;
using Multi.WebDaoTests.TestsLib;

namespace Multi.WebDaoTests
{
    internal class Program
    {
        private static void Main()
        {
            //new AnyQueryTests().Start();
            new SubscriberDaoTests().Start();
            //new UserDaoTests().Start();
            //new MessageContentDaoTests().Start();
            //new FactExtendedPropertiesDaoTests().Start();
            //new DomainDaoTests().Start();
            //new TagDaoTests().Start();
            //new SubscriberManagerTests().Start();
        }
    }
}
