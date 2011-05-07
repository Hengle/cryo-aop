using System;

namespace CryoAOP.Core.Exceptions
{
    public class IncorrectAttributeResultUsageException : FormattableExceptionBase
    {
        public IncorrectAttributeResultUsageException(string messageFormat, params object[] args)
            : base(messageFormat, args)
        {
        }

        public IncorrectAttributeResultUsageException(string messageFormat, Exception innerException,
                                                      params object[] args) : base(messageFormat, innerException, args)
        {
        }
    }
}