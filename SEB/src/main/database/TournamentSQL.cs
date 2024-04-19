using Npgsql;
using SEB.models;

namespace SEB.main.database;

public class TournamentSQL
{
    public int CreateTournament(string connectionString)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string queryString =
                "SELECT TournamentID FROM Tournaments WHERE CURRENT_TIMESTAMP BETWEEN StartTime AND EndTime";
            // Check if a tournament is currently running
            using (NpgsqlCommand command = new NpgsqlCommand(queryString, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int tournamentId = reader.GetInt32(0);
                        Console.WriteLine($"A tournament with ID: {tournamentId} is currently running.");
                        return tournamentId;
                    }
                    else
                    {
                        Console.WriteLine("No tournament is currently running.");
                    }
                }
            }

            // If no tournament is running, create a new one
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddMinutes(2);

            queryString =
                "INSERT INTO Tournaments (StartTime, EndTime) VALUES (@StartTime, @EndTime) RETURNING tournamentid";
            using (NpgsqlCommand command = new NpgsqlCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("@StartTime", startTime);
                command.Parameters.AddWithValue("@EndTime", endTime);
                try
                {
                    //connection.Open();
                    int tournamentid = (int)command.ExecuteScalar();
                    return tournamentid;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in newPushUpRecord: {ex.Message}");
                    //throw;
                }
                /*
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Tournament created successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to create tournament.");
                }
                */

                //connection.Close();
            }
        }

        return -1;
    }

    public void GetCurrentTournamentState(Tournament tournamentState, string connectionString, int type)
    {
        string queryString =
            @"SELECT 
                Tournaments.TournamentID,
                    Tournaments.StartTime, 
                    COUNT(TournamentEntries.EntryID) as Participants, 
                    Users.username as Leader,
                    SUM(PushUpRecords.Count) as TotalPushUps
                FROM 
                    Tournaments 
                JOIN 
                    TournamentEntries ON Tournaments.TournamentID = TournamentEntries.TournamentID 
                JOIN 
                    Users ON TournamentEntries.UserID = Users.user_id 
                JOIN
                    PushUpRecords ON TournamentEntries.RecordID = PushUpRecords.RecordID
                WHERE 
                    Tournaments.StartTime = (
                        SELECT MAX(StartTime) 
                        FROM Tournaments 
                    )
                GROUP BY 
                Tournaments.TournamentID,
                    Tournaments.StartTime, 
                    Users.username 
                ORDER BY 
                    SUM(PushUpRecords.Count) DESC 
                LIMIT 1;";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            using (var cmd = new NpgsqlCommand(queryString, connection))
            {
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tournamentState.StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime"));
                            if ((DateTime.Now - tournamentState.StartTime).TotalMinutes > 2 && type == 1)
                            {
                                Console.WriteLine("The start time was more than 2 minutes ago.");
                                return;
                            }

                            tournamentState.Participants = reader.GetInt32(reader.GetOrdinal("Participants"));
                            tournamentState.Leader = reader.GetString(reader.GetOrdinal("Leader"));
                            tournamentState.TotalPushUps = reader.GetInt32(reader.GetOrdinal("TotalPushUps"));
                            tournamentState.TournamentId = reader.GetInt32(reader.GetOrdinal("TournamentID"));
                        }
                        else
                        {
                            Console.WriteLine("Tournament state not found");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with Database in GetCurrentTournamentState: {ex.Message}");
                    throw;
                }
            }
        }
    }
}