using CryoAOP.TestAssembly.Aspects;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class MethodAspectTests
    {
        [Test]
        public void Should_intercept_aspects()
        {
            var wasIntercepted = false;
            var aspectTypeA = new AspectTypeA();
            aspectTypeA.WhenCalled<AspectTypeA>((i) => wasIntercepted = true);
            aspectTypeA.Method();
            Assert.That(wasIntercepted);
        }

        [Test]
        public void Mixins_should_not_cross_intercept_calls_on_types_for_instances()
        {
            var aspectAWasIntercepted = false;
            var aspectTypeA = new AspectTypeA();
            aspectTypeA.WhenCalled<AspectTypeA>((i) => aspectAWasIntercepted = true);

            var aspectBWasIntercepted = false;
            var aspectTypeB = new AspectTypeA();
            aspectTypeB.WhenCalled<AspectTypeB>((i) => aspectBWasIntercepted = true);

            aspectTypeA.Method();

            Assert.That(aspectAWasIntercepted && !aspectBWasIntercepted);
        }
    }
}