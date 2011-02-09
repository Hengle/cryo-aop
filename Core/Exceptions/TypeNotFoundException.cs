namespace CryoAOP.Core.Exceptions
{
    public class TypeNotFoundException : FormattableExceptionBase
    {
        public TypeNotFoundException(string messageFormat, params object[] args)
            : base(messageFormat, args)
        {
        }

        public TypeNotFoundException(string messageFormat, FormattableExceptionBase innerException,
                                                      params object[] args)
            : base(messageFormat, innerException, args)
        {
        }
    }
}