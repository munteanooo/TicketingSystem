using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Client.Application.Commands;

public class CloseTicketCommand : IRequest<Unit>
{
    [Required]
    public int TicketId { get; set; }

    [Required]
    public int ClientId { get; set; }

    [StringLength(1000)]
    public string ClosingNotes { get; set; } = string.Empty;
}