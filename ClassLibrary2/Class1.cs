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
}
