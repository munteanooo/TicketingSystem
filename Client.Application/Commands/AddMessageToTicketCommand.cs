using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Interfaces;

public class AddMessageToTicketCommand : IRequest<TicketMessageDto>
{
    public int TicketId { get; set; }
    public string Content { get; set; }
    public int AuthorId { get; set; }
}

public class AddMessageToTicketCommandHandler : IRequestHandler<AddMessageToTicketCommand, TicketMessageDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddMessageToTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketMessageDto> Handle(AddMessageToTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId);
        if (ticket == null)
            throw new Exception($"Ticket {request.TicketId} not found");

        var message = new TicketMessage(request.Content, request.AuthorId);
        ticket.AddMessage(message);

        await _unitOfWork.Tickets.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync();

        return new TicketMessageDto
        {
            Id = message.Id,
            TicketId = request.TicketId,
            Content = message.Content,
            AuthorId = message.AuthorId,
            CreatedAt = message.CreatedAt
        };
    }
}