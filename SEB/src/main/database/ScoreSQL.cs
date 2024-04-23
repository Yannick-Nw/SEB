using System.Text.Json;
using Npgsql;
using SEB.httpServer.response;
using SEB.models;

namespace SEB.main.database;

public class ScoreSQL
{
    public void GetAllUserStats(StreamWriter writer, List<UserStats> userStatsList, string connectionString)
    {
        string queryString =
            "SELECT Users.user_id, userELO, SUM(Count) as TotalPushups FROM Users JOIN PushUpRecords ON Users.user_id = PushUpRecords.UserID GROUP BY Users.user_id, username, userELO ORDER BY userELO DESC, TotalPushups DESC;";
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            using (var cmd = new NpgsqlCommand(queryString, connection))
            {
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserStats userStat = new UserStats
                            {
                                UserName = reader.GetInt32(reader.GetOrdinal("user_id")),
                                UserElo = reader.GetInt32(reader.GetOrdinal("userELO")),
                                TotalPushups = reader.GetInt32(reader.GetOrdinal("TotalPushups"))
                            };
                            string jsonString = JsonSerializer.Serialize(userStat);
                            ResponseHandler rs = new ResponseHandler();
                            rs.SendJsonResponse(writer, jsonString, 200);
                            userStatsList.Add(userStat);
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with Database in GetAllUserStats: {ex.Message}");
                    throw;
                }
            }
        }
    }
}