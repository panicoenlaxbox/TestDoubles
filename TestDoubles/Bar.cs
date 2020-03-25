using System;

namespace TestDoubles
{
    public class Bar
    {
        public Bar(Baz baz)
        {
            Baz = baz;
        }

        public Bar()
        {
            
        }
        public virtual Baz Baz { get; set; }
        public virtual bool Submit()
        {
            return true;
        }
        public string Submit(string value)
        {
            return value;
        }

        public void DivideByZero()
        {
            throw  new DivideByZeroException();
        }

        public void DivideByZeroWithInnerException()
        {
            throw  new DivideByZeroException(null, new ArgumentException());
        }
    }
}
