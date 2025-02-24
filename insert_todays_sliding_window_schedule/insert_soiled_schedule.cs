using System;
using Microsoft.Data.SqlClient;
using Microsoft.Azure.Functions.Worker;
using BRG.Helen.Backend.Core;

namespace insert_todays_sliding_window_schedule;

public class InsertSoiledSchedule
{
    [Function("insert_soiled_schedule")]
    public object Run([TimerTrigger("0 9 * * *")] TimerInfo myTimer)
    {

        string connectionString = Environment.GetEnvironmentVariable("connectionstring");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return Utils.Error("connection string is not valid");
        }

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("linens.spInsertTodaysSoiledSlidingWindowSchedule", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
            }

            return Utils.OK();
        }
        catch (Exception ex)
        {
            return Utils.Error(ex);
        }
    }
}
