using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestDoubles")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace ClassLibrary1
{

    // If we want to mock these class we can to add InternalsVisibleTo
    internal class MyInternalClass
    {
        public virtual void DoSomething()
        {

        }
    }
}
