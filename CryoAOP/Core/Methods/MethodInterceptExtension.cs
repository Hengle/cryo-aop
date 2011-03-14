namespace CryoAOP.Core.Methods
{
    internal class MethodInterceptExtension
    {
        protected readonly MethodInterceptContext Context;

        public MethodInterceptExtension(MethodInterceptContext context)
        {
            Context = context;
        }

        protected TypeIntercept TypeIntercept
        {
            get { return Context.TypeIntercept; }
        }
    }
}