using Microsoft.Azure.Functions.Worker;

namespace insert_todays_sliding_window_schedule;

public class StartOfDayCleanup
{
    [Function("StartOfDayCleanup")]
    public object Run([TimerTrigger("0 0 9 * * *")] TimerInfo myTimer)
    {
        return StartLinensDay.Start();
    }
}
