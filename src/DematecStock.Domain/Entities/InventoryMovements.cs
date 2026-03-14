using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DematecStock.Domain.Entities
{
    [Table("wmsInventoryMovement")]
    public class InventoryMovements
    {
        [Key]
        [Column("IdMovement")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("MovementDate")]
        public DateTimeOffset MovementDate { get; set; }

        [Column("IdUser")]
        public int IdUser { get; set; }

        [Column("Operation")]
        public string Operation { get; set; } = string.Empty;

        [Column("MovementDirection")]
        public char MovementDirection { get; set; } //'S' para saída, 'E' para entrada

        [Column("IdProduct")]
        public int IdProduct { get; set; }

        [Column("IdLocation")]
        public int IdLocation { get; set; }

        [Column("Quantity")]
        public decimal Quantity { get; set; }

        [Column("Remarks")]
        public string? Remarks { get; set; }


    }
}
