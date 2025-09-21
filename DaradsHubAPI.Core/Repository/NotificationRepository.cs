using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Customs;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace DaradsHubAPI.Core.Repository;
public class NotificationRepository(AppDbContext _context) : GenericRepository<HubNotification>(_context), INotificationRepository
{
    public async Task DeleteNotification(long Id)
    {
        var entity = await _context.HubNotifications.FirstOrDefaultAsync(x => x.Id == Id);
        if (entity is not null)
        {
            _context.HubNotifications.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<NotificationRequest>> GetAllNotificationsAsync(string email)
    {
        var today = GetLocalDateTime.CurrentDateTime();
        return await _context.HubNotifications.Where(n => n.NoteToEmail == email && n.IsRead == false).Select(n => new NotificationRequest
        {
            NotificationDate = n.TimeCreated,
            Message = n.Message,
            Id = n.Id,
            Title = n.Title,
            Duration = CustomizeCodes.GetPeriodDifference(n.TimeCreated, today)
        }).OrderByDescending(n => n.NotificationDate).ToListAsync();
    }

    public IQueryable<ChatMessageResponse> GetChatMessages(long conversationId)
    {
        var message = from m in _context.HubChatMessages
                      where m.ConversationId == conversationId
                      join u in _context.userstb on m.SenderId equals u.id
                      orderby m.SentAt descending
                      select new ChatMessageResponse
                      {
                          Sender = new SenderDetails
                          {
                              FullName = u.fullname,
                              Photo = u.Photo,
                              userId = u.id,
                              IsAgent = u.IsAgent
                          },
                          Content = m.Content,
                          ConversationId = m.ConversationId,
                          SentAt = m.SentAt,
                          IsRead = m.IsRead,
                          MessageId = m.Id
                      };

        return message;
    }

    public async Task SaveNotification(HubNotification entity)
    {
        await _context.HubNotifications.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChatMessage(HubChatMessage entity)
    {
        await _context.HubChatMessages.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task MarkNotificationAsRead(long id)
    {
        var notify = await _context.HubNotifications.FirstOrDefaultAsync(x => x.Id == id);
        if (notify != null)
        {
            notify.IsRead = true;
            _context.HubNotifications.Update(notify);
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllNotificationAsRead(string email)
    {
        await _context.HubNotifications.Where(n => n.NoteToEmail == email && n.IsRead == false)
             .ExecuteUpdateAsync(s =>
             s.SetProperty(c => c.IsRead, true)
             .SetProperty(c => c.TimeCreated, GetLocalDateTime.CurrentDateTime()));
    }

    public async Task MarkAllMessageAsRead(long conversationId)
    {
        await _context.HubChatMessages.Where(n => n.ConversationId == conversationId && n.IsRead == false)
             .ExecuteUpdateAsync(s =>
             s.SetProperty(c => c.IsRead, true));
    }

    public IQueryable<NotificationResponse> GetAllNotificationsAsync(NotificationListRequest request)
    {
        var today = GetLocalDateTime.CurrentDateTime();
        var notification = from n in _context.HubNotifications
                           where request.NotificationType == null || n.NotificationType == request.NotificationType
                           orderby n.TimeCreated descending
                           select new NotificationResponse
                           {
                               Message = n.Message,
                               Id = n.Id,
                               Title = n.Title,
                               NotificationType = n.NotificationType,
                               Duration = CustomizeCodes.GetPeriodDifference(n.TimeCreated, today)
                           };

        return notification;
    }

    #region Chat

    public async Task<HubChatConversation> GetOrCreateConversation(CreateConversationRequest request)
    {
        var conversation = await _context.HubChatConversations
            .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId && c.AgentId == request.AgentId);
        if (conversation == null)
        {
            conversation = new HubChatConversation
            {
                CustomerId = request.CustomerId,
                AgentId = request.AgentId,
                DateCreated = GetLocalDateTime.CurrentDateTime()
            };
            _context.HubChatConversations.Add(conversation);
            await _context.SaveChangesAsync();
        }
        return conversation;
    }

    #endregion
}
