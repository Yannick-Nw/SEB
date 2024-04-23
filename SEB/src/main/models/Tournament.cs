namespace SEB.models;

public class Tournament
{
    public DateTime StartTime { get; set; }
    public int Participants { get; set; }
    public int Leader { get; set; }
    public int TotalPushUps { get; set; }
    
    public int TournamentId { get; set; }
}