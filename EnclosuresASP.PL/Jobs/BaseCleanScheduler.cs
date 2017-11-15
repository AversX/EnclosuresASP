using Quartz;
using Quartz.Impl;

namespace EnclosuresASP.PL.Jobs
{
    public class BaseCleanScheduler
    {

        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<BaseCleaner>().Build();

            ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
                .StartNow()                            // запуск сразу после начала выполнения
                .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                    .WithIntervalInMinutes(30)          // через 30 минут
                    .RepeatForever())                   // бесконечное повторение
                .Build();                               // создаем триггер

            scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
        }
    }
}