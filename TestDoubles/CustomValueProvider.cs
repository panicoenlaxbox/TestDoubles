using System;
using Moq;

namespace TestDoubles
{
    public class CustomValueProvider : DefaultValueProvider
    {
        protected override object GetDefaultValue(Type type, Mock mock)
        {
            if (type == typeof(string))
            {
                return "foo";
            }

            if (type == typeof(int))
            {
                return 10;
            }

            return Activator.CreateInstance(type);
        }
    }
}