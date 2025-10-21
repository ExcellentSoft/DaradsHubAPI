using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Customs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

    public IQueryable<AgentDatum> GetUnreadChatMessages()
    {
        var last4hours = GetLocalDateTime.CurrentDateTime().AddHours(-4);
        var message = (from c in _context.HubChatConversations
                       join m in _context.HubChatMessages on c.Id equals m.ConversationId
                       join u in _context.userstb on c.AgentId equals u.id
                       where m.SentAt < last4hours && m.IsRead == false
                       select u).GroupBy(g => new { g.email, g.fullname }).Select(r => new AgentDatum
                       {
                           Email = r.Key.email,
                           FullName = r.Key.fullname,
                       });

        var _message = (from m in _context.HubChatMessages
                        join c in _context.HubChatConversations on m.ConversationId equals c.Id
                        join u in _context.userstb on c.AgentId equals u.id
                        where m.SentAt < last4hours && m.IsRead == false
                        select u).GroupBy(g => new { g.email, g.fullname }).Select(r => new AgentDatum
                        {
                            Email = r.Key.email,
                            FullName = r.Key.fullname,
                        }).ToList();

        return message;
    }

    public IQueryable<ViewChatMessagesResponse> GetAgentChatMessages(int agentId)
    {
        var message = (from c in _context.HubChatConversations
                       where c.AgentId == agentId
                       join m in _context.HubChatMessages on c.Id equals m.ConversationId
                       join u in _context.userstb on m.SenderId equals u.id
                       orderby m.SentAt descending
                       select new { u, m, c }).GroupBy(g => g.c.Id).Select(w => new ViewChatMessagesResponse
                       {
                           ConversationId = w.Key,
                           TotalPendingCount = w.Count(e => e.m.IsRead == false),
                           LastMessage = w.Select(r => new LastMessage
                           {
                               Content = r.m.Content,
                               SentAt = r.m.SentAt,
                               CustomerDetails = _context.userstb.Where(e => e.id == w.Select(d => d.c.CustomerId).FirstOrDefault()).Select(e => new SenderDetails
                               {
                                   FullName = e.fullname,
                                   Photo = e.Photo,
                                   userId = e.id,
                                   IsAgent = e.IsAgent
                               }).FirstOrDefault(),
                               AgentDetails = _context.userstb.Where(e => e.id == w.Select(d => d.c.AgentId).FirstOrDefault()).Select(e => new SenderDetails
                               {
                                   FullName = e.fullname,
                                   PhoneNumber = e.phone,
                                   Photo = e.Photo,
                                   userId = e.id,
                                   IsAgent = e.IsAgent
                               }).FirstOrDefault()
                           }).OrderBy(e => e.SentAt).LastOrDefault()
                       });
        return message;
    }

    public IQueryable<ViewChatMessagesResponse> GetCustomerChatMessages(int customerId)
    {
        var message = (from c in _context.HubChatConversations
                       where c.CustomerId == customerId
                       join m in _context.HubChatMessages on c.Id equals m.ConversationId
                       join u in _context.userstb on m.SenderId equals u.id
                       orderby m.SentAt descending
                       select new { u, m, c }).GroupBy(g => g.c.Id).Select(w => new ViewChatMessagesResponse
                       {
                           ConversationId = w.Key,
                           TotalPendingCount = w.Count(e => e.m.IsRead == false),
                           LastMessage = w.Select(r => new LastMessage
                           {
                               Content = r.m.Content,
                               SentAt = r.m.SentAt,
                               CustomerDetails = _context.userstb.Where(e => e.id == w.Select(d => d.c.CustomerId).FirstOrDefault()).Select(e => new SenderDetails
                               {
                                   FullName = e.fullname,
                                   PhoneNumber = e.phone,
                                   Photo = e.Photo,
                                   userId = e.id,
                                   IsAgent = e.IsAgent
                               }).FirstOrDefault(),
                               AgentDetails = _context.userstb.Where(e => e.id == w.Select(d => d.c.AgentId).FirstOrDefault()).Select(e => new SenderDetails
                               {
                                   FullName = e.fullname,
                                   PhoneNumber = e.phone,
                                   Photo = e.Photo,
                                   userId = e.id,
                                   IsAgent = e.IsAgent
                               }).FirstOrDefault()
                           }).OrderBy(e => e.SentAt).LastOrDefault()
                       });
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
        var today = GetLocalDateTime.CurrentDateTime();
        await _context.HubNotifications.Where(n => n.NoteToEmail == email && n.IsRead == false)
             .ExecuteUpdateAsync(s =>
             s.SetProperty(c => c.IsRead, true)
             .SetProperty(c => c.TimeCreated, today));
    }

    public async Task MarkAllMessageAsRead(long conversationId)
    {
        await _context.HubChatMessages.Where(n => n.ConversationId == conversationId && n.IsRead == false)
             .ExecuteUpdateAsync(s =>
             s.SetProperty(c => c.IsRead, true));
    }

    public async Task ReportAgent(ReportAgent report)
    {
        _context.ReportAgents.Add(report);
        await _context.SaveChangesAsync();
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
