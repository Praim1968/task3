using System;
using System.Collections.Generic;
using System.Linq;
using ATS.BillingSystemModel.Intarfaces;
using ATS.BillingSystemModel.TarifPlans;
using ATS.Station_Model.Intarfaces;
using ATS.Station_Model.States;
using ATS.Test;
using ATS.User_Model;

namespace ATS
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var ats = new Ats(new List<IPort>(), new List<ITerminal>());
            var velcome = new Velcome(new List<ITariffPlan>()
            {
                new Talk(),
           
            });

            var panel = new ControlPanel(ats, velcome);


            var user1 = new User();
            var user2 = new User();


            user1.Phone = velcome.GetContract(user1, velcome.TariffPlans.ElementAt(0));
            user2.Phone = velcome.GetContract(user2, velcome.TariffPlans.ElementAt(0));

            user1.Plug();
            user2.Plug();

            user1.Call(user2.Phone.Number);
            user2.Answer();
            user2.Drop();
            user2.Call(user1.Phone.Number);
            user1.Answer();
            user1.UnPlug();
            user1.Drop();
            user2.Call(user1.Phone.Number);
            user1.Call(user2.Phone.Number);

            velcome.PayForPhoneNubmer(user1.Phone.Number);

            var d = velcome.GetStatistic(x => x.TimeBegin < DateTime.Now.AddMonths(2), user2);
            Console.WriteLine("===============================================================");

            foreach (var v in d.ToList())
            {
                Console.WriteLine(v.State == TerminalState.IncomingCall
                    ? $"Abonent - {v.Target.Number} recive call from - {v.Source.Number};\nDuration : {v.Duration.TotalMinutes} (minutes);\nCost : {v.Cost};\nDate : {v.TimeBegin};"
                    : $"Abonent - {v.Source.Number} call to - {v.Target.Number};\nDuration : {v.Duration.TotalMinutes} (minutes);\nCost : {v.Cost};\nDate : {v.TimeBegin};");
                Console.WriteLine("===============================================================");
            }
            Console.ReadLine();
        }
    }
    
}