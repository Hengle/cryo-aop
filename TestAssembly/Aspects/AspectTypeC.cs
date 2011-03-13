using CryoAOP.Aspects;

namespace CryoAOP.TestAssembly.Aspects
{
    public class AspectTypeC
    {
        [Intercept]
        public void MethodThatShouldBeIntercepted()
        {
        }

        public void MethodThatShouldNotBeIntercepted()
        {
        }
    }
}