namespace Swar.API.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        string _message;

        public InvalidCredentialsException()
        {
            _message = "Email or password is incorrect";
        }

        public InvalidCredentialsException(string message)
        {
            _message = message;
        }

        public override string Message => _message;
    }
}
