using System;

namespace CryoAOP.Core.Attributes
{
    [AttributeUsage(
        AttributeTargets.Method
        | AttributeTargets.Property
        | AttributeTargets.Class)]
    public class InterceptAttribute : Attribute
    {
        private readonly MethodInterceptionScopeType interceptionScope = MethodInterceptionScopeType.Deep;

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