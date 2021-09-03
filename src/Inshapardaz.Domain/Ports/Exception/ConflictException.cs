namespace Inshapardaz.Domain.Exception
{
    public class ConflictException
        : System.Exception
    {
        public ConflictException()
        {
        }

        public ConflictException(string message)
            : base(message)
        {
        }
    }
}
