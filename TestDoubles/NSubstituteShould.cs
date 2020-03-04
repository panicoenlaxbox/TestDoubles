using Xunit;
using FluentAssertions;
using Xunit.Abstractions;
using NSubstitute;

namespace TestDoubles
{
    public class NSubstituteShould
    {
        private readonly ITestOutputHelper _logger;

        public NSubstituteShould(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        [Fact]
        public void create_a_mock()
        {
            var foo = Substitute.For<IFoo>();
            foo.DoSomething("foo").Returns(true);

            foo.DoSomething("foo").Should().BeTrue();
        }
    }
}
