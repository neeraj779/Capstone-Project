namespace Swar.API.Exceptions
{
    public class InactiveAccountException : Exception
    {
        string _message;

        public InactiveAccountException()
        {
            _message = "Account is inactive";
        }

        public InactiveAccountException(string message)
        {
            _message = message;
        }
        public override string Message => _message;
    }
}
