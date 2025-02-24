using System;
using System.Data;
using System.Threading.Tasks;
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

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return Utils.Error("connection string is not valid");
        }

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

             connection.Execute("linens.spInsertTodaysSoiledSlidingWindowSchedule",
                commandType: CommandType.StoredProcedure);
            SlackLogger.SendMessage("T06CDEQREBH", "B08FDSJ1U6L/xK99PnW50fDcsA0oMdqtIu3k", "insert_tower_schedule has been executed successfully");

            return Utils.OK();
        }
        catch (Exception ex)
        {
            return Utils.Error(ex);
        }
    }
}
