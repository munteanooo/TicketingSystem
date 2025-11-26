using MediatR;

namespace Tech.Application.Commands
{
    public class AssignTicketCommand : IRequest<Unit>
    {
        public int TicketId { get; set; }
        public int TechSupportId { get; set; }
    }
}