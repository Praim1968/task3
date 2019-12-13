using ATS.Station_Model.Intarfaces;

namespace ATS.User_Model
{
    public class User : IUser
    {
        public ITerminal Phone { get; set; }

        public void Call(PhoneNumber target)
        {
            Phone.Call(target);
        }

        public void Drop()
        {
            Phone.Drop();
        }

        public void Answer()
        {
            Phone.Answer();
        }

        public void Reject()
        {
            Phone.Reject();
        }

        public void Plug()//подключить
        {
            Phone.Plug();
        }

        public void UnPlug()
        {
            Phone.Unplug();
        }
    }
}