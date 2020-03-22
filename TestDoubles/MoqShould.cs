using Xunit;
using Moq;
using FluentAssertions;
using Xunit.Abstractions;
using System;

namespace TestDoubles
{
    public class MoqShould
    {
        private readonly ITestOutputHelper _logger;

        public MoqShould(ITestOutputHelper logger)
        {
            _logger = logger;
        }

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
        public void create_a_partial_mock_with_ctor_arguments()
        {
            // Type has to be a class
            Mock<Qux> quxMock = new Mock<Qux>("quux", 10);
        }

        [Fact]
        public void create_a_partial_mock()
        {
            // A partial mock is created from a class
            Mock<Bar> barMock = new Mock<Bar>();
        }

        [Fact]
        public void argument_matching_for_concrete_value()
        {
            var fooMock = new Mock<IFoo>();
            fooMock.Setup(f => f.DoSomething("foo")).Returns(true);
            fooMock.Object.DoSomething("foo").Should().BeTrue();
        }

        [Fact]
        public void argument_matching_for_lazy_evaluated_value()
        {
            var fooMock = new Mock<IFoo>();
            fooMock.Setup(f => f.DoSomething(It.Is<string>(arg => arg == "foo"))).Returns(true);
            fooMock.Object.DoSomething("foo").Should().BeTrue();
        }

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

        [Fact]
        public void property_hierarchy()
        {
            Mock<IFoo> fooMock = new Mock<IFoo>();
            fooMock.Setup(f => f.Bar.Baz.Name).Returns("foo");
            fooMock.Object.Bar.Baz.Name.Should().Be("foo");
        }

        [Fact]
        public void generate_mock_instead_default_value()
        {
            Mock<IFoo> fooMock = new Mock<IFoo>
            {
                DefaultValue = DefaultValue.Mock
            };
            fooMock.Object.Bar.Baz.Name.Should().Be(null);
        }

        [Fact]
        public void verify_behavior()
        {
            Mock<IStockService> stockServiceMock = new Mock<IStockService>();
            IStockService stockService = stockServiceMock.Object;
            Mock<OrderService> orderServiceMock = new Mock<OrderService>(stockService);
            OrderService orderService = orderServiceMock.Object;
            orderService.Place("foo", 1);
            stockServiceMock.Verify(s => s.GetStock("foo", 1), Times.Once);
        }

        [Fact]
        public void verify_no_more_calls()
        {
            Mock<IStockService> stockServiceMock = new Mock<IStockService>();
            IStockService stockService = stockServiceMock.Object;
            Mock<OrderService> orderServiceMock = new Mock<OrderService>(stockService);
            OrderService orderService = orderServiceMock.Object;
            orderService.Place("foo", 1);
            Action act = () => stockServiceMock.VerifyNoOtherCalls();
            act.Should().Throw<MockException>()
                .WithMessage("*This mock failed verification due to the following unverified invocations:*");
        }
    }
}
