using System;

namespace TestDoubles
{
    public class Qux
    {
        public Qux(string quux, int corge)
        {
            if (quux == null) throw new ArgumentNullException(nameof(quux));
            if (corge <= 0) throw new ArgumentOutOfRangeException(nameof(corge));
        }
    }
}