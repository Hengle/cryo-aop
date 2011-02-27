namespace CryoAOP.TestAssembly
{
    public class TestMethodInterceptorTypeDeepScope
    {
        public void InterceptMethod()
        {
        }

        public void CallToIntercept()
        {
            InterceptMethod();
        }
    }
}
