using CryoAOP.Aspects;

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