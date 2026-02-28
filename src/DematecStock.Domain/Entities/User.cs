using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DematecStock.Domain.Entities
{
    [Table("Usuarios")]
    public class User
    {
        [Key]
        [Column("CodUsuario")]
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        [Column("SenhaApp")]
        public string? AppPassword { get; set; }

        [Column("Nome")]
        public string? Name { get; set; }

    }
}