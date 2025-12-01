using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Client.Application.Commands;

public class ReopenTicketCommand : IRequest<Unit>
{
    [Required]
    public int TicketId { get; set; }

    [Required]
    public int ClientId { get; set; }
}