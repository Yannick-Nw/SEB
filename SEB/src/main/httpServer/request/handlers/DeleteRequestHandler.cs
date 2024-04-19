using SEB.httpServer.response;
using SEB.main.database;
using SEB.models;

namespace SEB.main.httpServer.request.handlers;

public class DeleteRequestHandler
{
    public void DeleteRequest(StreamWriter writer, string data, string route, string
        connectionString)
    {
        if (route == "/delete")
        {
            Console.WriteLine("Path: Delete");

            UserCredentials userdata = new UserCredentials();
            userdata.TokenSearch(data);

            if (userdata.Token == "admin-sebToken")
            {
                DatabaseHandler execute = new DatabaseHandler();
                execute.DeleteTables(connectionString);
            }
            else
            {
                Console.WriteLine("Wrong user");
                ResponseHandler rs = new ResponseHandler();
                rs.SendErrorResponse(writer, "Wrong user", 403);
            }
        }
        else
        {
            Console.WriteLine("Route does not exist");
            ResponseHandler rs = new ResponseHandler();
            rs.SendErrorResponse(writer, "Route does not exist", 404);
        }
    }
}