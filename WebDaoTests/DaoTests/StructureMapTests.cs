using WebDaoTests.Core;
using StructureMap;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebDaoTests.DaoTests
{
    [TestClass]
    public class StructureMapTests : Tester
    {
        public StructureMapTests()
        {
            RegisterTypes();
        }

        public override void Start()
        {
            RegisterTypes();
        }

        private void RegisterTypes()
        {
            var emailContainer = Container.CreateChildContainer();
            emailContainer.Configure(RegisterMethods.EmailFoo);

            
            var smmsContainer = Container.CreateChildContainer();
            smmsContainer.Configure(RegisterMethods.SmsMmsFoo);

            
            Container.Configure(c =>
            {
                c.For<IGlobal>().Use<Global>();

                c.For<IContainer>().Use(emailContainer).Named("Email");
                c.For<IContainer>().Use(smmsContainer).Named("SmsMms");


                c.For<IEmailContainer>().Singleton().Use<EmailContainer>();
                c.For<ISmsMmsContainer>().Singleton().Use<SmsMmsContainer>();


                c.Profile("Email", RegisterMethods.EmailFoo);
                c.Profile("SmsMms", RegisterMethods.SmsMmsFoo);
            });
        }

        [TestMethod]
        public void ChildContainerByName()
        {
            var container = Container.GetInstance<IContainer>("Email");

            Assert.IsInstanceOfType(container, typeof(IContainer));
            Assert.IsNotNull(container);
        }

        [TestMethod]
        public void NewContainerByType()
        {
            var container = Container.GetInstance<IEmailContainer>();

            Assert.IsInstanceOfType(container, typeof(EmailContainer));
        }

        [TestMethod]
        public void ProfileContainerByName()
        {
            var container = Container.GetProfile("Email");

            Assert.IsInstanceOfType(container, typeof(IContainer));
            Assert.IsNotNull(container);
        }

        [TestMethod]
        public void FooFromChildContainerByName()
        {
            var container = Container.GetInstance<IContainer>("Email");
            var foo = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(foo, typeof(EmailFoo));
            Assert.IsNotNull(foo);
        }

        [TestMethod]
        public void FooFromNewContainerByType()
        {
            var container = Container.GetInstance<IEmailContainer>();
            var foo = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(foo, typeof(EmailFoo));
            Assert.IsNotNull(foo);
        }

        [TestMethod]
        public void FooFromProfileContainerByName()
        {
            var container = Container.GetProfile("Email");
            var foo = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(foo, typeof(EmailFoo));
            Assert.IsNotNull(foo);
        }

        [TestMethod]
        public void GlobalFromChildContainerByName()
        {
            var container = Container.GetInstance<IContainer>("Email");
            var global = container.GetInstance<IGlobal>();

            Assert.IsInstanceOfType(global, typeof(Global));
            Assert.IsNotNull(global);
        }

        [TestMethod]
        public void GlobalFromNewContainerByType()
        {
            var container = Container.GetInstance<IEmailContainer>();
            var global = container.GetInstance<IGlobal>();

            Assert.IsInstanceOfType(global, typeof(Global));
            Assert.IsNotNull(global);
        }

        [TestMethod]
        public void GlobalFromProfileContainerByName()
        {
            var container = Container.GetProfile("Email");
            var global = container.GetInstance<IGlobal>();

            Assert.IsInstanceOfType(global, typeof(Global));
            Assert.IsNotNull(global);
        }

        [TestMethod]
        public void AFromNewContainerByType()
        {
            var container = Container.GetInstance<IEmailContainer>();
            var a = container.GetInstance<A>();

            Assert.IsNotNull(a);
            Assert.IsNotNull(a.Foo);
            Assert.IsInstanceOfType(a.Foo, typeof(EmailFoo));
        }

        [TestMethod]
        public void AFromProfileContainerByName()
        {
            var container = Container.GetProfile("Email");
            var a = container.GetInstance<A>();

            Assert.IsNotNull(a);
            Assert.IsNotNull(a.Foo);
            Assert.IsInstanceOfType(a.Foo, typeof(EmailFoo));
        }

        [TestMethod]
        public void AFromChildContainerByName()
        {
            var container = Container.GetInstance<IContainer>("Email");
            var a = container.GetInstance<A>();

            Assert.IsNotNull(a);
            Assert.IsNotNull(a.Foo);
            Assert.IsInstanceOfType(a.Foo, typeof(EmailFoo));
        }
    }


    public class A
    {
        public IFoo Foo { get; set; }

        public A(IFoo foo)
        {
            Foo = foo;
        }
    }

    public class B
    {
        public IFoo Foo { get; set; }

        public B(IContainer container)
        {
            Foo = container.GetInstance<IFoo>();
        }
    }


    public static class RegisterMethods
    {
        public static Action<IProfileRegistry> EmailFoo
        {
            get
            {
                return c =>
                {
                    c.For<IFoo>().Use<EmailFoo>();
                };
            }
        }

        public static Action<IProfileRegistry> SmsMmsFoo
        {
            get
            {
                return c =>
                {
                    c.For<IFoo>().Use<SmsMmsFoo>();
                };
            }
        }
    }

    #region Container types
    public class EmailContainer : Container, IEmailContainer
    {
        public EmailContainer()
        {
            Configure(RegisterMethods.EmailFoo);
        }
    }

    public class SmsMmsContainer : Container, ISmsMmsContainer
    {
        public SmsMmsContainer()
        {
            Configure(RegisterMethods.SmsMmsFoo);
        }
    }

    public interface IEmailContainer : IContainer { }
    public interface ISmsMmsContainer : IContainer { }
    #endregion

    #region Foo types
    public class EmailFoo : IFoo { }
    public class SmsMmsFoo : IFoo { }
    public interface IFoo { }

    public class Global : IGlobal { }
    public interface IGlobal { }
    #endregion
}
