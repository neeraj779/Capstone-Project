namespace Swar.API.Exceptions
{
    public class InvalidRefreshTokenException : Exception
    {
        string _message;

        public InvalidRefreshTokenException()
        {
            _message = "Invalid refresh token";
        }
        public override string Message => _message;
    }
}
