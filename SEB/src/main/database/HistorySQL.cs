using System.Text.Json;
using Npgsql;
using SEB.httpServer.response;
using SEB.models;

namespace SEB.main.database;

public class HistorySQL
{
    public void newPushUpRecord(History data, int tournamentID, string connString)
    {
        using (var conn = new NpgsqlConnection(connString))
        {
            string queryString =
                "INSERT INTO PushUpRecords (UserID, Count, Duration) VALUES (@userID, @count, @duration) RETURNING RecordID";
            //conn.Open();
            using (var command = new NpgsqlCommand(queryString, conn))
            {
                command.Parameters.AddWithValue("userID", data.UserId);
                command.Parameters.AddWithValue("count", data.Count);
                command.Parameters.AddWithValue("duration", data.DurationInSeconds);
                try
                {
                    conn.Open();
                    int recordID = (int)command.ExecuteScalar();
                    InsertIntoTournamentEntries(tournamentID, recordID, data.UserId, connString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in newPushUpRecord: {ex.Message}");
                    //throw;
                }
            }
        }
    }

    public void InsertIntoTournamentEntries(int tournamentID, int recordID, int userID, string connString)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                //conn.Open();
                string queryString =
                    "INSERT INTO TournamentEntries (TournamentID, UserID, RecordID) VALUES (@tournamentID, @userID, @recordID)";

                using (var command = new NpgsqlCommand(queryString, connection))
                {
                    //currval('PushUpRecords_RecordID_seq')
                    command.Parameters.AddWithValue("tournamentID", tournamentID);
                    command.Parameters.AddWithValue("userID", userID);
                    command.Parameters.AddWithValue("recordID", recordID);
                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} row(s) inserted successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in CreateUser: {ex.Message}");
                        //throw;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public void GetUserHistory(StreamWriter writer, List<History> userHistoryList, string connectionString, string userToken)
    {
        string queryString =
            "SELECT Count, Duration FROM PushUpRecords JOIN Users ON PushUpRecords.UserID = Users.user_id WHERE Users.userToken = @userToken ORDER BY PushUpRecords.Timestamp DESC;";
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            using (var cmd = new NpgsqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("@userToken", userToken);
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            History userHistory = new History
                            {
                                //UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                                Count = reader.GetInt32(reader.GetOrdinal("Count")),
                                DurationInSeconds = reader.GetInt32(reader.GetOrdinal("Duration"))
                            };
                            string jsonString = JsonSerializer.Serialize(userHistory);
                            ResponseHandler rs = new ResponseHandler();
                            rs.SendJsonResponse(writer, jsonString, 200);
                            userHistoryList.Add(userHistory);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with Database in GetUserStatsByToken: {ex.Message}");
                    throw;
                }
            }
        }
    }
}