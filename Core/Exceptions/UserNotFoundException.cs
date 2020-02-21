namespace Core.Exceptions
{
    public class UserNotFoundException : BaseException
    {
        public UserNotFoundException()
            : base(@"Kullanıcı bilgisine ulaşılamadı.")
        {
        }
    }
}