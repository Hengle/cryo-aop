using System;

namespace CryoAOP.Aspects
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class InterceptAttribute : Attribute
    {
        private readonly MethodInterceptionScopeType interceptionScope = MethodInterceptionScopeType.Shallow;

        public InterceptAttribute()
        {
        }

        public InterceptAttribute(MethodInterceptionScopeType interceptionScope)
        {
            this.interceptionScope = interceptionScope;
        }

        public MethodInterceptionScopeType Scope
        {
            get { return interceptionScope; }
        }
    }
}