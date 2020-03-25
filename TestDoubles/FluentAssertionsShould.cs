using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

namespace TestDoubles
{
    public class FluentAssertionsShould
    {
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
    }
}
