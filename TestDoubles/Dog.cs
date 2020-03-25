namespace TestDoubles
{
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
}