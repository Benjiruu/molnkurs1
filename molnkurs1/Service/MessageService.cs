using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

        channel.ExchangeDeclare("update-listing", ExchangeType.Fanout);
    }

    // Skicka ett meddelande till andra microservices
    public void NotifyListingCreation(ListingDto listing)
    {
        var json = JsonSerializer.Serialize(listing);
        var message = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish("create-listing", string.Empty, null, message);
    }

    public void NotifyListingUpdate(string listingId)
    {
        var updateMessage = new { ListingId = listingId };
        var json = JsonSerializer.Serialize(updateMessage);
        var message = Encoding.UTF8.GetBytes(json);

        // Skickar meddelande 
        channel.BasicPublish("update-listing", string.Empty, null, message);
        Console.WriteLine($"Updated listing with ID: {listingId} sent to search service.");
    }

    private void ListenForProfileCreations()
    {
       
        channel.ExchangeDeclare(exchange: "create-user", type: ExchangeType.Fanout);

       
        var queueName = channel.QueueDeclare("user-queue", true, false, false).QueueName;
        channel.QueueBind(queue: queueName, exchange: "create-user", routingKey: string.Empty);

        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            try
            {
                
                var user = JsonSerializer.Deserialize<UserCreationMessage>(json);
                Console.WriteLine("Received userId: " + user.UserId);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error processing message: " + e.ToString());
            }
        };

  
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }


    // Anropas när programmet startas
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Connect();

        ListenForProfileCreations();

        return Task.CompletedTask;
    }

    //Logging
    public void SendLoggingActions(string action)
    {
        var message = Encoding.UTF8.GetBytes(action);
        channel.BasicPublish("logging", string.Empty, null, message);
    }

    // Anropas när programmet stoppas, och då kopplar vi bort från
    // RabbitMQ
    public Task StopAsync(CancellationToken cancellationToken)
    {
        channel.Close();
        connection.Close();
        return Task.CompletedTask;
    }

    public class UserCreationMessage
    {
        public string UserId { get; set; }
    }

}