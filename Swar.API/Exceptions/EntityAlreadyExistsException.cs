namespace Swar.API.Exceptions
{
    public class EntityAlreadyExistsException : Exception
    {
        string _message;

        public EntityAlreadyExistsException()
        {
            _message = "Entity already exists in the database";
        }

        public EntityAlreadyExistsException(string message)
        {
            _message = message;
        }

        public override string Message => _message;
    }
}
