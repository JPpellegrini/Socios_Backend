using System.ComponentModel.DataAnnotations;

namespace Socios.Application.DTOs
{
    public class EntidadFiltroBasicoDto
    {
        public string? Dni { get; set; }
        public string? Busqueda { get; set; }
        public bool IncluirInactivos { get; set; } = false;
    }
}
