using System;
using ATS.BillingSystemModel.Intarfaces;

namespace ATS.Station_Model.Intarfaces
{
    public interface ITerminal : IShouldClearEventHandlers
    {
        PhoneNumber Number { get; }
        ITariffPlan TariffPlan { get; }
        //делаем чтобы дерннуть событие
        event EventHandler<CallInfo> OutgoingCall; //Пердаем Информацию О Звонке и Собраемся позвонить(событие) 
        event EventHandler<Response> Responce;//Передаем ответ и событе ответ 
        event EventHandler<PhoneNumber> IncomingRequest;//Передоем номер и событие входящий выозов 
        event EventHandler Plugging;//подключен
        event EventHandler UnPlugging;//не подключен 
        event EventHandler Online;//онлайн 
        event EventHandler Offline;//офлайн

        void GetReqest(PhoneNumber source);//сделать запрос ? проверка
        void Drop();//сбросить
        void Answer();//ответ 
        void Reject();//отклонить
        void Plug();//подключить
        void Unplug();//не подключать 
        void Call(PhoneNumber target);//звонок

        void RegisterEventHandlersForPort(IPort port);//Регистрация Обработчиков Событий Для Порта
    }
}