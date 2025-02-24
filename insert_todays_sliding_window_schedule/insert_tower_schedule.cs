using BRG.Helen.Backend.Core;
using BRG.Helen.Cloud.Logging;
using Dapper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
namespace insert_todays_sliding_window_schedule;

public class InsertTowerSchedule
{
    [Function("insert_tower_schedule")]
    public async Task<object> Run([TimerTrigger("0 0 9 * * *")] TimerInfo myTimer)
    {

        string connectionString = Environment.GetEnvironmentVariable("connectionstring");
        string slack_app_id     = Environment.GetEnvironmentVariable("slack_app_id");
        string slack_channel_id = Environment.GetEnvironmentVariable("slack_channel_id");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return Utils.Error("connection string is not valid");
        }

        try
        {
            SlackLogger.SendMessage(slack_app_id, slack_channel_id, "insert_tower_schedule is executing.");
            using var connection = new SqlConnection(connectionString);
            //connection.Execute("linens.spInsertTodaysTowerSlidingWindowSchedule");
            SlackLogger.SendMessage(slack_app_id, slack_channel_id, "insert_tower_schedule has executed successfully.");
            return Utils.OK();
        }
        catch (Exception ex)
        {
            return Utils.Error(ex);
        }
    }
}
