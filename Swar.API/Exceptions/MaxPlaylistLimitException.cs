namespace Swar.API.Exceptions
{
    public class MaxPlaylistLimitException : Exception
    {
        string _message;

        public MaxPlaylistLimitException()
        {
            _message = "Max playlist limit reached, you can only have 3 playlists";
        }
        public override string Message => _message;
    }
}
