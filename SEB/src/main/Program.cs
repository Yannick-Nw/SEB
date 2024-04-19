using SEB.httpServer.server;
using SEB.main.database;

namespace SEB
{
    class Program
    {
        static void Main(string[] args)
        {
            // DATABASE NAME: sebdb
            // USERNAME: user
            // PASSWORD: seb-password
            string connectionString =
                "Host=localhost;Database=sebdb;Username=user;Password=seb-password";
            DatabaseHandler db = new DatabaseHandler();
            db.Start(connectionString);

            HttpServer server = new HttpServer(10001);
            server.Start(connectionString);

            server.Stop();
        }
    }
}