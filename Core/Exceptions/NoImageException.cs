namespace Core.Exceptions
{
    public class NoImageException : BaseException
    {
        public NoImageException()
            : base("Upload at least one image")
        {
        }
    }
}