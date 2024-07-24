namespace SongService.API.Exceptions
{
    public class InvalidSongIdException : Exception
    {
        string _message;
        public InvalidSongIdException()
        {
            _message = "Invalid song id";
        }
        public override string Message => _message;
    }
}
