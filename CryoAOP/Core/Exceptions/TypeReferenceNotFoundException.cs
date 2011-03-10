using System;

namespace CryoAOP.Core.Exceptions
{
    public class TypeReferenceNotFoundException : FormattableExceptionBase
    {
        public TypeReferenceNotFoundException(string messageFormat, params object[] args) : base(messageFormat, args)
        {
        }

        public TypeReferenceNotFoundException(string messageFormat, Exception innerException, params object[] args) : base(messageFormat, innerException, args)
        {
        }
    }
}