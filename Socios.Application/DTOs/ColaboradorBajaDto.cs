using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs;
public class ColaboradorBajaDto
{
    [Required]
    public string Motivo { get; set; } = null!;
}
