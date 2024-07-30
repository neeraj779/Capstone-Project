namespace Swar.API.Exceptions
{
    public class WeakPasswordException : Exception
    {
        string _message;

        public WeakPasswordException()
        {
            _message = "Password is too weak. Please use a stronger password.";
        }
        public override string Message => _message;
    }
}
