using System;

namespace ClassLibrary2
{
    public class BadClassThatReturnsAClassWithProtectedInternalCtor
    {
        public virtual BadClassWithProtectedInternalCtor DoSomething()
        {
            return new BadClassWithProtectedInternalCtor();
        }
    }
    public class BadClassWithProtectedInternalCtor
    {
        protected internal BadClassWithProtectedInternalCtor()
        {

        }

        public void Foo()
        {

        }
    }

    public class BadClassWithPrivateCtor
    {
        private readonly string _value;

        protected internal BadClassWithPrivateCtor(string value)
        {
            _value = value;
        }

        public string GetValue() => _value;
    }
}
