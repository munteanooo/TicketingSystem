using MediatR;

namespace Client.Application.Commands
{
    public class CreateTicketCommand : IRequest<int>  
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ClientId { get; set; }
    }
}