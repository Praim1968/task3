using System;

namespace ATS.Station_Model.Intarfaces
{
    public interface IStation : IShouldClearEventHandlers
    {
        event EventHandler<CallInfo> CallInfoPrepared;
        void RegisterEventHandlersForTerminal(ITerminal terminal);//регестраця оброботчка события для терминала 
        void RegisterEventHandlersForPort(IPort port);//регестраця оброботчка события для порта 
    }
}