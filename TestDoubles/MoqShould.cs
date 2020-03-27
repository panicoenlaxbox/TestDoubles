using Xunit;
using Moq;
using FluentAssertions;
using Xunit.Abstractions;
using System;
using System.Linq;
using System.Reflection;
using ClassLibrary1;
using ClassLibrary2;
using Moq.Protected;

namespace TestDoubles
{
    public class MoqShould
    {
        private readonly ITestOutputHelper _logger;

        public MoqShould(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        #region create

        [Fact]
        public void create_a_mock()
        {
            Mock<IFoo> fooMock = new Mock<IFoo>();
            IFoo foo = fooMock.Object;
        }

        [Fact]
        public void create_a_mock_with_static_method()
        {
            IFoo foo = Mock.Of<IFoo>();
            Mock<IFoo> fooMock = Mock.Get(foo);
        }

        [Fact]
        public void create_a_mock_with_static_method_and_lambda_in_ctor_for_setup()
        {
            // alternative and very odd syntax
            IFoo foo = Mock.Of<IFoo>(f =>
                f.DoSomethingStringy(
                    It.IsAny<string>()
                    ) == "foo", MockBehavior.Default);
            foo.DoSomethingStringy("bar").Should().Be("foo");
        }

        [Fact]
        public void create_a_partial_mock()
        {
            // A partial mock is created from a class
            Mock<Bar> barMock = new Mock<Bar>();
        }

        [Fact]
        public void create_a_partial_mock_with_ctor_arguments()
        {
            // Type has to be a class
            Mock<Qux> quxMock = new Mock<Qux>("quux", 10);
        }

        #endregion

        #region Callback

        [Fact]
        public void callback_like_alternative_for_instrumenting_a_mock()
        {
            var fooMock = new Mock<IFoo>();
            var counter = 0;
            fooMock.Setup(f => f.DoSomething(It.IsAny<string>())).Callback(() => { counter++; }).Returns(true);
            var foo = fooMock.Object;
            foo.DoSomething("foo");
            foo.DoSomething("bar");
            counter.Should().Be(2);
        }

        #endregion

        #region Throws

        [Fact]
        public void throws_exception()
        {
            var fooMock = new Mock<IFoo>();
            fooMock.Setup(f => f.DoSomething(It.IsAny<string>())).Throws<DivideByZeroException>();
            Action act = () => { fooMock.Object.DoSomething("foo"); };
            act.Should().Throw<DivideByZeroException>();
        }

        [Fact]
        public void throws_a_new_exception()
        {
            var fooMock = new Mock<IFoo>();
            fooMock.Setup(f => f.DoSomething(It.IsAny<string>())).Throws(new DivideByZeroException());
            Action act = () => { fooMock.Object.DoSomething("foo"); };
            act.Should().Throw<DivideByZeroException>();
        }

        #endregion

        #region Callbase

        [Fact]
        public void callbase()
        {
            Mock<Bar> barMock = new Mock<Bar>();
            // CallBase will call the implementation of a virtual method
            barMock.Setup(f => f.Submit()).CallBase();
            barMock.Object.Submit().Should().BeTrue();
        }

        #endregion

        #region Values in sequence

        [Fact]
        public void sequence()
        {
            var fooMock = new Mock<IFoo>();
            fooMock.SetupSequence(f => f.DoSomethingStringy(It.IsAny<string>()))
                .Returns("first")
                .Throws<DivideByZeroException>()
                .Returns("third")
                .Returns(() => "bar");
            fooMock.Object.DoSomethingStringy("foo").Should().Be("first");
            Action act = () => fooMock.Object.DoSomethingStringy("foo").Should().Be("second");
            act.Should().Throw<DivideByZeroException>();
            fooMock.Object.DoSomethingStringy("foo").Should().Be("third");
            fooMock.Object.DoSomethingStringy("foo").Should().Be("bar");
        }

        #endregion

        #region argument_matching

        [Fact]
        public void argument_matching_for_concrete_value()
        {
            var fooMock = new Mock<IFoo>();
            fooMock.Setup(f => f.DoSomething("foo")).Returns(true);
            fooMock.Object.DoSomething("foo").Should().BeTrue();
        }

        [Fact]
        public void argument_matching_for_any_value()
        {
            var fooMock = new Mock<IFoo>();
            fooMock.Setup(f => f.DoSomething(It.IsAny<string>())).Returns(true);
            fooMock.Object.DoSomething("foo").Should().BeTrue();
        }

        [Fact]
        public void argument_matching_for_lazy_evaluated_value()
        {
            var fooMock = new Mock<IFoo>();
            fooMock.Setup(f => f.DoSomething(It.Is<string>(arg => arg == "foo"))).Returns(true);
            fooMock.Object.DoSomething("foo").Should().BeTrue();
        }

        #endregion

        #region Behavior

        [Fact]
        public void create_a_mock_in_loose_mode()
        {
            // By default, Default that it's the same that Loose
            Mock<IFoo> fooMock = new Mock<IFoo>();
            Mock<IFoo> otherFooMock = new Mock<IFoo>(MockBehavior.Loose);
        }

        [Fact]
        public void create_a_mock_in_strict_mode()
        {
            // By default, a mock is created in Loose mode
            Mock<IFoo> fooMock = new Mock<IFoo>(MockBehavior.Strict);
            Action act = () => fooMock.Object.DoSomething("foo");
            act.Should().Throw<MockException>()
                .WithMessage("IFoo.DoSomething(\"foo\") invocation failed with mock behavior Strict*");
        }

        #endregion

        #region Properties

        [Fact]
        public void setup_property_getter_and_setter()
        {
            Mock<IFoo> fooMock = new Mock<IFoo>();
            // SetupGet and SetupSet allows configure mock, but not persist the value
            fooMock.SetupGet(f => f.Name).Returns("name");
            fooMock.SetupSet(f => f.Name = "Sergio").Callback(() =>
            {
                _logger.WriteLine("Sergio");
            }).Verifiable(); // Will be verified with Verify
            fooMock.Object.Name = "Sergio";
            fooMock.Verify();
        }

        [Fact]
        public void auto_property_tracking()
        {
            Mock<IFoo> fooMock = new Mock<IFoo>();
            // Without SetupProperty, a property of a mock will not do tracking
            fooMock.SetupProperty(f => f.Name);
            IFoo foo = fooMock.Object;
            foo.Name = "foo";
            foo.Name.Should().Be("foo");
        }

        [Fact]
        public void auto_property_tracking_with_initial_value()
        {
            Mock<IFoo> fooMock = new Mock<IFoo>();
            fooMock.SetupProperty(f => f.Name, "foo");
            IFoo foo = fooMock.Object;
            foo.Name.Should().Be("foo");
        }

        [Fact]
        public void auto_property_tracking_for_all_properties()
        {
            Mock<IFoo> fooMock = new Mock<IFoo>();
            fooMock.SetupAllProperties();
            IFoo foo = fooMock.Object;
            foo.Name = "foo";
            foo.Name.Should().Be("foo");
        }

        #endregion

        #region DefaultValue

        [Fact]
        public void generate_default_value()
        {
            Mock<IFoo> fooMock = new Mock<IFoo>
            {
                // Specifies the behavior to use when returning default values for unexpected invocation on loose mocks
                DefaultValue = DefaultValue.Empty // Empty by default
            };
            fooMock.Object.DoSomething("foo").Should().BeFalse();
        }

        [Fact]
        public void generate_mock_instead_default_value()
        {
            Mock<IFoo> fooMock = new Mock<IFoo>
            {
                // if the type can be mocked, do it
                DefaultValue = DefaultValue.Mock
            };
            // these works because Bar and Baz are mocked
            fooMock.Object.Bar.Baz.Name.Should().Be(null);
        }

        [Fact]
        public void generate_mock_instead_default_value_with_custom_value_provider()
        {
            Mock<IFoo> fooMock = new Mock<IFoo>
            {
                DefaultValueProvider = new CustomValueProvider()
            };
            // "foo" is now the default value for string when not it's configured
            fooMock.Object.DoSomethingStringy("x").Should().Be("foo");
        }

        #endregion

        #region property hierarchy

        [Fact]
        public void property_hierarchy()
        {
            Mock<IFoo> fooMock = new Mock<IFoo>();
            fooMock.Setup(f => f.Bar.Baz.Name).Returns("foo");
            fooMock.Object.Bar.Baz.Name.Should().Be("foo");
        }

        #endregion

        #region Verify

        // Verify internal behavior of the sut (right calls, right parameters, right order call, etc.), through the expectations configured in the mock object

        [Fact]
        public void verify_behavior()
        {
            Mock<IStockService> stockServiceMock = new Mock<IStockService>();
            Mock<OrderService> orderServiceMock = new Mock<OrderService>(stockServiceMock.Object);
            orderServiceMock.Object.Place("foo", 1); // comment/uncomment
            stockServiceMock.Verify(s => s.GetStock("foo", 1), Times.Once);
        }

        [Fact]
        public void verify_verifiable_behavior()
        {
            var mockFoo = new Mock<IFoo>();
            mockFoo.Setup(foo => foo.DoSomethingStringy("foo")).Returns("foo")
                .Verifiable(); // Marks the expectation as verifiable, so a call to Verify will check if this expectation was met
            mockFoo.Object.DoSomethingStringy("foo"); // comment/uncomment
            // Verifies that all verifiable expectations have been met 
            mockFoo.Verify();
        }

        [Fact]
        public void verify_verifiable_behavior_with_static_method()
        {
            var mockFoo = new Mock<IFoo>();
            mockFoo.Setup(foo => foo.DoSomethingStringy("foo")).Returns("foo")
                .Verifiable(); // Marks the expectation as verifiable, so a call to Verify will check if this expectation was met
            mockFoo.Object.DoSomethingStringy("foo"); // comment/uncomment
            // Verifies that all verifiable expectations have been met 
            Mock.Verify(mockFoo);
        }

        [Fact]
        public void verify_all_behavior_regardless_is_or_not_verifiable()
        {
            var mockFoo = new Mock<IFoo>();
            mockFoo.Setup(foo => foo.DoSomethingStringy("foo")).Returns("foo");
            mockFoo.Object.DoSomethingStringy("foo"); // comment/uncomment
            // Verifies that all expectations (with or without verifiable method) have been met 
            mockFoo.VerifyAll();
        }

        [Fact]
        public void verify_all_behavior_regardless_is_or_not_verifiable_with_static_method()
        {
            var mockFoo = new Mock<IFoo>();
            mockFoo.Setup(foo => foo.DoSomethingStringy("foo")).Returns("foo");
            mockFoo.Object.DoSomethingStringy("foo"); // comment/uncomment
            // Verifies that all expectations (with or without verifiable method) have been met 
            Mock.VerifyAll(mockFoo);
        }

        [Fact]
        public void verify_no_others_calls()
        {
            Mock<IStockService> stockServiceMock = new Mock<IStockService>();
            Mock<OrderService> orderServiceMock = new Mock<OrderService>(stockServiceMock.Object);
            orderServiceMock.Object.Place("foo", 1);
            stockServiceMock.Verify(s => s.GetStock("foo"), Times.Once);
            stockServiceMock.Verify(s => s.GetStock("foo", 1), Times.Once);
            // verify that only calls in previous verify methods has been made
            stockServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void view_calls_made_to_our_mock_with_no_others_calls_hack()
        {
            Mock<IStockService> stockServiceMock = new Mock<IStockService>();
            Mock<OrderService> orderServiceMock = new Mock<OrderService>(stockServiceMock.Object);
            orderServiceMock.Object.Place("foo", 1);

            /*
             This mock failed verification due to the following unverified invocations:
   IStockService.GetStock("foo")
   IStockService.GetStock("foo", 1)
             */

            // verify that only calls in previous verify methods has been made
            stockServiceMock.VerifyNoOtherCalls();
        }

        #endregion

        #region Protected

        [Fact]
        public void configure_protected_method()
        {
            var mockDog = new Mock<Dog>();
            // method is a string
            // ItExpr instead it
            mockDog
                .Protected()
                .Setup<string>("Sound", ItExpr.IsAny<int>()).Returns("hola");
            mockDog.Object.DoSomething(3).Should().Be("hola");
        }

        #endregion

        #region Repositories

        // Central point to create mocks with unified behavior. Furthermore, only one place to verify

        [Fact]
        public void repositories()
        {
            // MockBehavior, CallBase, DefaultValue and DefaultValueProvider for all mocks created with this repository

            MockRepository repository = new MockRepository(MockBehavior.Default);
            repository.CallBase = false;
            repository.DefaultValue = DefaultValue.Empty;
            //repository.DefaultValueProvider = new CustomValueProvider();

            // Create mock from repository, three options

            // Of<T>, LINQ syntax
            var foo1 = repository
                .Of<IFoo>()
                .First(foo => foo.DoSomething(It.IsAny<string>()) == true);

            // OneOf<T> is similar to Mock.Of<T>
            var foo2 = repository.OneOf<IFoo>();
            Mock.Get(foo2).Setup(foo => foo.DoSomething(It.IsAny<string>())).Returns(true);

            // Create<T> is similar to new Mock<T>
            var fooMock = repository.Create<IFoo>();
            fooMock.Setup(foo => foo.DoSomething(It.IsAny<string>())).Returns(true);
            var foo3 = fooMock.Object;

            // All mocks can be verified all together
            //repository.Verify();
            //repository.VerifyAll();
            //repository.VerifyNoOtherCalls();
        }

        #endregion

        #region BadClass

        [Fact]
        public void bad_method()
        {
            var bcMock = new Mock<BadClass>();
            // System.NotSupportedException : Unsupported expression: b => b.BadMethod(It.IsAny<string>())
            // Non-overridable members (here: BadClass.BadMethod) may not be used in setup / verification expressions.
            bcMock.Setup(b => b.BadMethod(It.IsAny<string>())).Returns("bar");
        }

        [Fact]
        public void bad_property()
        {
            var bcMock = new Mock<BadClass>();
            // System.NotSupportedException : Unsupported expression: b => b.BadProperty
            // Non-overridable members (here: BadClass.get_BadProperty) may not be used in setup / verification expressions.
            bcMock.SetupProperty(b => b.BadProperty);
        }

        [Fact]
        public void solution_via_interface_and_wrapper_class()
        {
            // first, extract interface from bad class -> IBadClass
            //  we assume that we can not implement this new interface in the BadClass
            // create a new class that acts like a wrapper and implement IBadClass
            // inject bad class into the new wrapper class and pass-through methods and properties
            //  -> proxy design pattern
            // now mock the new interface or the new class (if you have added virtual to members)

            //var bcMock = new Mock<IBadClass>();
            var bcMock = new Mock<BadClassWrapper>(new BadClass());
            bcMock.Setup(b => b.BadMethod(It.IsAny<string>())).Returns("bar");
            bcMock.Object.BadMethod("foo").Should().Be("bar");
        }

        [Fact]
        public void solution_for_method_that_returns_an_object_with_protected_internal_constructor()
        {
            var bcMock = new Mock<BadClassThatReturnsAClassWithProtectedInternalCtor>();
            bcMock.Setup(b => b.DoSomething()).Returns(() =>
            {
                // BadClassWithInternalCtor.BadClassWithInternalCtor()' is inaccessible due to its protection level
                //return new BadClassWithProtectedInternalCtor();

                // We hace created a new type that inherits from a class with a protected internal and with polymorphism we can continue working
                return new FixForBadClassWithProtectedInternalCtor();
            });
            // BeAssignableTo OK
            // BeOfType KO, because FixForBadClassWithProtectedInternalCtor inherits from FixForBadClassWithProtectedInternalCtor
            var f = bcMock.Object.DoSomething();
            f.Should().BeAssignableTo<BadClassWithProtectedInternalCtor>();
            f.Foo(); // Polymorphism
        }

        [Fact]
        public void create_object_with_private_ctor_with_reflection()
        {
            Type type = typeof(BadClassWithPrivateCtor);
            ConstructorInfo? constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof(string)}, null);
            object obj = constructor.Invoke(new[]{"foo"});
            BadClassWithPrivateCtor instance = (BadClassWithPrivateCtor)obj;
            instance.GetValue().Should().Be("foo");
        }

        #endregion
    }
}
