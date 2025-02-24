using System;
using Microsoft.Data.SqlClient;
using Microsoft.Azure.Functions.Worker;
using BRG.Helen.Backend.Core;
using Dapper;
using System.Data;
using BRG.Helen.Cloud.Logging;
namespace insert_todays_sliding_window_schedule;

public class InsertTowerSchedule
{
    [Function("insert_tower_schedule")]
    public async Task<object> Run([TimerTrigger("0 0 9 * * *")] TimerInfo myTimer)
    {

        string connectionString = Environment.GetEnvironmentVariable("connectionstring");
        string appId = Environment.GetEnvironmentVariable("appId");
        string channelId = Environment.GetEnvironmentVariable("channelId");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return Utils.Error("connection string is not valid");
        }

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

             connection.Execute("linens.spInsertTodaysTowerSlidingWindowSchedule",commandType: CommandType.StoredProcedure);
            SlackLogger.SendMessage(appId, channelId, "insert_tower_schedule has been executed successfully");
            return Utils.OK();
        }
        catch (Exception ex)
        {
            return Utils.Error(ex);
        }
    }
}
