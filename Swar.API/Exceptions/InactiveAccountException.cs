namespace Swar.API.Exceptions
{
    public class InactiveAccountException : Exception
    {
        string _message;

        public InactiveAccountException()
        {
            _message = "Account is inactive";
        }
        public override string Message => _message;
    }
}
