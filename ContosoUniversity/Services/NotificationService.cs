using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using ContosoUniversity.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContosoUniversity.Services
{
    public class NotificationService : INotificationService, IDisposable
    {
        private readonly ServiceBusClient? _client;
        private readonly ServiceBusSender? _sender;
        private readonly ServiceBusReceiver? _receiver;
        private readonly string _queueName;
        private readonly ILogger<NotificationService> _logger;
        private readonly bool _isConfigured;

        public NotificationService(IConfiguration configuration, ILogger<NotificationService> logger)
        {
            _logger = logger;
            _queueName = configuration["Azure:ServiceBus:QueueName"] ?? "contoso-notifications";
            
            var serviceBusConnectionString = configuration["Azure:ServiceBus:ConnectionString"];
            var serviceBusNamespace = configuration["Azure:ServiceBus:Namespace"];
            
            // Check if Service Bus is configured
            _isConfigured = !string.IsNullOrEmpty(serviceBusConnectionString) || 
                           !string.IsNullOrEmpty(serviceBusNamespace);
            
            if (_isConfigured)
            {
                try
                {
                    // Create Service Bus client
                    if (!string.IsNullOrEmpty(serviceBusConnectionString))
                    {
                        _client = new ServiceBusClient(serviceBusConnectionString);
                    }
                    else if (!string.IsNullOrEmpty(serviceBusNamespace))
                    {
                        // Use Managed Identity for Azure
                        _client = new ServiceBusClient($"{serviceBusNamespace}.servicebus.windows.net", 
                            new Azure.Identity.DefaultAzureCredential());
                    }

                    if (_client != null)
                    {
                        _sender = _client.CreateSender(_queueName);
                        _receiver = _client.CreateReceiver(_queueName);
                        _logger.LogInformation("Azure Service Bus notification service initialized");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to initialize Azure Service Bus. Notifications will be disabled.");
                    _isConfigured = false;
                }
            }
            else
            {
                _logger.LogInformation("Azure Service Bus not configured. Notifications will be disabled.");
            }
        }

        public async Task SendNotificationAsync(string entityType, string entityId, EntityOperation operation, string? userName = null)
        {
            await SendNotificationAsync(entityType, entityId, null, operation, userName);
        }

        public async Task SendNotificationAsync(string entityType, string entityId, string? entityDisplayName, 
            EntityOperation operation, string? userName = null)
        {
            if (!_isConfigured || _sender == null)
            {
                _logger.LogDebug("Service Bus not configured. Skipping notification for {EntityType} {EntityId}", 
                    entityType, entityId);
                return;
            }

            try
            {
                var notification = new Notification
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    Operation = operation.ToString(),
                    Message = GenerateMessage(entityType, entityId, entityDisplayName, operation),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userName ?? "System",
                    IsRead = false
                };

                var jsonMessage = JsonSerializer.Serialize(notification);
                var message = new ServiceBusMessage(jsonMessage)
                {
                    Subject = $"{entityType} {operation}",
                    ContentType = "application/json"
                };

                await _sender.SendMessageAsync(message);
                _logger.LogInformation("Notification sent for {EntityType} {EntityId}", entityType, entityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification for {EntityType} {EntityId}", entityType, entityId);
            }
        }

        public async Task<Notification?> ReceiveNotificationAsync()
        {
            if (!_isConfigured || _receiver == null)
            {
                return null;
            }

            try
            {
                var message = await _receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(1));
                if (message != null)
                {
                    var jsonContent = message.Body.ToString();
                    var notification = JsonSerializer.Deserialize<Notification>(jsonContent);
                    
                    // Complete the message to remove it from the queue
                    await _receiver.CompleteMessageAsync(message);
                    
                    return notification;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to receive notification");
            }

            return null;
        }

        private string GenerateMessage(string entityType, string entityId, string? entityDisplayName, 
            EntityOperation operation)
        {
            var displayText = !string.IsNullOrWhiteSpace(entityDisplayName) 
                ? $"{entityType} '{entityDisplayName}'" 
                : $"{entityType} (ID: {entityId})";

            return operation switch
            {
                EntityOperation.CREATE => $"New {displayText} has been created",
                EntityOperation.UPDATE => $"{displayText} has been updated",
                EntityOperation.DELETE => $"{displayText} has been deleted",
                _ => $"{displayText} operation: {operation}"
            };
        }

        public void Dispose()
        {
            _sender?.DisposeAsync().AsTask().Wait();
            _receiver?.DisposeAsync().AsTask().Wait();
            _client?.DisposeAsync().AsTask().Wait();
        }
    }
}
