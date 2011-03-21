using System;

namespace CryoAOP.Core.Exceptions
{
    public class PropertyNotFoundException : FormattableExceptionBase
    {
        public PropertyNotFoundException(string messageFormat, params object[] args)
            : base(messageFormat, args)
        {
        }

        public PropertyNotFoundException(string messageFormat, Exception innerException, params object[] args)
            : base(messageFormat, innerException, args)
        {
        }
    }
}