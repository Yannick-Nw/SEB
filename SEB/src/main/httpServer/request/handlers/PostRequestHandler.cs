using SEB.httpServer.response;
using SEB.main.database;
using SEB.models;

namespace SEB.main.httpServer.request.handlers;

public class PostRequestHandler
{
    public void PostRequest(StreamWriter writer, string data, string route, string
        connectionString)
    {
        if (route == "/users")
        {
            Console.WriteLine("Path: User");
            UserCredentials userdata = new UserCredentials();
            userdata.Read(data);

            UserSQL execute = new UserSQL();
            execute.CreateUser(writer, userdata, connectionString);
        }
        else if (route == "/sessions")
        {
            Console.WriteLine("Path: Session");
            UserCredentials userdata = new UserCredentials();
            userdata.Read(data);

            SessionSQL execute = new SessionSQL();
            execute.UserLogin(userdata, connectionString);
        }
        else if (route == "/history")
        {
            Console.WriteLine("Path: History");
            TournamentSQL executefirst = new TournamentSQL();
            int tournamentID = executefirst.CreateTournament(connectionString);

            UserCredentials userdata = new UserCredentials();
            userdata.TokenSearch(data);

            History entry = new History();
            entry.Read(data);

            UserSQL executesecond = new UserSQL();
            entry.UserId = executesecond.GetUserId(userdata.Token, connectionString);

            HistorySQL executethird = new HistorySQL();
            executethird.newPushUpRecord(entry, tournamentID, connectionString);
        }
        else
        {
            Console.WriteLine("Route does not exist");
            ResponseHandler rs = new ResponseHandler();
            rs.SendErrorResponse(writer, "Route does not exist", 404);
        }
    }
}