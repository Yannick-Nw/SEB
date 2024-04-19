using System.Text.Json;

namespace SEB.models;

public class UserCredentials
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Token { get; set; }

    public void Read(string req)
    {
        Console.WriteLine(req);
        int jsonStart = req.IndexOf("{");
        int jsonEnd = req.LastIndexOf("}") + 1;
        string jsonString = req.Substring(jsonStart, jsonEnd - jsonStart);
        UserCredentials data = JsonSerializer.Deserialize<UserCredentials>(jsonString);
        this.Username = data.Username;
        this.Password = data.Password;
    }

    public void TokenSearch(string req)
    {
        string[] lines = req.Split('\n');

        foreach (string line in lines)
        {
            if (line.StartsWith("Authorization: Basic "))
            {
                this.Token = line.Substring("Authorization: Basic ".Length).TrimEnd('\r');
                break;
            }
        }

    }
}