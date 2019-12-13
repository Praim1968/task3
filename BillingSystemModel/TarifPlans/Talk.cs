using System;
using ATS.BillingSystemModel.Intarfaces;

namespace ATS.BillingSystemModel.TarifPlans
{
    public class Talk : ITariffPlan
    {
        public static double CostOneMinute => 25.1;//стомость одной минуты

        public double FreeMinutes { get; private set; }

        public double CalculateCallCost(TimeSpan duration)//Рассчитать Стоимость Звонка
        {
            if (FreeMinutes == 0) return CostOneMinute*Math.Abs(duration.TotalMinutes);
            if (FreeMinutes - Math.Abs(duration.TotalMinutes) < 0)
            {
                FreeMinutes = 0;

                return CostOneMinute*(Math.Abs(duration.TotalMinutes) - FreeMinutes);
            }

            FreeMinutes -= Math.Abs(duration.TotalMinutes);

            return 0;
        }
    }
}