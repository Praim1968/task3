using System;
using ATS.Station_Model.States;

namespace ATS.Station_Model.Intarfaces
{
    public interface IPort : IShouldClearEventHandlers
    {
        PortState State { get; set; }
        event EventHandler<PortState> StateChanging;//изменене состояние 
        event EventHandler<PortState> StateChanged;//Состояние Изменилось
        void RegisterEventHandlersForTerminal(ITerminal terminal);//Регистрация Обработчиков Событий Для Терминала
    }
}