using System;
using CryoAOP.Core.Extensions;

namespace CryoAOP.Core.Exceptions
{
    public abstract class FormattableExceptionBase : Exception
    {
        protected FormattableExceptionBase(string messageFormat, params object[] args)
            : base(messageFormat.FormatWith(args))
        {
        }

        protected FormattableExceptionBase(string messageFormat, Exception innerException, params object[] args)
            : base(messageFormat.FormatWith(args), innerException)
        {
        }
    }
}