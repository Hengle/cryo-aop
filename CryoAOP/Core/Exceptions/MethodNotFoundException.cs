using System;

namespace CryoAOP.Core.Exceptions
{
    public class MethodNotFoundException : FormattableExceptionBase
    {
        public MethodNotFoundException(string messageFormat, params object[] args)
            : base(messageFormat, args)
        {
        }

        public MethodNotFoundException(string messageFormat, Exception innerException, params object[] args)
            : base(messageFormat, innerException, args)
        {
        }
    }
}