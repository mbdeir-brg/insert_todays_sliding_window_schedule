using System.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Dapper;
using BRG.Helen.Backend.Core;
using BRG.Helen.Cloud.Logging;

namespace insert_todays_sliding_window_schedule;
// 1- Call the proc to Start the Day
// 2- Call the service to Refresh the Kanban
// 3- Call the proc to Insert Today's Soiled Schedule
// 4- Call the proc to Insert Today's Clean Schedule
public class StartOfDayCleanup
{
    [Function("StartOfDayCleanup")]
    public object Run([TimerTrigger("0 0 9 * * *")] TimerInfo myTimer)
    {
        string connectionString = Environment.GetEnvironmentVariable("connectionstring");
        string slackAppId = Environment.GetEnvironmentVariable("appId");
        string slackChannelId = Environment.GetEnvironmentVariable("channelId");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return Utils.Error("Connection string is not valid.");
        }

        try
        {
            using var connection = new SqlConnection(connectionString);

            #region Step 1: Call the Start of Day Cleanup Procedure
            StartOfDayCleanupFunction(connection);
            #endregion

            #region Step 2: Call the Service to Refresh the Kanban
            RefreshKanbanFunction();
            #endregion

            #region Step 3: Insert Today's Soiled Schedule
            InsertSoiledScheduleFunction(connection, slackAppId, slackChannelId);
            #endregion

            #region Step 4: Insert Today's Clean Schedule
            InsertCleanScheduleFunction(connection, slackAppId, slackChannelId);
            #endregion

            return Utils.OK();
        }
        catch (Exception ex)
        {
            return Utils.Error(ex);
        }
    }

    private void InsertSoiledScheduleFunction(SqlConnection connection, string slackAppId, string slackChannelId)
    {
        try
        {
            SlackLogger.SendMessage(slackAppId, slackChannelId, "insert_Soiled_schedule is executing.");
            connection.Execute("linens.spInsertTodaysSoiledSlidingWindowSchedule");
            SlackLogger.SendMessage(slackAppId, slackChannelId, "insert_Soiled_schedule has executed successfully.");
        }
        catch (Exception ex)
        {
            SlackLogger.SendMessage(slackAppId, slackChannelId, $"Error in insert_Soiled_schedule: {ex.Message}");
            throw;
        }
    }

    private void InsertCleanScheduleFunction(SqlConnection connection, string slackAppId, string slackChannelId)
    {
        try
        {
            SlackLogger.SendMessage(slackAppId, slackChannelId, "insert_Clean_schedule is executing.");
            connection.Execute("linens.spInsertTodaysTowerSlidingWindowSchedule");
            SlackLogger.SendMessage(slackAppId, slackChannelId, "insert_Clean_schedule has executed successfully.");
        }
        catch (Exception ex)
        {
            SlackLogger.SendMessage(slackAppId, slackChannelId, $"Error in insert_Clean_schedule: {ex.Message}");
            throw;
        }
    }

    private void StartOfDayCleanupFunction(SqlConnection connection)
    {
        try
        {
            connection.Execute("linens.spStartDayCleanup");
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private void RefreshKanbanFunction()
    {
        try
        {
            // i just need the logic to refresh the kanban

        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
