namespace TestDoubles
{
    public abstract class Animal
    {
        protected abstract string Sound(int times);
    }

    public class Dog : Animal
    {
        protected override string Sound(int times)
        {
            var m = "";
            for (var i = 0; i < times; i++)
            {
                m += "guau!";
            }

            return m;
        }

        public string DoSomething(int times)
        {
            return Sound(times);
        }
    }
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
    }
}
