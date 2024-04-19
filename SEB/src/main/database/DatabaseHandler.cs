using Npgsql;
using System.Data;

namespace SEB.main.database
{
    internal class DatabaseHandler
    {
        public void Start(string connectionString)
        {
            Console.WriteLine("Creating tables...");
            CreateUsersTable(connectionString);
            CreatePushUpRecordsTable(connectionString);
            CreateTournamentsTable(connectionString);
            CreateTournamentEntriesTable(connectionString);

            Console.WriteLine("Finished creating tables...");
        }

        private static void CreateUsersTable(string connectionString)
        {
            try
            {
                using (IDbConnection connection = new NpgsqlConnection(connectionString))
                {
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE IF NOT EXISTS Users (
                                                    user_id SERIAL PRIMARY KEY,
                                                    username VARCHAR(50) NOT NULL UNIQUE,
                                                    passwordHash VARCHAR(60) NOT NULL,
                                                    userELO INT DEFAULT 100,
                                                    userToken VARCHAR(50) UNIQUE,
                                                    bio VARCHAR(50) DEFAULT '',
                                                    image VARCHAR(50) DEFAULT ''
                                                    );";

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Npgsql Error: {ex.Message}");
            }

            Console.WriteLine("Created Users table...");
        }

        private static void CreatePushUpRecordsTable(string connectionString)
        {
            try
            {
                using (IDbConnection connection = new NpgsqlConnection(connectionString))
                {
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE IF NOT EXISTS PushUpRecords (
                                                    RecordID SERIAL PRIMARY KEY,
                                                    UserID INT REFERENCES Users(User_ID) ON DELETE CASCADE,
                                                    Count INT,
                                                    Duration INT,
                                                    Timestamp TIMESTAMP DEFAULT current_timestamp
                                                );";

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Npgsql Error: {ex.Message}");
            }

            Console.WriteLine("Created PushUpRecords table...");
        }

        private static void CreateTournamentsTable(string connectionString)
        {
            try
            {
                using (IDbConnection connection = new NpgsqlConnection(connectionString))
                {
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE IF NOT EXISTS Tournaments (
                                                    TournamentID SERIAL PRIMARY KEY,
                                                    StartTime TIMESTAMP,
                                                    EndTime TIMESTAMP
                                                );";

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Npgsql Error: {ex.Message}");
            }

            Console.WriteLine("Created Tournaments table...");
        }

        private static void CreateTournamentEntriesTable(string connectionString)
        {
            try
            {
                using (IDbConnection connection = new NpgsqlConnection(connectionString))
                {
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE IF NOT EXISTS TournamentEntries (
                                                    EntryID SERIAL PRIMARY KEY,
                                                    TournamentID INT REFERENCES Tournaments(TournamentID),
                                                    UserID INT REFERENCES Users(User_ID),
                                                    RecordID INT REFERENCES PushUpRecords(RecordID)
                                                );";

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Npgsql Error: {ex.Message}");
            }

            Console.WriteLine("Created TournamentEntries table...");
        }

        public void DeleteTables(string connectionString)
        {
            string queryString =
                "DROP TABLE IF EXISTS TournamentEntries, Tournaments, PushUpRecords, Users CASCADE";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand(queryString, connection))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Tables deleted successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error with Database in DeleteTables: {ex.Message}");
                        throw;
                    }
                }
            }
        }

    }
}