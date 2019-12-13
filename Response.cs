using ATS.Station_Model.States;

namespace ATS
{
    public class Response
    {
        public Response(ResponseState state, PhoneNumber source)
        {
            State = state;
            Source = source;

        }

        public ResponseState State { get; }
        public PhoneNumber Source { get; }
    }
}