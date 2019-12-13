using System;
using System.Collections.Generic;
using ATS.Station_Model.Intarfaces;
using ATS.User_Model;

namespace ATS.BillingSystemModel.Intarfaces
{
    public interface IBillingSystem
    {
        ICollection<ITariffPlan> TariffPlans { get; }//тарфный план
        ITerminal GetContract(IUser user, ITariffPlan tariffPlan);//Получение контракта 
        void CallInfoHandler(object sender, CallInfo callInfo);//Обработчик Информации О Вызове
        event EventHandler<IUser> Pay;//платть 
        event EventHandler<ITerminal> ToSignСontract;//Подписать Договор
        void PayForPhoneNubmer(PhoneNumber phone);//Платить За Телефонный Номер
    }
}