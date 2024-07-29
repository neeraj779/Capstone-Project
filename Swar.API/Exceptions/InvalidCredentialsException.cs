namespace Swar.API.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        string _message;

        public InvalidCredentialsException()
        {
            _message = "Email or password is incorrect";
        }

        public override string Message => _message;
    }
}
