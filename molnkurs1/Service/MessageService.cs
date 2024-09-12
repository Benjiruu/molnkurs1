using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
public class MessageService : IHostedService
{

    // Två variabler för att spara kopplingen till RabbitMQ
    private IConnection connection;
    private IModel channel;

    // Anslut till RabbitMQ
    public void Connect()
    {
        System.Console.WriteLine("HEJ!");
        var factory = new ConnectionFactory { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        // Skapa en exchange för att kunna skicka meddelanden
        // till andra microservices
        channel.ExchangeDeclare("create-listing", ExchangeType.Fanout);
    }

    // Skicka ett meddelande till andra microservices
    public void NotifyListingCreation(ListingDto listing)
    {
        var json = JsonSerializer.Serialize(listing);
        var message = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish("create-listing", string.Empty, null, message);
    }

    // Anropas när programmet startas
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Connect();
        return Task.CompletedTask;
    }

    // Anropas när programmet stoppas, och då kopplar vi bort från
    // RabbitMQ
    public Task StopAsync(CancellationToken cancellationToken)
    {
        channel.Close();
        connection.Close();
        return Task.CompletedTask;
    }
}