using CryoAOP.Core.Attributes;

namespace CryoAOP.TestAssembly.Aspects
{
    [Intercept]
    public class AspectTypeB
    {
        public void Method()
        {
        }

        public static void StaticMethod()
        {
        }
    }
}