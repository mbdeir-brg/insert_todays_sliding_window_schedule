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
        // Get current time in UTC
        DateTime utcNow = DateTime.UtcNow;

        // Ensure function runs only at 9:00 AM UTC (Prevent execution outside this window)
        if (utcNow.Hour != 9 || utcNow.Minute != 0)
        {
            return Utils.Error("The current time should be 4 am EST for it to run"); // Exit function without executing
        }

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
