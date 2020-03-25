using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Extensions;
using FluentAssertions.Numeric;
using FluentAssertions.Primitives;
using Xunit;

namespace TestDoubles
{
    public class FluentAssertionsShould
    {
        [Fact]
        public void datetime_and_timespan()
        {
            DateTime dateTime = new DateTime(2020, 3, 25, 12, 30, 59);
            DateTime fluentDateTime = 25.March(2020).At(12, 30, 59);
            
            TimeSpan timeSpan = TimeSpan.FromHours(20);
            TimeSpan fluentTimeSpan = 20.Hours();

            TimeSpan timeSpan2 = new TimeSpan(12, 30, 15);
            TimeSpan fluentTimeSpan2 = 12.Hours().And(30.Minutes().And(15.Seconds()));
        }

        [Fact]
        public void assert_date()
        {
            var sut = new ClassForAsserting();
            var expected = new DateTime(2020, 3, 25, 12, 30, 59);
            var actual = sut.GetDate();

            Assert.Equal(expected, actual);
            /*
Assert.Equal() Failure
Expected: 2020-03-25T12:30:58.0000000
Actual:   2020-03-25T12:30:59.0000000
             */

            // more semantic
            // framework agnostic
            // fluent syntax
            // more failed detailed message
            actual.Should().Be(25.March(2020).At(12, 30, 59));
            /*
Expected actual to be <2020-03-25 12:30:58>, but found <2020-03-25 12:30:59>.             
             */
        }

        [Fact]
        public void and()
        {
            AndConstraint<NumericAssertions<int>> and = 5.Should().Be(5);

            5.Should().BeGreaterThan(4).And.Should().Be(5);
        }

        [Fact]
        public void which()
        {
            var argumentException = new ArgumentException("failed");
            // with BeOfType<>, BeAssignableTo<>, etc
            AndWhichConstraint<ObjectAssertions, Exception> andWhichConstraint = argumentException.Should().BeAssignableTo<Exception>();
            // now, I can continue with the type specified
            Exception exception = andWhichConstraint.Which;
            exception.Message.Should().NotBeNullOrWhiteSpace();

            // real world sentence
            (new ArgumentException("failed")).Should().BeAssignableTo<Exception>().Which.Message.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void custom_message()
        {
            var value = 5;
            value.Should().BeGreaterThan(expected: 6, because: "El número {0} no me vale :(", value);
            // because is a string.Format
            // becauseArgs is params

            /*
Expected value to be greater than 6 because El número 5 no me vale :(, but found 5.             
             */
        }

        [Fact]
        public void collection()
        {
            var list = new List<int>() {1,2,3,4,5};

            using (new AssertionScope())
            {
                list.Count.Should().Be(0); // Expected list.Count to be 0, but found 5.

                // more specific
                // better message
                list.Should().HaveCount(0); // Expected list to contain 0 item(s), but found 5.
            }
        }
    }
}
