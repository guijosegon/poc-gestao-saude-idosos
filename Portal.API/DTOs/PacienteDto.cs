using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoSaudeIdosos.API.DTOs
{
    public class PacienteDto
    {
        public int? PacienteId { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data de nascimento é obrigatória")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        public int? ResponsavelId { get; set; }
    }
}
