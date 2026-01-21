using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContosoUniversity.Services;
using ContosoUniversity.Models;
using ContosoUniversity.Data;

namespace ContosoUniversity.Controllers
{
    public class NotificationsController : BaseController
    {
        public NotificationsController(
            SchoolContext db,
            INotificationService notificationService,
            ILogger<NotificationsController> logger)
            : base(db, notificationService, logger)
        {
        }

        // GET: api/notifications - Get pending notifications for admin
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = new List<Notification>();
            
            try
            {
                // Read all available notifications from the queue
                Notification? notification;
                while ((notification = await _notificationService.ReceiveNotificationAsync()) != null)
                {
                    notifications.Add(notification);
                    
                    // Limit to prevent overwhelming the UI
                    if (notifications.Count >= 10)
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications");
                return Json(new { success = false, message = "Error retrieving notifications" });
            }

            return Json(new { 
                success = true, 
                notifications = notifications,
                count = notifications.Count 
            });
        }

        // POST: api/notifications/mark-read
        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            try
            {
                // Note: MarkAsRead is not implemented in the new NotificationService
                // In a real implementation, this would mark the notification in a database
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return Json(new { success = false, message = "Error updating notification" });
            }
        }

        // GET: Notifications/Index - Admin notification dashboard
        public IActionResult Index()
        {
            return View();
        }
    }
}
