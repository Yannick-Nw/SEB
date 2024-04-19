using System.Text.Json;

namespace SEB.models;

public class History
{
    public int Count { get; set; }
    public int DurationInSeconds { get; set; }
    public int UserId { get; set; }

    public void Read(string req)
    {
        Console.WriteLine(req);
        int jsonStart = req.IndexOf("{");
        int jsonEnd = req.LastIndexOf("}") + 1;
        string jsonString = req.Substring(jsonStart, jsonEnd - jsonStart);
        History data = JsonSerializer.Deserialize<History>(jsonString);
        this.Count = data.Count;
        this.DurationInSeconds = data.DurationInSeconds;
        //this.UserId = id;
    }
}