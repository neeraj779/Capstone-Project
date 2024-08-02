namespace Swar.API.Exceptions
{
    [Serializable]
    public class UnableToAddException : Exception
    {
        string _message;
        public UnableToAddException()
        {
            _message = "Uh oh! Something went wrong while adding; please try again.";
        }

        public UnableToAddException(string? message) : base(message)
        {
            _message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public override string Message => _message;
    }
}