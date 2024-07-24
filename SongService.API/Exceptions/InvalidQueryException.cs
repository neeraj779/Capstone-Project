namespace SongService.API.Exceptions
{
    public class InvalidQueryException : Exception
    {
        string _message;
        public InvalidQueryException()
        {
            _message = "Invalid query";
        }
        public override string Message => _message;
    }
}
