using CryoAOP.Core.Attributes;

namespace CryoAOP.TestAssembly
{
    public class PropertyInterceptorTarget
    {
        public int SomeInteger { get; set; }

        [Intercept]
        public int SomeIntegerWithAttribute { get; set; }

        [Intercept]
        public static int SomeStaticIntegerWithAttribute { get; set; }
    }
}