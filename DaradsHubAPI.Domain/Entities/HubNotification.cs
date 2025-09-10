using System.ComponentModel.DataAnnotations;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Domain.Entities;

public class HubNotification
{
    [Key]
    public long Id { get; set; }

    [MaxLength(800)]
    public string Message { get; set; } = default!;
    public bool IsRead { get; set; }
    [MaxLength(100)]
    public string? Title { get; set; }
    [MaxLength(100)]
    public string NoteToEmail { get; set; } = default!;
    public NotificationType NotificationType { get; set; }
    public DateTime TimeCreated { get; set; }
}