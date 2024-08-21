namespace Swar.API.Exceptions
{
    public class ExternalServiceLoginException : Exception
    {
        string _message;

        public ExternalServiceLoginException()
        {
            _message = "Your account was created using an external authentication service. Please log in using the same service.";
        }
        public override string Message => _message;
    }
}
