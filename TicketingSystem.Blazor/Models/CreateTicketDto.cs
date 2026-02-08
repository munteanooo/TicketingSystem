using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Blazor.Models.DTOs;

public class CreateTicketDto
{
    [Required(ErrorMessage = "Titlul este obligatoriu")]
    [StringLength(100, ErrorMessage = "Titlul este prea lung")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Descrierea este obligatorie")]
    [MinLength(10, ErrorMessage = "Descrierea trebuie să aibă cel puțin 10 caractere")]
    public string Description { get; set; } = string.Empty;
}