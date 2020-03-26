using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using ClassLibrary2;

// Can not create proxy for type TestDoubles.BadClassWrapper because it is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] attribute, because assembly TestDoubles is not strong-named.
[assembly:InternalsVisibleTo("DynamicProxyGenAssembly2")]
// By default, types are internal, with public will not be problem
// Moq has no visibility on the internal member types
// DynamicProxyGenAssembly2 is used internally by Moq to generate proxy classes
namespace TestDoubles
{
    interface IBadClass
    {
        string BadProperty { get; set; }
        string BadMethod(string value);
    }

    class BadClass : IBadClass
    {
        public string BadProperty { get; set; }

        public string BadMethod(string value)
        {
            return value;
        }
    }

    class BadClassWrapper : IBadClass
    {
        private readonly BadClass _badClass;

        public BadClassWrapper(BadClass badClass)
        {
            _badClass = badClass;
        }

        // We could autoimplement property but property in BadClass could be calculated, so better call to the wrapped instance
        public virtual string BadProperty
        {
            get => _badClass.BadProperty;
            set => _badClass.BadProperty = value;
        }

        public virtual string BadMethod(string value)
        {
            return _badClass.BadMethod(value);
        }
    }

    internal class FixForBadClassWithProtectedInternalCtor : BadClassWithProtectedInternalCtor
    {


    }
}
