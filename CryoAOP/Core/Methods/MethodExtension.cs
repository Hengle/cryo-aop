namespace CryoAOP.Core.Methods
{
    internal class MethodExtension
    {
        protected readonly MethodContext Context;

        public MethodExtension(MethodContext context)
        {
            Context = context;
        }

        protected Type Type
        {
            get { return Context.Type; }
        }
    }
}