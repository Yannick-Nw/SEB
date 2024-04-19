using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace SEB.main.httpServer.request.handlers;

public class PutRequestHandler
{
    string req;
    string data;
    string response;

    TcpClient client;
    StreamReader reader;
    StreamWriter writer;

    public PutRequestHandler(TcpClient client, StreamReader reader, StreamWriter writer, string req)
    {

        string[] reqLines = req.Split("\n");
        string[] route = reqLines[0].Split(" ");

        if (Regex.IsMatch(route[1], @"/users/[a-zA-Z]*"))
        {
            //UserSQL.UpdateUserInfo(route[1]);
        }
        else
        {
            //throw
        }
    }
}