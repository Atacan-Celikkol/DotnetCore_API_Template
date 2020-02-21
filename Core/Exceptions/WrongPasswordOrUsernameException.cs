namespace Core.Exceptions
{
    public class WrongPasswordOrUsernameException : BaseException
    {
        public WrongPasswordOrUsernameException()
            : base("Wrong Password or Username")
        {
        }
    }
}