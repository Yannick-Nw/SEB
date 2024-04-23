using System.Net.Sockets;
using SEB.httpServer.response;
using SEB.main.database;
using SEB.models;

namespace SEB.main.httpServer.request.handlers;

internal class GetRequestHandler
{
    public void GetRequest(TcpClient client, StreamWriter writer, string data, string path, string
        connectionString)
    {
        
        string[] route = path.Split("/");

        if (route[1] == "users")
        {
            Console.WriteLine("Path: Users");

            UserData userdata = new UserData();
            UserSQL execute = new UserSQL();
            execute.GetUserInfo(userdata, route[2], connectionString);
        }
        else if (route[1] == "stats")
        {
            Console.WriteLine("Path: Stats");

            UserStats userstats = new UserStats();
            StatsSQL execute = new StatsSQL();

            UserCredentials userdata = new UserCredentials();
            userdata.TokenSearch(data);

            UserSQL executesecond = new UserSQL();
            int userId = executesecond.GetUserId(userdata.Token, connectionString);
            execute.GetUserStats(writer, userstats, userId, connectionString);
        }
        else if (route[1] == "score")
        {
            Console.WriteLine("Path: Score");
            List<UserStats> userStatsList = new List<UserStats>();
            ScoreSQL execute = new ScoreSQL();
            execute.GetAllUserStats(writer, userStatsList, connectionString);
        }
        else if (route[1] == "history")
        {
            Console.WriteLine("Path: History");
            
            UserCredentials userdata = new UserCredentials();
            userdata.TokenSearch(data);

            List<History> entry = new List<History>();
            
            HistorySQL execute = new HistorySQL();
            execute.GetUserHistory(writer, entry, connectionString, userdata.Token);
        }
        else if (route[1] == "tournament")
        {
            Tournament tournamentdatalookup = new Tournament();
            TournamentSQL executelookup = new TournamentSQL();
            executelookup.GetCurrentTournamentState(tournamentdatalookup, connectionString, 2);

            if ((DateTime.Now - tournamentdatalookup.StartTime).TotalMinutes > 2)
            {
                Console.WriteLine("The Tournament finished more than 2 minutes ago");
                StatsSQL executeT = new StatsSQL();
                executeT.UpdateUserStats(tournamentdatalookup.Leader, tournamentdatalookup.TournamentId,
                    connectionString);
            }
            
            Console.WriteLine("Path: Tournament");
            Tournament tournamentdata = new Tournament();
            TournamentSQL execute = new TournamentSQL();
            execute.GetCurrentTournamentState(tournamentdata, connectionString, 1);
        }
        else
        {
            Console.WriteLine("Route does not exist");
            ResponseHandler rs = new ResponseHandler();
            rs.SendErrorResponse(writer, "Route does not exist", 404);
        }
    }
}