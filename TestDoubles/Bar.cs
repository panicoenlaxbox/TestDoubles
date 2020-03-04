namespace TestDoubles
{
    public class Bar
    {
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
