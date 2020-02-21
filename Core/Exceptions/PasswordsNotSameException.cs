namespace Core.Exceptions
{
    public class PasswordsNotSameException : BaseException
    {
        public PasswordsNotSameException() : base("Passwords not matched")
        {
        }
    }
}