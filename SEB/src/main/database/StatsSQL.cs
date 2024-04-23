using System.Text.Json;
using Npgsql;
using SEB.httpServer.response;
using SEB.models;

namespace SEB.main.database;

public class StatsSQL
{
    public void GetUserStats(StreamWriter writer, UserStats userStats, int userID, string connectionString)
    {
        string queryString =
            "SELECT Users.user_id, userELO, SUM(Count) as TotalPushups FROM Users JOIN PushUpRecords ON Users.user_id = PushUpRecords.UserID WHERE Users.user_id = @userID GROUP BY Users.user_id";
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            using (var cmd = new NpgsqlCommand(queryString, connection))
            {
                // Add parameters to the query
                cmd.Parameters.AddWithValue("userID", userID);
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userStats.UserName = reader.GetInt32(reader.GetOrdinal("user_id"));
                            userStats.UserElo = reader.GetInt32(reader.GetOrdinal("userELO"));
                            userStats.TotalPushups = reader.GetInt32(reader.GetOrdinal("TotalPushups"));
                            string jsonString = JsonSerializer.Serialize(userStats);
                            ResponseHandler rs = new ResponseHandler();
                            rs.SendJsonResponse(writer, jsonString, 200);
                        }
                        else
                        {
                            Console.WriteLine("Userstats not found");
                            ResponseHandler rs = new ResponseHandler();
                            rs.SendErrorResponse(writer, "Userstats not found", 404);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with Database in GetUserStats: {ex.Message}");
                    throw;
                }
            }
        }
    }

    public void UpdateUserStats(int winnerUserName, int tournamentID, string connectionString)
    {
        string updateWinnerQuery = "UPDATE Users SET userELO = userELO + 2 WHERE user_id = @winnerUserID";
        string updateLosersQuery =
            "UPDATE Users SET userELO = userELO - 1 WHERE user_id IN (SELECT UserID FROM TournamentEntries WHERE TournamentID = @tournamentID AND tournamententries.userid != @winnerUserID)";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            using (var cmd = new NpgsqlCommand(updateWinnerQuery, connection))
            {
                // Add parameters to the query
                cmd.Parameters.AddWithValue("winnerUserID", winnerUserName);
                cmd.Parameters.AddWithValue("tournamentID", tournamentID);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with Database in UpdateUserStats (Winner): {ex.Message}");
                    throw;
                }
            }
            
            using (var cmd = new NpgsqlCommand(updateLosersQuery, connection))
            {
                // Add parameters to the query
                cmd.Parameters.AddWithValue("tournamentID", tournamentID);
                cmd.Parameters.AddWithValue("winnerUserID", winnerUserName);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with Database in UpdateUserStats (Losers): {ex.Message}");
                    throw;
                }
            }
        }
    }

}