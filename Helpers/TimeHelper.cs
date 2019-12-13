using System;

namespace ATS.Helpers
{
    public static class TimeHelper
    {
        private static int _counter;

        public static EventHandler<DateTime> NewDateTime;

        public static DateTime Now
        {
            get
            {

                _counter++;
                var date = DateTime.Now.Add(new TimeSpan(10*_counter, 0, 10*_counter, 5*_counter));
                OnNewDateTime(null, date);
                return date;
            }
        }

        private static void OnNewDateTime(object sender, DateTime time)
        {
            NewDateTime?.Invoke(null, time);
        }

        public static TimeSpan Duration()
        {
            return new TimeSpan(0, new Random().Next(0, 59), new Random().Next(0, 59));
        }
    }
}