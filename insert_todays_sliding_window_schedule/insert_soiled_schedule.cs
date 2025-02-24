using System.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Dapper;
using BRG.Helen.Backend.Core;
using BRG.Helen.Cloud.Logging;

namespace insert_todays_sliding_window_schedule;

public class InsertSoiledSchedule
{
    [Function("insert_soiled_schedule")]
    public async Task<object> Run([TimerTrigger("0 0 9 * * *")] TimerInfo myTimer)
    {
        string connectionString = Environment.GetEnvironmentVariable("connectionstring");
        string slack_app_id     = Environment.GetEnvironmentVariable("appId");
        string slack_channel_id = Environment.GetEnvironmentVariable("channelId");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return Utils.Error("connection string is not valid");
        }

        try
        {
            SlackLogger.SendMessage(slack_app_id, slack_channel_id, "insert_Soiled_schedule is executing.");
            using var connection = new SqlConnection(connectionString);
            //connection.Execute("linens.spInsertTodaysSoiledSlidingWindowSchedule");
            SlackLogger.SendMessage(slack_app_id, slack_channel_id, "insert_Soiled_schedule has executed successfully.");
            return Utils.OK();
        }
        catch (Exception ex)
        {
            return Utils.Error(ex);
        }
    }
}
