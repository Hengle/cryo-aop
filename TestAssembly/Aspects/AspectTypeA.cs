using CryoAOP.Core.Attributes;

namespace CryoAOP.TestAssembly.Aspects
{
    [Intercept]
    public class AspectTypeA
    {
        public void Method()
        {
        }

        public static void StaticMethod()
        {
        }
    }
}