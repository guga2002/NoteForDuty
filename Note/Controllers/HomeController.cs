using Microsoft.AspNetCore.Mvc;
using Note.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Speaker.leison.Sistem.layer.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private IUdpComunicationRepository _udpService;

    // Field to hold old data
    private static List<ExcellDataMode3l> _oldData = new List<ExcellDataMode3l>();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _udpService = new UdpComunicationRepository();
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var uri = new Uri("amqps://username:password@kangaroo.rmq.cloudamqp.com/vhost");

            var factory = new ConnectionFactory
            {
                Uri = uri,
                Port = 5671,
                UserName = "aevgmbez",
                VirtualHost = "aevgmbez",
                Password = "9sslko6b6JJXlTzxTECydNL59ww0KfBp"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var queueName = "Tabaxmela_Excel";
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var newData = JsonSerializer.Deserialize<List<ExcellDataMode3l>>(message);

                    if (newData != null)
                    {
                        foreach (var item in newData)
                        {
                            // Check if the item already exists in _oldData
                            var existingItem = _oldData.FirstOrDefault(x => x.Index == item.Index);
                            if (existingItem != null)
                            {
                                // Update existing item if new data is not null
                                if (item != null)
                                {
                                    existingItem.job = item.job ?? existingItem.job; // Use old value if new is null
                                    existingItem.ChanellName = item.ChanellName ?? existingItem.ChanellName;
                                    existingItem.Date = item.Date ?? existingItem.Date;
                                    existingItem.endtime = item.endtime ?? existingItem.endtime;
                                    existingItem.OnDuty = item.OnDuty ?? existingItem.OnDuty;
                                    existingItem.AdditionInfo = item.AdditionInfo ?? existingItem.AdditionInfo;
                                    existingItem.ShouldSeen = item.ShouldSeen ?? existingItem.ShouldSeen;
                                }
                            }
                            else
                            {
                                // Add as new item
                                _oldData.Add(item);
                            }
                        }
                    }
                };

                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            }

            // Return the current list of data
            return View(_oldData);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred: " + ex.Message);
            return View(new List<ExcellDataMode3l>()); // Return empty on error
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var updatedData = await _udpService.ReceiveAsync(); // Adjust this method as needed
        return Json(updatedData);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
