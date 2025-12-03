using System.Security.Claims;
using ClientApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.ClientApi.Models;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Infrastructure.Data;

namespace TicketingSystem.ClientApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets(
            [FromQuery] string? search = null,
            [FromQuery] TicketStatus? status = null,
            [FromQuery] TicketPriority? priority = null,
            [FromQuery] bool? assignedToMe = false)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            var query = _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.AssignedTo)
                .AsQueryable();

            if (userRole == UserRole.Client)
            {
                query = query.Where(t => t.ClientId == userId);
            }
            else if (userRole == UserRole.TechSupport && assignedToMe == true)
            {
                query = query.Where(t => t.AssignedToId == userId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t =>
                    t.Title.Contains(search) ||
                    t.Description.Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            var tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var ticketDtos = tickets.Select(t => new TicketDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority.ToString(),
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ClosedAt = t.ClosedAt,
                ClientName = t.Client?.FullName ?? "Unknown",
                AssignedToName = t.AssignedTo?.FullName,
                MessageCount = t.Messages?.Count ?? 0
            }).ToList();

            return Ok(ticketDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDetailDto>> GetTicket(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.AssignedTo)
                .Include(t => t.Messages)
                    .ThenInclude(m => m.User)
                .Include(t => t.Messages)
                    .ThenInclude(m => m.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            if (!HasAccessToTicket(ticket))
            {
                return Forbid();
            }

            var ticketDto = MapToDetailDto(ticket);
            return Ok(ticketDto);
        }

        [HttpPost]
        public async Task<ActionResult<TicketDto>> CreateTicket(CreateTicketRequest request)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            if (userRole != UserRole.Client)
            {
                return Forbid();
            }

            var ticket = new Ticket
            {
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                Status = TicketStatus.Open,
                ClientId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(request.InitialMessage))
            {
                var message = new TicketMessage
                {
                    Content = request.InitialMessage,
                    TicketId = ticket.Id,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsInternal = false
                };

                _context.TicketMessages.Add(message);
                await _context.SaveChangesAsync();
            }

            var createdTicket = await _context.Tickets
                .Include(t => t.Client)
                .FirstOrDefaultAsync(t => t.Id == ticket.Id);

            return CreatedAtAction(nameof(GetTicket),
                new { id = ticket.Id },
                MapToDto(createdTicket));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, UpdateTicketRequest request)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            if (!CanUpdateTicket(ticket))
            {
                return Forbid();
            }

            ticket.Title = request.Title;
            ticket.Description = request.Description;
            ticket.Priority = request.Priority;
            ticket.Status = request.Status;
            ticket.AssignedToId = request.AssignedToId;
            ticket.UpdatedAt = DateTime.UtcNow;

            if (request.Status == TicketStatus.Closed || request.Status == TicketStatus.Resolved)
            {
                ticket.ClosedAt = DateTime.UtcNow;
                ticket.ClosingNotes = request.ClosingNotes ?? string.Empty;
            }
            else
            {
                ticket.ClosedAt = null;
                ticket.ClosingNotes = string.Empty;
            }

            await AddSystemMessageIfNeeded(ticket, request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/messages")]
        public async Task<ActionResult<MessageDto>> AddMessage(int id, AddMessageRequest request)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            if (!HasAccessToTicket(ticket))
            {
                return Forbid();
            }

            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            var message = new TicketMessage
            {
                Content = request.Content,
                TicketId = id,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsInternal = request.IsInternal && userRole == UserRole.TechSupport
            };

            _context.TicketMessages.Add(message);
            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            if (request.Attachments != null && request.Attachments.Any())
            {
                await ProcessAttachments(request.Attachments, message.Id);
            }

            var messageDto = await GetMessageDto(message.Id);
            return CreatedAtAction("GetMessage",
                new { ticketId = id, messageId = message.Id },
                messageDto);
        }

        [HttpGet("{id}/messages")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetTicketMessages(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            if (!HasAccessToTicket(ticket))
            {
                return Forbid();
            }

            var messages = await _context.TicketMessages
                .Include(m => m.User)
                .Include(m => m.Attachments)
                .Where(m => m.TicketId == id)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            var userRole = GetCurrentUserRole();

            if (userRole == UserRole.Client)
            {
                messages = messages.Where(m => !m.IsInternal).ToList();
            }

            var messageDtos = messages.Select(m => MapToMessageDto(m)).ToList();
            return Ok(messageDtos);
        }

        [HttpPost("{id}/assign")]
        [Authorize(Roles = "TechSupport")]
        public async Task<IActionResult> AssignToMe(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            var userId = GetCurrentUserId();
            ticket.AssignedToId = userId;
            ticket.UpdatedAt = DateTime.UtcNow;

            var message = new TicketMessage
            {
                Content = "Ticket assigned to me.",
                TicketId = ticket.Id,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsInternal = true
            };

            _context.TicketMessages.Add(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("stats")]
        [Authorize(Roles = "TechSupport")]
        public async Task<ActionResult<StatsDto>> GetStats()
        {
            var userId = GetCurrentUserId();

            var stats = new StatsDto
            {
                TotalTickets = await _context.Tickets.CountAsync(),
                OpenTickets = await _context.Tickets
                    .CountAsync(t => t.Status == TicketStatus.Open),
                InProgressTickets = await _context.Tickets
                    .CountAsync(t => t.Status == TicketStatus.InProgress),
                ClosedTickets = await _context.Tickets
                    .CountAsync(t => t.Status == TicketStatus.Closed ||
                                    t.Status == TicketStatus.Resolved),
                UnassignedTickets = await _context.Tickets
                    .CountAsync(t => t.AssignedToId == null &&
                                    t.Status != TicketStatus.Closed &&
                                    t.Status != TicketStatus.Resolved),
                MyAssignedTickets = await _context.Tickets
                    .CountAsync(t => t.AssignedToId == userId &&
                                    t.Status != TicketStatus.Closed &&
                                    t.Status != TicketStatus.Resolved)
            };

            return Ok(stats);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        private UserRole GetCurrentUserRole()
        {
            var claim = User.FindFirst(ClaimTypes.Role);
            return claim != null ? Enum.Parse<UserRole>(claim.Value) : UserRole.Client;
        }

        private bool HasAccessToTicket(Ticket ticket)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            if (userRole == UserRole.Client)
            {
                return ticket.ClientId == userId;
            }

            return true;
        }

        private bool CanUpdateTicket(Ticket ticket)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            if (userRole == UserRole.Client)
            {
                return ticket.ClientId == userId && ticket.Status != TicketStatus.Closed;
            }

            return userRole == UserRole.TechSupport;
        }

        private async Task AddSystemMessageIfNeeded(Ticket originalTicket, UpdateTicketRequest request)
        {
            var userId = GetCurrentUserId();
            var changes = new List<string>();

            if (originalTicket.Status != request.Status)
            {
                changes.Add($"Status changed from {originalTicket.Status} to {request.Status}");
            }

            if (originalTicket.AssignedToId != request.AssignedToId)
            {
                var oldAssignee = originalTicket.AssignedToId.HasValue
                    ? await _context.Users.FindAsync(originalTicket.AssignedToId.Value)
                    : null;
                var newAssignee = request.AssignedToId.HasValue
                    ? await _context.Users.FindAsync(request.AssignedToId.Value)
                    : null;

                changes.Add($"Assigned from {(oldAssignee?.FullName ?? "Unassigned")} to {(newAssignee?.FullName ?? "Unassigned")}");
            }

            if (changes.Any())
            {
                var message = new TicketMessage
                {
                    Content = string.Join(". ", changes),
                    TicketId = originalTicket.Id,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsInternal = true
                };

                _context.TicketMessages.Add(message);
            }
        }

        private async Task ProcessAttachments(List<IFormFile> attachments, int messageId)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            foreach (var file in attachments)
            {
                if (file.Length > 0)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(uploadsPath, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var attachment = new Attachment
                    {
                        FileName = file.FileName,
                        FilePath = $"/uploads/{uniqueFileName}",
                        ContentType = file.ContentType,
                        FileSize = file.Length,
                        UploadedAt = DateTime.UtcNow,
                        TicketMessageId = messageId
                    };

                    _context.Attachments.Add(attachment);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<MessageDto> GetMessageDto(int messageId)
        {
            var message = await _context.TicketMessages
                .Include(m => m.User)
                .Include(m => m.Attachments)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            return MapToMessageDto(message);
        }

        private TicketDto MapToDto(Ticket ticket)
        {
            return new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority.ToString(),
                Status = ticket.Status.ToString(),
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ClosedAt = ticket.ClosedAt,
                ClientName = ticket.Client?.FullName ?? "Unknown",
                AssignedToName = ticket.AssignedTo?.FullName,
                MessageCount = ticket.Messages?.Count ?? 0
            };
        }

        private TicketDetailDto MapToDetailDto(Ticket ticket)
        {
            var userRole = GetCurrentUserRole();
            var messages = ticket.Messages?.ToList() ?? new List<TicketMessage>();

            if (userRole == UserRole.Client)
            {
                messages = messages.Where(m => !m.IsInternal).ToList();
            }

            return new TicketDetailDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority.ToString(),
                Status = ticket.Status.ToString(),
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ClosedAt = ticket.ClosedAt,
                ClosingNotes = ticket.ClosingNotes,
                ClientName = ticket.Client?.FullName ?? "Unknown",
                AssignedToName = ticket.AssignedTo?.FullName,
                MessageCount = messages.Count,
                Client = ticket.Client != null ? new UserDto
                {
                    Id = ticket.Client.Id,
                    FullName = ticket.Client.FullName,
                    Email = ticket.Client.Email,
                    Role = ticket.Client.Role.ToString()
                } : null,
                AssignedTo = ticket.AssignedTo != null ? new UserDto
                {
                    Id = ticket.AssignedTo.Id,
                    FullName = ticket.AssignedTo.FullName,
                    Email = ticket.AssignedTo.Email,
                    Role = ticket.AssignedTo.Role.ToString()
                } : null,
                Messages = messages.Select(m => MapToMessageDto(m)).ToList()
            };
        }

        private MessageDto MapToMessageDto(TicketMessage message)
        {
            return new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                IsInternal = message.IsInternal,
                User = message.User != null ? new UserDto
                {
                    Id = message.User.Id,
                    FullName = message.User.FullName,
                    Email = message.User.Email,
                    Role = message.User.Role.ToString()
                } : null,
                Attachments = message.Attachments?.Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FileSize = a.FileSize,
                    ContentType = a.ContentType,
                    UploadedAt = a.UploadedAt,
                    FilePath = a.FilePath
                }).ToList() ?? new List<AttachmentDto>()
            };
        }
    }
}