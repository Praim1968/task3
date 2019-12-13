using System;
using ATS.Station_Model.States;

namespace ATS
{
    public class CallInfo
    {
        public CallInfo(PhoneNumber target, PhoneNumber source, TerminalState state)
        {
            Target = target;
            Source = source;
            State = state;
        }

        public PhoneNumber Target { get; }

        public PhoneNumber Source { get; }

        public DateTime TimeBegin { get; set; }

        public TimeSpan Duration { get; set; }
        public TerminalState State { get; }
        public double Cost { get; set; }

    }
}