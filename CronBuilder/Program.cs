using System;

namespace CronBuilder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cronBuilder = new CronBuilder()
                                .Daily()
                                .RepeatEvery(3)
                                .OccurringAt(14, 30);

            Console.WriteLine(string.Format("Cron Expression: {0}", cronBuilder.GetCronExpression()));

            var startDateTime = new DateTime(2015, 06, 01);
            var endDateTime = new DateTime(2015, 06, 30);

            var occurances = cronBuilder.OccurancesBetween(startDateTime, endDateTime);

            foreach (var occurance in occurances)
            {
                Console.WriteLine(occurance);
            }
           
            Console.ReadKey();
        }
    }
}
