using CryoAOP.Core.Attributes;

namespace CryoAOP.TestAssembly.Aspects
{
    public class AspectTypeC
    {
        [Intercept]
        public int PropertyThatShouldBeIntercepted { get; set; }

        [Intercept]
        public void MethodThatShouldBeIntercepted()
        {
        }

        public void MethodThatShouldNotBeIntercepted()
        {
        }
    }
}