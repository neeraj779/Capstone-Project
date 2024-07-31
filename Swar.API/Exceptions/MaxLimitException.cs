namespace Swar.API.Exceptions
{
    public class MaxLimitException : Exception
    {
        string _message;

        public MaxLimitException()
        {
            _message = "Max playlist limit reached, you can only have 3 playlists";
        }

        public MaxLimitException(string message)
        {
            _message = message;
        }
        public override string Message => _message;
    }
}
