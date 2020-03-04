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
            var foo = new Mock<IFoo>();
            foo.Setup(f => f.DoSomething(It.IsAny<string>())).Returns(true);

            foo.Object.DoSomething("foo").Should().BeTrue();
        }

        [Fact]
        public void create_a_mock_for_a_concrete_argument()
        {
            var foo = new Mock<IFoo>();
            foo.Setup(f => f.DoSomething("foo")).Returns(true);

            foo.Object.DoSomething("foo").Should().BeTrue();
        }

        [Fact]
        public void create_a_mock_for_lazy_evaluated_argument()
        {
            var foo = new Mock<IFoo>();
            foo.Setup(f => f.DoSomething(It.Is<string>((s) => s.Equals("foo", StringComparison.CurrentCultureIgnoreCase)))).Returns(true);

            foo.Object.DoSomething("foo").Should().BeTrue();
        }

        [Fact]
        public void create_a_partial_mock()
        {
            var bar = new Mock<Bar>();

            bar.Object.Submit().Should().BeFalse();
        }

        [Fact]
        public void create_a_mock_with_loose_mode()
        {
            var foo = new Mock<IFoo>();
            foo.Object.DoSomething("foo");
        }

        [Fact]
        public void create_a_mock_with_strict_mode()
        {
            var foo = new Mock<IFoo>(MockBehavior.Strict);
            Action act = () => foo.Object.DoSomething("foo");
            act.Should().Throw<MockException>().WithMessage("IFoo.DoSomething(\"foo\") invocation failed with mock behavior Strict*");
        }

        [Fact]
        public void create_a_mock_with_auto_property_tracking()
        {
            var mock = new Mock<IFoo>();

            mock.SetupProperty(f => f.Name);
            var foo = mock.Object;
            foo.Name = "foo";
            foo.Name.Should().Be("foo");
        }

        [Fact]
        public void create_a_mock_with_auto_property_tracking_and_initial_value()
        {
            var mock = new Mock<IFoo>();

            mock.SetupProperty(f => f.Name, "foo");
            var foo = mock.Object;
            foo.Name.Should().Be("foo");
        }

        [Fact]
        public void create_a_mock_with_all_properties_with_tracking()
        {
            var mock = new Mock<IFoo>();

            mock.SetupAllProperties();
            var foo = mock.Object;
            foo.Name = "foo";
            foo.Name.Should().Be("foo");
        }

        [Fact]
        public void create_a_mock_with_property_hierarchy()
        {
            var mock = new Mock<IFoo>();
            mock.Setup(f => f.Bar.Baz.Name).Returns("foo");
            mock.Object.Bar.Baz.Name.Should().Be("foo");
        }

        [Fact]
        public void create_a_mock_with_default_value_mock()
        {
            var mock = new Mock<IFoo>
            {
                DefaultValue = DefaultValue.Mock
            };
            mock.Object.Bar.Baz.Name.Should().Be(null);
        }

        [Fact]
        public void create_a_mock_and_verify_behavior()
        {
            var mockStockService = new Mock<IStockService>();
            var stockService = mockStockService.Object;
            var mockOrderService = new Mock<OrderService>(stockService);
            var orserService = mockOrderService.Object;
            orserService.Place("foo", 1);
            mockStockService.Verify(s => s.GetStock("foo", 1), Times.Once);
        }

        [Fact]
        public void create_a_mock_and_verify_no_more_calls()
        {
            var mockStockService = new Mock<IStockService>();
            var stockService = mockStockService.Object;
            var mockOrderService = new Mock<OrderService>(stockService);
            var orserService = mockOrderService.Object;
            orserService.Place("foo", 1);
            Action act = () => mockStockService.VerifyNoOtherCalls();
            act.Should().Throw<MockException>().WithMessage("*This mock failed verification due to the following unverified invocations:*");
        }
    }
}
