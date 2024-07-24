namespace SongService.API.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        string _message;
        public EntityNotFoundException()
        {
            _message = "Entity not found";
        }

        public EntityNotFoundException(string? message) : base(message)
        {
            _message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public override string Message => _message;
    }
}
