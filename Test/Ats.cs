using System;
using System.Collections.Generic;
using System.Linq;
using ATS.Helpers;
using ATS.Station_Model.AbstractClasses;
using ATS.Station_Model.Intarfaces;
using ATS.Station_Model.States;

namespace ATS.Test
{
    public class Ats : Station
    {
        public Ats(ICollection<IPort> ports, ICollection<ITerminal> terminals) : base(ports, terminals)
        {
        }

        protected override void InnerConnectionHandler(object sender, CallInfo callInfo)//Обработчик Внутренних Соединений
        {
            var targetPort = GetPortByPhoneNumber(callInfo.Target);//Получить Порт По Номеру Телефона
            if (targetPort.State == PortState.Unpluged || targetPort.State == PortState.Call ||
                callInfo.Source == callInfo.Target)
            {
                SetTerminalStateTo(callInfo.Source, TerminalState.Free); //Установите Состояние В Терминала

                callInfo.TimeBegin = TimeHelper.Now;
                callInfo.Duration = TimeSpan.Zero;
                OnCallInfoPrepared(this, callInfo);//По Вызову Информация Подготовлена
            }
            else
            {
                SetPortsStateTo(callInfo.Source, callInfo.Target, PortState.Call);//Установите Состояние Портов В
                SetTerminalStateTo(callInfo.Source, TerminalState.OutGoingCall);//Установите Состояние Термнала
                SetTerminalStateTo(callInfo.Target, TerminalState.IncomingCall);//Установите Состояние Термнала
                var targetTerminal = _terminalCollection.FirstOrDefault(x => x.Number == callInfo.Target);

                targetTerminal?.GetReqest(callInfo.Source);
                _callInfoCollection.Add(callInfo);
                _waitActionTerminals.Add(targetTerminal);
            }
        }

        private void SetTerminalStateTo(PhoneNumber source, TerminalState state)//Установите Состояние Терминала
        {
            var sourceTerminal = GetTerminalByPhoneNumber(source) as TestTerminal;//Получить Терминал По Номеру Телефона

            if (sourceTerminal != null) sourceTerminal.State = state;
        }

        public override void RegisterEventHandlersForTerminal(ITerminal terminal)//Регистрация Обработчиков Событий Для Терминала
        {
            terminal.OutgoingCall += InnerConnectionHandler;//Обработчик Внутренних Соединений
            terminal.Responce += ResponseConnectionHandler;//Обработчик Ответных Соединений
        }

        public override void RegisterEventHandlersForPort(IPort port)////Регистрация Обработчиков Событий Для Порта
        {
            port.StateChanged +=
                (sender, state) => { Console.WriteLine("Станция обнаружила, что порт изменил свое состояние на {0}", state); };//Station detected the port changed its State to 
            port.StateChanging += (sender, state) =>
            {
                var a = PortsMapping.FirstOrDefault(x => x.Value == sender).Key;
                var d = GetTerminalByPhoneNumber(a);//Получить Терминал По Номеру Телефона

                if (_activeCallMapping.ContainsKey(d) || _activeCallMapping.Values.Contains(d))
                {
                    d.Drop();
                }
                else
                {
                    d.Reject();
                }
            };
        }

        protected override void ResponseConnectionHandler(object sender, Response responce)//Обработчик Ответных Соединений
        {
            if (responce.State == ResponseState.Drop || responce.State == ResponseState.Reject)
            {
                var callinfo = GetCallInfo(responce.Source);
                SetTerminalsStateTo(callinfo.Source, callinfo.Target, TerminalState.Free);
            }
            base.ResponseConnectionHandler(sender, responce);
        }

        private void SetTerminalsStateTo(PhoneNumber source, PhoneNumber target, TerminalState state)
        {
            var sourceTerminal = GetTerminalByPhoneNumber(source) as TestTerminal;//Получить Терминал По Номеру Телефона
            var targetTerminal = GetTerminalByPhoneNumber(target) as TestTerminal;
            if (sourceTerminal != null) sourceTerminal.State = state;
            if (targetTerminal != null) targetTerminal.State = state;
        }
    }
}