namespace CryoAOP.Core.Exceptions
{
    public class AssemblyNotFoundException : FormattableExceptionBase
    {
        public AssemblyNotFoundException(string messageFormat, params object[] args)
            : base(messageFormat, args)
        {
        }

        public AssemblyNotFoundException(string messageFormat, FormattableExceptionBase innerException,
                                         params object[] args)
            : base(messageFormat, innerException, args)
        {
        }
    }
}