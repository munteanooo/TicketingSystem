using MediatR;
using System.ComponentModel.DataAnnotations;
using TicketingSystem.Domain.Enums;

namespace Client.Application.Commands;

public class CreateTicketCommand : IRequest<int>
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public TicketPriority Priority { get; set; }

    [Required]
    public int ClientId { get; set; }
}