using Npgsql;
using SEB.models;

namespace SEB.main.database;

public class SessionSQL
{
    //public string UserToken { get; private set; } // needs to be public so it can be returned to user via HttpResponse
    public void UserLogin(UserCredentials userDataCredentials, string connectionString)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            string queryString = "SELECT passwordHash FROM Users WHERE username = @username";
            using (var command = new NpgsqlCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("username", userDataCredentials.Username);
                string userToken = "";
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string passwordHash = reader.GetString(0);
                            if (BCrypt.Net.BCrypt.Verify(userDataCredentials.Password, passwordHash))
                            {
                                Console.WriteLine("Login Successful");
                                userDataCredentials.Token = UpdateUserToken(userDataCredentials.Username, connectionString);
                            }
                            else
                            {
                                throw new Exception("Error Login credentials");
                            }
                        }
                        else
                        {
                            throw new Exception("User not found");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in DatabaseLogin: " + ex.Message);
                }
            }
        }
    }

    private string UpdateUserToken(string username, string connectionString)
    {
        string updateQuery =
            "UPDATE users SET userToken = @userToken WHERE username = @username";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            using (var command = new NpgsqlCommand(updateQuery, connection))
            {
                string token = GenerateUserToken(username);
                command.Parameters.AddWithValue("userToken", token);
                command.Parameters.AddWithValue("username", username);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.WriteLine("User token updated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating user token: " + ex.Message);
                }

                return token;
            }
        }
    }

    private string GenerateUserToken(string username)
    {
        return $"{username}-sebToken";
    }

    //public bool
}
