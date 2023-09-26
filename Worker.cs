using System.IO.Pipes;
using System.Text;

namespace NotificationSender
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private NamedPipeClientStream _pipeClient;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Connect to the named pipe server
                    _pipeClient = new NamedPipeClientStream(".", "NotificationPipe", PipeDirection.Out);
                    _pipeClient.Connect();

                    // Send a message to the notification display component
                    string message = "New Message: " + DateTime.Now.ToString();
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    _pipeClient.Write(messageBytes, 0, messageBytes.Length);

                    // Disconnect from the named pipe
                    _pipeClient.Close();

                    _logger.LogInformation("Message sent: " + message);

                    await Task.Delay(1000*10, stoppingToken); // Check for messages every minute (adjust as needed).
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error sending message: " + ex.Message);
                }
            }
        }

    }
}