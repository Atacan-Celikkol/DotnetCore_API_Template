namespace Core.Exceptions
{
    public class WrongPasswordException : BaseException
    {
        public WrongPasswordException()
            : base("Wrong Password")
        {
        }
    }
}