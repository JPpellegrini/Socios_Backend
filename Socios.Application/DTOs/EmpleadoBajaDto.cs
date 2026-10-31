using System.ComponentModel.DataAnnotations;

public class EmpleadoBajaDto
{
    [Required]
    public string Motivo { get; set; } = null!;
}
