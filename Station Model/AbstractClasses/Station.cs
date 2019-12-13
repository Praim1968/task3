using System;
using System.Collections.Generic;
using System.Linq;
using ATS.Helpers;
using ATS.Station_Model.Intarfaces;
using ATS.Station_Model.States;

namespace ATS.Station_Model.AbstractClasses
{
    public abstract class Station : IStation
    {
        protected readonly IDictionary<ITerminal, ITerminal> _activeCallMapping;//активное сопоставление вызовов
        protected readonly ICollection<CallInfo> _callInfoCollection;
        private readonly ICollection<IPort> _portCollection;
        protected readonly ICollection<ITerminal> _terminalCollection;
        protected readonly ICollection<ITerminal> _waitActionTerminals;//терминалы действия ожидания

        protected Station(ICollection<IPort> ports, ICollection<ITerminal> terminals)
        {
            _portCollection = ports;
            _terminalCollection = terminals;
            PortsMapping = new Dictionary<PhoneNumber, IPort>();
            _callInfoCollection = new List<CallInfo>();
            _waitActionTerminals = new List<ITerminal>();
            _activeCallMapping = new Dictionary<ITerminal, ITerminal>();
        }

        public IDictionary<PhoneNumber, IPort> PortsMapping { get; }//унивесальная коллекция пар(номеров телефонов)

        public abstract void RegisterEventHandlersForTerminal(ITerminal terminal);//Регистрация Обработчиков Событий Для Термнала

        public abstract void RegisterEventHandlersForPort(IPort port);//Регистрация Обработчиков Событий Для Порта

        public event EventHandler<CallInfo> CallInfoPrepared;//Подготовка информации о звонке 

        public void ClearEvents()
        {
            CallInfoPrepared = null;
        }

        protected virtual void InnerConnectionHandler(object sender, CallInfo callInfo)//оброботчк внутрених соеднений
        {
            var targetPort = GetPortByPhoneNumber(callInfo.Target);// получения порта по номеру телефона 
            if (targetPort.State == PortState.Unpluged || targetPort.State == PortState.Call ||
                callInfo.Source == callInfo.Target)
            {
                callInfo.TimeBegin = TimeHelper.Now;
                callInfo.Duration = TimeSpan.Zero;
                OnCallInfoPrepared(this, callInfo);//информация о вызове 
            }
            else
            {
                SetPortsStateTo(callInfo.Source, callInfo.Target, PortState.Call);//Установите Состояние Портов
                var targetTerminal = _terminalCollection.FirstOrDefault(x => x.Number == callInfo.Target);//возврощаем первый эллемент последовательности

                targetTerminal?.GetReqest(callInfo.Source);
                _callInfoCollection.Add(callInfo);
                _waitActionTerminals.Add(targetTerminal);
            }
        }


        protected virtual void ResponseConnectionHandler(object sender, Response responce)//Обработчик Ответных Соединений
        {
            var callInfo = GetCallInfo(responce.Source);

            switch (responce.State)
            {
                case ResponseState.Accept:
                    MakeCallActive(callInfo);//Сделать Вызов Активным
                    break;
                case ResponseState.Drop:
                    InterruptActiveCall(callInfo);//Прервать Активный Вызов
                    OnCallInfoPrepared(this, callInfo);// Информация По Вызову
                    break;
                case ResponseState.Reject:
                    InterruptCall(callInfo);//Прерывание Вызова
                    OnCallInfoPrepared(this, callInfo);// Информация По Вызову
                    break;
                default:
                    Console.WriteLine($"Недопустимый ответ от {responce.Source} статус = {responce.State} ");
                    break;
            }
        }

        private void MakeCallActive(CallInfo info)//Сделать Вызов Активным
        {
            var sourceTerminal = GetTerminalByPhoneNumber(info.Source);//Получить Терминал По Номеру Телефона
            var targetTerminal = GetTerminalByPhoneNumber(info.Target);

            _waitActionTerminals.Remove(sourceTerminal);

            _activeCallMapping.Add(sourceTerminal, targetTerminal);

            info.TimeBegin = TimeHelper.Now;
        }

        private void InterruptCall(CallInfo info)//Прерывание Вызова
        {
            _callInfoCollection.Remove(info);
            SetPortsStateTo(info.Source, info.Target, PortState.Free);//Установите Состояние Портов 
            info.TimeBegin = TimeHelper.Now;
        }

        private void InterruptActiveCall(CallInfo info)//Прервать Активный Вызов
        {
            var sourceTerminal = GetTerminalByPhoneNumber(info.Source);//Получить Терминал По Номеру Телефона
            var targetTerminal = GetTerminalByPhoneNumber(info.Target);

            info.Duration = TimeHelper.Duration();//продолжительность

            _waitActionTerminals.Remove(targetTerminal);

            _activeCallMapping.Remove(new KeyValuePair<ITerminal, ITerminal>(sourceTerminal, targetTerminal));

            _callInfoCollection.Remove(info);

            SetPortsStateTo(info.Source, info.Target, PortState.Free);
        }

        protected void SetPortsStateTo(PhoneNumber source, PhoneNumber target, PortState state)//Установите Состояние Портов
        {
            var targetPort = GetPortByPhoneNumber(target);
            var sourcePort = GetPortByPhoneNumber(source);

            if (targetPort != null)
            {
                targetPort.State = state;
            }
            if (sourcePort != null)
            {
                sourcePort.State = state;
            }
        }

        protected ITerminal GetTerminalByPhoneNumber(PhoneNumber number)//Получить Терминал По Номеру Телефона
        {
            return _terminalCollection.FirstOrDefault(x => x.Number == number);
        }

        protected virtual CallInfo GetCallInfo(PhoneNumber target)//получть информацию звонка
        {
            return _callInfoCollection.FirstOrDefault(x => x.Target == target);
        }

        protected virtual IPort GetPortByPhoneNumber(PhoneNumber number)//Получить Порт По Номеру Телефона
        {
            return PortsMapping[number];
        }

        public void AddPort(IPort port)
        {
            _portCollection.Add(port);
        }

        public bool Add(ITerminal terminal)
        {
            var freePort = _portCollection.Except(PortsMapping.Values).FirstOrDefault();
            if (freePort == null) return false;
            _terminalCollection.Add(terminal);

            MapTerminalToPort(terminal, freePort);//Карта Терминала В Порт

            RegisterEventHandlersForTerminal(terminal);//Регистрация Обработчиков Событий Для Терминала
            RegisterEventHandlersForPort(freePort);//Регистрация Обработчиков Событий Для Портов

            return true;
        }

        private void MapTerminalToPort(ITerminal terminal, IPort port)//Карта Терминала В Порт
        {
            PortsMapping.Add(terminal.Number, port);
            port.RegisterEventHandlersForTerminal(terminal);
            terminal.RegisterEventHandlersForPort(port);
        }

        private void UnMapTerminalFromPort(ITerminal terminal, IPort port)//Карта Терминала Из Порта
        {
            PortsMapping.Remove(terminal.Number);
        }

        protected virtual void OnCallInfoPrepared(object sender, CallInfo callInfo)//По Вызову Информация Подготовлена
        {
            CallInfoPrepared?.Invoke(this, callInfo);
        }
    }
}