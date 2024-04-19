using System.Text.Json;

namespace SEB.models;

public class UserData
{
    public string Name { get; set; } = "";
    public string Bio { get; set; } = "";
    public string Image { get; set; } = "";

    public void Read(string req)
    {
        UserData data = JsonSerializer.Deserialize<UserData>(req);
        this.Name = data.Name;
        this.Bio = data.Bio;
        this.Image = data.Image;
    }
}