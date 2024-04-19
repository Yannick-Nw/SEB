using Npgsql;
using SEB.httpServer.response;
using SEB.models;

namespace SEB.main.database;

public class UserSQL
{
    public void CreateUser(StreamWriter writer, UserCredentials userCredentials, string connectionString)
    {
        // Hash password
        // string salt = BCrypt.Net.BCrypt.GenerateSalt();
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userCredentials.Password);

        string queryString =
            "INSERT INTO users (username, passwordHash, userToken) VALUES (@username, @passwordHash, @usertoken)";
        using (var connection = new NpgsqlConnection(connectionString))
        {
            using (var command = new NpgsqlCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("username", userCredentials.Username);
                command.Parameters.AddWithValue("passwordHash", hashedPassword);
                command.Parameters.AddWithValue("usertoken", GenerateUserToken(userCredentials.Username));

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"{rowsAffected} row(s) inserted successfully.");
                    ResponseHandler rs = new ResponseHandler();
                    rs.SendPlaintextResponse(writer, "row(s) inserted successfully.", 201);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in CreateUser: {ex.Message}");
                    ResponseHandler rs = new ResponseHandler();
                    rs.SendErrorResponse(writer, "row(s) NOT inserted.", 409);
                    //throw;
                }
            }
        }
    }

    private string GenerateUserToken(string username)
    {
        return $"{username}-sebToken";
    }

    public void UpdateUserInfo(UserData userDataData, string username, string connectionString)
    {
        string updateQuery =
            "UPDATE users SET bio = @bio, image = @image, username = @newusername WHERE username = @username";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            using (var command = new NpgsqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("bio", userDataData.Bio);
                command.Parameters.AddWithValue("image", userDataData.Image);
                command.Parameters.AddWithValue("newusername", userDataData.Name);
                command.Parameters.AddWithValue("username", username);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"{rowsAffected} row(s) updated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating user info: " + ex.Message);
                }
            }
        }
    }

    public void GetUserInfo(UserData userData, string username, string connectionString)
    {
        string queryString = "SELECT username, bio, image FROM users WHERE username = @username";
        using (var connection = new NpgsqlConnection(connectionString))
        {
            using (var cmd = new NpgsqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("username", username);

                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userData.Name = reader["username"] as string;
                            userData.Bio = reader["bio"] as string;
                            userData.Image = reader["image"] as string;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with Database in GetUserInfo: {ex.Message}");
                    throw;
                }
            }
        }
    }

    public int GetUserId(string token, string connectionString)
    {
        string queryString = "SELECT user_id FROM users WHERE usertoken = @token";
        int userId = -1;
        using (var connection = new NpgsqlConnection(connectionString))
        {
            using (var cmd = new NpgsqlCommand(queryString, connection))
            {
                cmd.Parameters.AddWithValue("token", token);

                try
                {
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userId = reader.GetInt32(reader.GetOrdinal("user_id"));
                        }
                        else
                        {
                            Console.WriteLine("No userid for token found");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with Database in GetUserId: {ex.Message}");
                    throw;
                }
            }
        }

        return userId;
    }
}