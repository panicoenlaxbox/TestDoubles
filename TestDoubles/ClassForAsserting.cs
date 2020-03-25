using System;
using System.Collections.Generic;
using System.Text;

namespace TestDoubles
{
    class ClassForAsserting
    {
        public DateTime GetDate()
        {
            return new DateTime(2020, 3, 25, 12, 30, 59);
        }
    }
}
