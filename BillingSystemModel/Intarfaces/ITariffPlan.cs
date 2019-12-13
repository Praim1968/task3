using System;

namespace ATS.BillingSystemModel.Intarfaces
{
    public interface ITariffPlan
    {
        double CalculateCallCost(TimeSpan duration);//Рассчитать Стоимость Звонка
    }
}