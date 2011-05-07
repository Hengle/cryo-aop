using System;

namespace CryoAOP.Core.Exceptions
{
    public class MethodSignatureViolationException : FormattableExceptionBase
    {
        public MethodSignatureViolationException(string messageFormat, params object[] args) : base(messageFormat, args)
        {
        }

        public MethodSignatureViolationException(string messageFormat, Exception innerException, params object[] args)
            : base(messageFormat, innerException, args)
        {
        }
    }
}