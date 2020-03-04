using System;
using Xunit;
using Moq;
using FluentAssertions;

namespace TestDoubles
{
    public class MoqShould
    {
        [Fact]
        public void create_a_mock()
        {
            var foo = new Mock<IFoo>();
            
            foo.Object.DoSomething("foo").Should().BeFalse();

            foo.Setup(f=>f.DoSomething("foo")).Returns(true);
            
            foo.Object.DoSomething("foo").Should().BeTrue();
        }
    }

    public interface IFoo
    {
        Bar Bar { get; set; }
        string Name { get; set; }
        int Value { get; set; }
        bool DoSomething(string value);
        bool DoSomething(int number, string value);
        string DoSomethingStringy(string value);
        bool TryParse(string value, out string outputValue);
        bool Submit(ref Bar bar);
        int GetCount();
        bool Add(int value);
    }

    public class Bar
    {
        public virtual Baz Baz { get; set; }
        public virtual bool Submit() { return false; }
    }

    public class Baz
    {
        public virtual string Name { get; set; }
    }
}
