using ATS.Station_Model.Intarfaces;

namespace ATS.User_Model
{
    public interface IUser
    {
        ITerminal Phone { get; }
        void Call(PhoneNumber target);
        void Drop();
        void Answer();
        void Reject();
        void Plug();
        void UnPlug();
    }
}