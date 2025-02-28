using BRG.Helen.Backend.Core;
using BRG.Helen.Cloud.Logging;
using Dapper;
using Firebase.TeamHelenSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace insert_todays_sliding_window_schedule;

internal static class StartLinensDay
{
    static readonly string connectionString = Environment.GetEnvironmentVariable("connectionstring");
    static readonly string slackAppId = Environment.GetEnvironmentVariable("slack_app_Id");
    static readonly string slackChannelId = Environment.GetEnvironmentVariable("slack_app_channel_id");
    internal static OkObjectResult Start()
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return Utils.Error("Connection string is not valid.");
        }

        try
        {
            using var connection = new SqlConnection(connectionString);

            #region Step 1: Call the proc to Start the Day
            StartOfDayCleanupProc(connection);
            #endregion

            #region Step 2: Call the service to Refresh the Kanban
            RefreshKanban();
            #endregion

            #region Step 3: Call the proc to Insert Today's Soiled Schedule
            InsertSoiledScheduleProc(connection);
            #endregion

            #region Step 4: Call the proc to Insert Today's Clean Schedule
            InsertCleanScheduleProc(connection);
            #endregion

            return Utils.OK();
        }
        catch (Exception ex)
        {
            SlackLogger.SendMessage(slackAppId, slackChannelId, $"Run: {ex.Message}");
            return Utils.Error(ex);
        }
    }

    private static void InsertSoiledScheduleProc(SqlConnection connection)
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

    private static void InsertCleanScheduleProc(SqlConnection connection)
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

    private static void StartOfDayCleanupProc(SqlConnection connection)
    {
        try
        {
            connection.Execute("linens.spStartDayCleanup");
        }
        catch (Exception ex)
        {
            SlackLogger.SendMessage(slackAppId, slackChannelId, $"Error in insert_Clean_schedule: {ex.Message}");
            throw;
        }
    }

    private static void RefreshKanban()
    {
        try
        {
            TeamHelenSettingsBuilder.CreateInstance().SetRefreshKanbanBoard(true).Save();
        }
        catch (Exception ex)
        {
            SlackLogger.SendMessage(slackAppId, slackChannelId, $"Error in RefreshKanbanFunction: {ex.Message}");
            throw;
        }
    }
}
