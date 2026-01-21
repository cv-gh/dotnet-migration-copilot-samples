using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContosoUniversity.Services;
using ContosoUniversity.Models;
using ContosoUniversity.Data;

namespace ContosoUniversity.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly SchoolContext _db;
        protected readonly INotificationService _notificationService;
        protected readonly ILogger _logger;

        public BaseController(
            SchoolContext db,
            INotificationService notificationService,
            ILogger logger)
        {
            _db = db;
            _notificationService = notificationService;
            _logger = logger;
        }

        protected async Task SendEntityNotificationAsync(string entityType, string entityId, EntityOperation operation, CancellationToken cancellationToken = default)
        {
            await SendEntityNotificationAsync(entityType, entityId, null, operation, cancellationToken);
        }

        protected async Task SendEntityNotificationAsync(string entityType, string entityId, string? entityDisplayName, EntityOperation operation, CancellationToken cancellationToken = default)
        {
            try
            {
                var userName = User?.Identity?.Name ?? "System";
                await _notificationService.SendNotificationAsync(entityType, entityId, entityDisplayName, operation, userName);
            }
            catch (Exception ex)
            {
                // Log the error but don't break the main operation
                _logger.LogError(ex, "Failed to send notification for {EntityType} {EntityId}", entityType, entityId);
            }
        }

        protected override void Dispose(bool disposing)
        {
            // DbContext is disposed by the DI container
            base.Dispose(disposing);
        }
    }
}
