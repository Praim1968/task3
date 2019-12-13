using System;
using System.Collections.Generic;
using System.Linq;
using ATS.BillingSystemModel.Intarfaces;
using ATS.Helpers;
using ATS.Station_Model.Intarfaces;
using ATS.Station_Model.States;
using ATS.Test;
using ATS.User_Model;

namespace ATS.BillingSystemModel.AbstractClass
{
    public abstract class BillingSystem : IBillingSystem
    {
        private readonly IDictionary<ITerminal, IUser> _terminalsUserMapp;
        private readonly IDictionary<IUser, ITariffPlan> _userTariffPlansMapp;
        private int _id;
        private readonly IDictionary<IUser, DateTime> _userRegistryDateTimeMapp;//??

        protected BillingSystem(ICollection<ITariffPlan> tariffPlans)
        {
            TariffPlans = tariffPlans;//тарфный план
            _terminalsUserMapp = new Dictionary<ITerminal, IUser>();
            UserCallinfoDictionary = new Dictionary<IUser, ICollection<CallInfo>>();//Словарь информации о вызове пользователя
            _userTariffPlansMapp = new Dictionary<IUser, ITariffPlan>();
            _userRegistryDateTimeMapp = new Dictionary<IUser, DateTime>();
            UserPayDateTime = new Dictionary<IUser, DateTime>();//Пользователь Платит за Время
        }

        protected IDictionary<IUser, ICollection<CallInfo>> UserCallinfoDictionary { get; }

        public IDictionary<IUser, DateTime> UserPayDateTime { get; set; }


        public ICollection<ITariffPlan> TariffPlans { get; }


        public ITerminal GetContract(IUser user, ITariffPlan tariffPlan)//создаем контракт 
        {
            var terminal = new TestTerminal(new PhoneNumber((10000 + _id).ToString()), tariffPlan);
            _id++;

            _userTariffPlansMapp.Add(user, tariffPlan);
            UserCallinfoDictionary.Add(user, new List<CallInfo>());
            _terminalsUserMapp.Add(terminal, user);
            var date = TimeHelper.Now;
            _userRegistryDateTimeMapp.Add(user, date);
            UserPayDateTime.Add(user, date);
            OnToSignСontract(terminal);// Подписание договора с терминалом
            return terminal;
        }

        public void CallInfoHandler(object sender, CallInfo callInfo)//Обработчик Информации О Вызове
        {
            var sourcePair = GetUserTerminalMapPair(callInfo.Source);
            var targetPair = GetUserTerminalMapPair(callInfo.Target);
            var targetCallInfo = new CallInfo(callInfo.Target, callInfo.Source, TerminalState.IncomingCall)
            {
                TimeBegin = callInfo.TimeBegin,
                Duration = callInfo.Duration,
                Cost = 0
            };

            callInfo.Cost = sourcePair.Key.TariffPlan.CalculateCallCost(callInfo.Duration);//Рассчитать Стоимость Звонка

            UserCallinfoDictionary[sourcePair.Value].Add(callInfo);
            UserCallinfoDictionary[targetPair.Value].Add(targetCallInfo);
        }

        public event EventHandler<ITerminal> ToSignСontract;
        public event EventHandler<IUser> Pay;

        public void PayForPhoneNubmer(PhoneNumber phoneNumber)//оплата
        {
            OnPay(_terminalsUserMapp.FirstOrDefault(x => x.Key.Number == phoneNumber).Value);
        }


        public void SetNewTariffPlan(IUser user, ITariffPlan tariffPlan)//?
        {
            var contractDate = GetContracDateTime(user);
            if (contractDate > TimeHelper.Now)
            {
                _userTariffPlansMapp.Remove(user);
                _userTariffPlansMapp.Add(user, tariffPlan);
                _userRegistryDateTimeMapp.Remove(user);
                _userRegistryDateTimeMapp.Add(user, TimeHelper.Now);
            }
            else
            {
                Console.WriteLine("Month has not yet passed");//месяц еще не окончился
            }
        }

        private DateTime GetContracDateTime(IUser user)
        {
            return _userRegistryDateTimeMapp.FirstOrDefault(x => x.Key == user).Value;
        }

        private KeyValuePair<ITerminal, IUser> GetUserTerminalMapPair(PhoneNumber number)
        {
            return _terminalsUserMapp.FirstOrDefault(x => x.Key.Number == number);
        }

        protected ITariffPlan GeTariffPlan(IUser user)
        {
            return _userTariffPlansMapp.FirstOrDefault(x => x.Key == user).Value;
        }

        public void AddNewTariffPlan(ITariffPlan tariffPlan)
        {
            TariffPlans.Add(tariffPlan);
        }

        protected virtual void OnPay(IUser user)
        {
            Pay?.Invoke(this, user);
        }


        protected virtual void OnToSignСontract(ITerminal terminal)//подпиание договора
        {
            ToSignСontract?.Invoke(this, terminal);
        }
    }
}