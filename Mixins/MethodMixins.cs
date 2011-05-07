using System;
using CryoAOP.Core;
using CryoAOP.Core.Attributes;

namespace CryoAOP.Mixins
{
    public interface IMethodMixins
    {
        void WhenMethodCalled(Action<Invocation> invocation);
    }

    public class MethodMixins
    {
        [MixinMethod]
        public void WhenCalled<T>(Action<Invocation> invocation)
        {
            Intercept.Call +=
                (i) =>
                    {
                        if (i.Type == typeof (T))
                            invocation(i);
                    };
        }

        [MixinMethod]
        public static void WhenCalledStatic<T>(Action<Invocation> invocation)
        {
            Intercept.Call +=
                (i) =>
                    {
                        if (i.Type == typeof (T))
                            invocation(i);
                    };
        }
    }
}